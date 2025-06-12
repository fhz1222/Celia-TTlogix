using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using QRCoder;
using ServiceResult;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TT.Common;
using TT.Common.Extensions;
using TT.Core.Entities;
using TT.Core.Enums;
using TT.Core.Interfaces;
using TT.Core.QueryFilters;
using TT.Core.QueryResults;
using TT.DB;
using TT.Services.Interfaces;
using TT.Services.Models;
using TT.Services.Services.Utilities;
using TT.Services.Utilities;

namespace TT.Services.Services
{
    public class OutboundService : ServiceBase<OutboundService>, IOutboundService
    {
        public OutboundService(ITTLogixRepository repository,
            IOptions<AppSettings> appSettings,
            IMapper mapper,
            IUtilityService utilityService,
            IEKanbanService eKanbanService,
            IReportService reportService,
            ILoggerService loggerService,
            IStorageService storageService,
            ILocker locker,
            IILogConnect iLogConnect,
            ILogger<OutboundService> logger,
            IBillingService billingService,
            ITTLogixRepositoryForOutboundImportEKanban ttLogixRepositoryForOutboundImportEKanban = null) : base(locker, logger)
        {
            this.repository = repository;
            this.appSettings = appSettings.Value;
            this.mapper = mapper;
            this.utilityService = utilityService;
            this.eKanbanService = eKanbanService;
            this.loggerService = loggerService;
            this.storageService = storageService;
            this.billingService = billingService;
            this.reportService = reportService;
            this.iLogConnect = iLogConnect;
            if (ttLogixRepositoryForOutboundImportEKanban != null)
            {
                this.outboundImportEKanbanFastService = new OutboundImportEKanbanFastService(
                    ttLogixRepositoryForOutboundImportEKanban,
                    appSettings,
                    mapper,
                    utilityService,
                    eKanbanService,
                    loggerService
                );
            }
        }

        public async Task<OutboundListDto> GetOutboundList(OutboundListQueryFilter filter)
        {
            var query = repository.GetOutboundList<OutboundListItemDto>(filter);

            var pagedQuery = query.Skip(filter.PageSize * (filter.PageNo - 1)).Take(filter.PageSize);
            var total = await query.CountAsync();
            var data = await pagedQuery.ToListAsync();

            return new OutboundListDto
            {
                Data = data,
                PageSize = filter.PageSize,
                PageNo = filter.PageNo,
                Total = total
            };
        }

        public async Task<OutboundDto> GetOutbound(string jobNo)
        {
            var entity = await repository.GetOutboundAsync(jobNo);
            if (entity != null)
            {
                var reports = await repository.GetLastReportPrintingLogs(jobNo);
                var calculatedNoOfPallet = await repository.GetNoOfPalletsForOutbound(jobNo);
                var hasCancelledOrderLines = await repository.HasCancelledOrderLines(entity.RefNo);

                var preventedStatuses = new OutboundStatus[] 
                { 
                    OutboundStatus.Picked, 
                    OutboundStatus.Packed, 
                    OutboundStatus.InTransit,
                    OutboundStatus.Completed,
                    OutboundStatus.Cancelled,
                    OutboundStatus.Discrepancy 
                };

                return mapper.Map<OutboundDto>(entity, opts =>
                {
                    opts.AfterMap((src, dest) =>
                    {
                        dest.ReportsPrinted = mapper.Map<IEnumerable<ReportPrintedDto>>(reports).OrderByDescending(x => x.PrintedDate);
                        dest.CalculatedNoOfPallet = calculatedNoOfPallet;
                        dest.AllowAutoallocation = entity.Status.NotIn(preventedStatuses) && entity.TransType != OutboundType.EKanban;
                        dest.ShowOrderSummary = entity.TransType == OutboundType.EKanban && hasCancelledOrderLines;
                    });
                });
            }
            return null;
        }

        public async Task<IEnumerable<OutboundDetailDto>> GetOutboundDetailList(string jobNo)
        {
            return await repository.GetOutboundDetailList<OutboundDetailDto>(jobNo);
        }

        public async Task<IEnumerable<OutboundDetailDto>> GetOutboundDetailWithReceivedQtyList(string jobNo)
        {
            return await repository.GetOutboundDetailWithReceivedQtyList<OutboundDetailDto>(jobNo);
        }

        public async Task<Result<string>> CreateOutboundManual(OutboundManualDto outboundDto, string userCode)
        {
            return await WithTransactionScope(async () =>
            {
                var entity = mapper.Map<Outbound>(outboundDto);
                entity.CreatedBy = userCode;
                var result = await CreateOutbound(entity);

                if (result.ResultType == ResultType.Ok)
                {
                    var jobNo = result.Data;
                    if (outboundDto.ManualType != Models.ModelEnums.ManualType.Empty)
                    {
                        var resultOrderNo = await eKanbanService.CreateEKanbanManual(jobNo, entity.CustomerCode, outboundDto.ManualType);

                        if (resultOrderNo.ResultType != ResultType.Ok)
                        {
                            return resultOrderNo;
                        }
                        entity.RefNo = resultOrderNo.Data;
                        await repository.SaveChangesAsync();
                    }
                    return new SuccessResult<string>(jobNo);
                }
                return result;
            });
        }

        private async Task<Result<string>> CreateOutbound(Outbound entity)
        {
            //step 1 : Generate Outbound JobNo
            var jobNoResult = await utilityService.GenerateJobNo(JobType.Outbound);
            if (jobNoResult.ResultType == ResultType.Invalid)
            {
                return new InvalidResult<string>(jobNoResult.Errors[0]);
            }
            //Step 2 : Insert into Outbound
            entity.JobNo = jobNoResult.Data;
            await repository.AddOutboundAsync(entity);
            return new SuccessResult<string>(entity.JobNo);
        }

        public async Task<Result<Outbound>> UpdateOutbound(string jobNo, OutboundDto outboundDto)
        {
            if(await iLogConnect.IsProcessingOutbound(jobNo))
                return new InvalidResult<Outbound>(new JsonResultError("ILogIsProcessingThisOutbound").ToJson());

            return await WithTransactionScope<Outbound>(async () =>
            {
                return await UpdateOutboundAsync(jobNo, outboundDto);
            });
        }

        private async Task<Result<Outbound>> UpdateOutboundAsync(string jobNo, OutboundDto outboundDto, bool updateStatus = true)
        {
            var entity = await repository.GetOutboundAsync(jobNo);
            if (entity == null)
            {
                return new NotFoundResult<Outbound>(new JsonResultError("RecordNotFound").ToJson());
            }

            mapper.Map(outboundDto, entity);
            await repository.SaveChangesAsync();

            // #2803 missing recalculation of the Outbound status
            if (updateStatus)
                await UpdateOutboundStatusAsync(entity, Ownership.Supplier, 0);

            return new SuccessResult<Outbound>(entity);
        }

        public async Task<Result<bool>> CancelOutbound(string jobNo, string userCode)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                #region Check Picking List is Empty  -->added by biao

                if (await repository.PickingLists().Where(pl => pl.JobNo == jobNo).AnyAsync())
                    return new InvalidResult<bool>(new JsonResultError("ThereAreItemsInPickingListPleaseUndo").ToJson());

                #endregion

                #region Update Inventory --> added by biao
                //Adjust the Inventory AllocatedQty 

                var outbound = await repository.GetOutboundAsync(jobNo);

                if (outbound.Status == OutboundStatus.TruckDeparture)
                    return Error<bool>.Get("InvalidOutboundStatusToCancel");

                var groupedByProductCode = (await repository.OutboundDetails().Where(o => o.JobNo == jobNo).ToListAsync())
                            .GroupBy(o => o.ProductCode);
                foreach (var g in groupedByProductCode)
                {
                    var inventory = await repository.GetInventoryAsync(outbound.CustomerCode, g.First().SupplierID, g.Key, outbound.WHSCode, (byte)Ownership.Supplier);
                    if (inventory != null)
                    {
                        inventory.AllocatedQty -= g.Sum(o => o.Qty);
                    }
                }
                await repository.SaveChangesAsync();
                #endregion

                #region Update Kanban header status to "New"
                if (outbound.TransType == OutboundType.EKanban)
                {
                    var eKanbanHeader = await repository.GetEKanbanHeaderAsync(outbound.RefNo);
                    if (eKanbanHeader == null || eKanbanHeader.Status != (int)EKanbanStatus.Imported)
                        return new InvalidResult<bool>(new JsonResultError("InvalidEKanbanHeaderStatus").ToJson());

                    eKanbanHeader.Status = (int)EKanbanStatus.New;
                    eKanbanHeader.OutJobNo = "";
                    await repository.SaveChangesAsync();

                    if (appSettings.OwnerCode.StartsWith(UtilityService.TESA_CODE))
                    {
                        var ekanbanDetailsToDelete = await repository.EKanbanDetails().Where(e => e.OrderNo == outbound.RefNo
                                                && e.Quantity == 0).ToListAsync();

                        foreach (var item in ekanbanDetailsToDelete)
                        {
                            await repository.DeleteEKanbanDetailAsync(item);
                        }
                        var eordersToDelete = await repository.EOrders().Where(e => e.PurchaseOrderNo == outbound.RefNo
                                                    && e.OrderQuantity == "0").ToListAsync();
                        foreach (var item in eordersToDelete)
                        {
                            await repository.DeleteEOrderAsync(item);
                        }
                    }
                }
                else if (!String.IsNullOrEmpty(outbound.RefNo))
                {
                    var eKanbanHeader = await repository.GetEKanbanHeaderAsync(outbound.RefNo);
                    if (eKanbanHeader == null || eKanbanHeader.Status != (int)EKanbanStatus.Imported)
                    {
                        return new InvalidResult<bool>(new JsonResultError("InvalidEKanbanHeaderStatus").ToJson());
                    }

                    eKanbanHeader.Status = (int)EKanbanStatus.Cancelled;
                    eKanbanHeader.OutJobNo = "";
                    await repository.SaveChangesAsync();
                }
                #endregion

                outbound.Status = OutboundStatus.Cancelled;
                outbound.CancelledBy = userCode;
                outbound.CancelledDate = DateTime.Now;
                await repository.SaveChangesAsync();

                #region Check if outbound job added to loading job or not

                var batchDeleteLoadingResult = await DeleteLoadingDetail(outbound.RefNo, userCode);
                if (batchDeleteLoadingResult.ResultType != ResultType.Ok)
                    return batchDeleteLoadingResult;

                #endregion

                return new SuccessResult<bool>(true);
            });
        }

        private async Task<Result<bool>> DeleteLoadingDetail(string orderNumber, string userCode)
        {
            var loadingDetail = await repository.LoadingDetails().Where(l => l.OrderNo == orderNumber).FirstOrDefaultAsync();
            if (loadingDetail == null) return new SuccessResult<bool>(true);

            //step 1 : Get Loading
            var jobNo = loadingDetail.JobNo;
            var loading = await repository.GetLoadingAsync(jobNo);

            //Step 2 : Insert Into Loading Detail (or rather delete loading detail - AU)
            await repository.DeleteLoadingDetailAsync(loadingDetail);

            if (appSettings.IsSAPFactory(loading.CustomerCode))
            {
                var ekanbanHeaders = repository.EKanbanHeaders().Where(h =>
                            h.OrderNo == orderNumber
                            && h.Status == (int)EKanbanStatus.DataSent)
                    .ToList();
                foreach (var item in ekanbanHeaders)
                {
                    item.Status = (int)EKanbanStatus.InTransit;
                }
            }
            var loadingStatus = await RefreshLoadingStatus(jobNo);
            loading.Status = loadingStatus;
            loading.RevisedBy = userCode;
            loading.RevisedDate = DateTime.Now;

            await repository.SaveChangesAsync();
            return new SuccessResult<bool>(true);
        }

        private async Task<LoadingStatus> RefreshLoadingStatus(string jobNo)
        {
            var outboundStatuses = (await (from ld in repository.LoadingDetails()
                                           join ekh in repository.EKanbanHeaders() on ld.OrderNo equals ekh.OrderNo
                                           join ob in repository.Outbounds() on ekh.OutJobNo equals ob.JobNo
                                           where ld.JobNo == jobNo
                                           select ob.Status).ToListAsync()).Distinct();

            var loadingstatus = LoadingStatus.NewJob;

            if (outboundStatuses.Any())
            {
                foreach (var status in outboundStatuses)
                {
                    if (status >= OutboundStatus.Picked && status < OutboundStatus.Cancelled)
                    {
                        loadingstatus = LoadingStatus.Picked;
                    }
                    else if (status >= (int)OutboundStatus.NewJob && status < OutboundStatus.Picked)
                    {
                        if (loadingstatus != LoadingStatus.NewJob)
                            loadingstatus = LoadingStatus.Processing;
                    }
                }
            }
            return loadingstatus;
        }

        public async Task<Result<bool>> UpdateOutboundStatus(string jobNo)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                var outbound = await repository.GetOutboundAsync(jobNo);
                await UpdateOutboundStatusAsync(outbound, Ownership.Supplier, 0);
                return new SuccessResult<bool>(true);
            });
        }

        private async Task UpdateOutboundStatusAsync(Outbound outbound, Ownership ownership, decimal masterPIDQty)
        {
            var outboundDetailPickingList = await repository.GetOutboundDetailPickingResultList(outbound.JobNo);
            bool isPartialPicked = false;
            bool noPick = true;

            foreach (var row in outboundDetailPickingList)
            {
                if (row.TotalPickedQty > 0)
                    noPick = false;

                if (row.OutboundDetail.Qty == row.TotalPickedQty)
                {
                    row.OutboundDetail.Status = (int)OutboundDetailStatus.Picked;
                }
                else
                {
                    row.OutboundDetail.Status = (int)OutboundDetailStatus.Picking;
                    isPartialPicked = true;
                }
                //Step 4 : Update Outbound Detail
                //await UpdateOutboundDetail(outbound, row.OutboundDetail, masterPIDQty, ownership, row.OutboundDetail.Qty);
                if (!appSettings.OwnerCode.StartsWith(UtilityService.TESA_CODE))
                {
                    var inventory = await repository.GetInventoryAsync(outbound.CustomerCode, row.OutboundDetail.SupplierID, row.OutboundDetail.ProductCode, outbound.WHSCode, ownership);
                    inventory.AllocatedQty -= masterPIDQty;
                }
                await repository.SaveChangesAsync();
            }

            if (outbound.Status == (int)OutboundStatus.NewJob || outbound.Status == OutboundStatus.PartialPicked || outbound.Status == OutboundStatus.Picked)
            {
                if (noPick)
                    outbound.Status = (int)OutboundStatus.NewJob;
                else if (isPartialPicked)
                    outbound.Status = OutboundStatus.PartialPicked;
                else
                {
                    outbound.Status = OutboundStatus.Picked;
                }
            }

            await repository.SaveChangesAsync();
        }

        public async Task<Result<string>> ImportEKanbanEUCPart(string orderNo, string factoryId, string whsCode, string userCode)
        {
            return await WithTransactionScope<string>(async () =>
            {
                // Validate Kanban header status
                var eKanbanHeaderResult = await eKanbanService.GetEKanbanHeaderForImport(orderNo, whsCode);
                if (eKanbanHeaderResult.ResultType != ResultType.Ok)
                {
                    return new InvalidResult<string>(eKanbanHeaderResult.Errors.First());
                }

                var importedOutboundJobNo = await GetImportedJobNo(orderNo);
                var isOrderAlreadyImported = importedOutboundJobNo != null;
                if (isOrderAlreadyImported)
                {
                    return new InvalidResult<string>(new JsonResultError
                    {
                        MessageKey = "UnableToImportOrderImported__",
                        Arguments = new Dictionary<string, string>
                    {
                        { "orderNo", orderNo },
                        { "importedJobNo", importedOutboundJobNo }
                    }
                    }.ToJson());
                }

                var eKanbanHeader = eKanbanHeaderResult.Data;

                if (eKanbanHeader.Instructions == "EHP")
                {
                    return await ImportEHPCallOff(eKanbanHeader, whsCode, userCode);
                }
                else
                {
                    if (outboundImportEKanbanFastService == null)
                        return await ImportEKanbanEUCPartInternal(eKanbanHeader, whsCode, userCode);
                    else
                        return await outboundImportEKanbanFastService.ImportEKanbanEUCPartInternal(eKanbanHeader, whsCode, userCode);
                }
            });
        }

        public async Task<Result<bool>> CompleteOutboundEurope(IEnumerable<string> jobNos, string userCode, bool withTransaction = true)
        {
            loggerService.LogInformation($"CompleteOutboundEurope (jobNo {string.Join(",", jobNos.ToArray())} - started");

            return await CompleteOutbound(jobNos, userCode, false, false, withTransaction);
        }

        public async Task<Result<bool>> CompleteOutboundReturn(IEnumerable<string> jobNos, string userCode, bool withTransaction = true)
        {
            return await CompleteOutbound(jobNos, userCode, true, false, withTransaction);
        }

        public async Task<Result<bool>> CompleteOutboundManual(IEnumerable<string> jobNos, string userCode, bool withTransaction = true)
        {
            return await CompleteOutbound(jobNos, userCode, false, true, withTransaction);
        }

        private bool IsInvalidType(Outbound outbound, bool isManual, bool isReturn)
        {
            if (isManual)
                return outbound.TransType != OutboundType.ManualEntry && outbound.TransType != OutboundType.ScannerManualEntry;
            else if (isReturn)
                return outbound.TransType != OutboundType.Return;
            else
                return outbound.TransType != OutboundType.EKanban;
        }

        private async Task<Result<bool>> CompleteOutbound(IEnumerable<string> jobNos, string userCode, bool isReturn, bool isManual, bool withTransaction = true)
        {
            if (withTransaction)
                return await WithTransactionScope<bool>(async () =>
                {
                    return await CompleteOutboundInternal(jobNos, userCode, isReturn, isManual);
                });
            else
                return await CompleteOutboundInternal(jobNos, userCode, isReturn, isManual);
        }

        private async Task<Result<bool>> CompleteOutboundInternal(IEnumerable<string> jobNos, string userCode, bool isReturn, bool isManual)
        {
            loggerService.LogInformation($"CompleteOutboundInternal (jobNo {string.Join(",", jobNos.ToArray())} - started");

            // get all objects i.e. outbound, picking lists, storage, inventory in one go.
            var dataToUpdate = (await repository.GetCompleteOutboundData(jobNos))
                               .GroupBy(o => o.Outbound.JobNo)
                               .ToDictionary(o => o.Key, o => o.ToList());

            var outbounds = dataToUpdate.Values.Select(o => o.FirstOrDefault().Outbound).Distinct().ToList();

            foreach (var jobNo in jobNos)
            {
                #region Step 1 : Get Outbound
                if (!dataToUpdate.TryGetValue(jobNo, out List<CompleteOutboundQueryResult> outboundRelatedData))
                {
                    return new InvalidResult<bool>(new JsonResultError("CannotFindOutboundForJobNo__", "jobNo", jobNo).ToJson());
                }
                var outbound = outboundRelatedData.First().Outbound;
                if (IsInvalidType(outbound, isManual, isReturn))
                {
                    return new InvalidResult<bool>(new JsonResultError("InvalidJobTypeForJobNo__", "jobNo", jobNo.Trim()).ToJson());
                }
                #endregion

                #region Step 2 : Get Picking List
                var pickingLists = outboundRelatedData.OrderBy(i => i.PickingList.LineItem).ThenBy(i => i.PickingList.SeqNo)
                    .GroupBy(pl => pl.PickingList)
                    .ToDictionary(pl => pl.Key, pl => pl.First());
                if (!pickingLists.Any())
                    return new InvalidResult<bool>(new JsonResultError("EmptyPickingListForJobNo__", "jobNo", jobNo.Trim()).ToJson());

                #endregion

                #region Step 2.1 : Perform Checking
                if (pickingLists.Values.Select(p => p.StorageDetail.Ownership).Distinct().Count() > 1)
                {
                    return new InvalidResult<bool>(new JsonResultError("OutboundHasMixedOwnership__", "jobNo", jobNo).ToJson());
                }
                if (pickingLists.Keys.Any(pickingListItem => String.IsNullOrEmpty(pickingListItem.PickedBy)))
                {
                    return new InvalidResult<bool>(new JsonResultError("CompleteReturnFailProductNotScanned").ToJson());
                }
                #endregion
                var j = -1;
                foreach (var pickingList in pickingLists)
                {
                    j++;
                    var pickingListItem = pickingList.Key;
                    loggerService.LogInformation($"CompleteOutboundInternal (jobNo {string.Join(",", jobNos.ToArray())} - pickingList {pickingListItem.PID} parsed");

                    #region Step 2.2 : Get Storage Detail Instance
                    var storageDetail = pickingList.Value.StorageDetail;
                    if (storageDetail == null)
                        return new InvalidResult<bool>(new JsonResultError("FailToRetrieveStorageDetailForPID__", "pid", pickingListItem.PID).ToJson());
                    #endregion

                    #region Step 2.3 : Checking for Inbound Status
                    var inbound = pickingList.Value.Inbound;
                    if (inbound == null)
                        return new InvalidResult<bool>(new JsonResultError("FailToRetrieveInboundForPID__", "pid", storageDetail.PID).ToJson());

                    if (inbound.Status != InboundStatus.Completed)
                        return new InvalidResult<bool>(new JsonResultError()
                        {
                            MessageKey = "InboundJobNotCompletedForPID__",
                            Arguments = new Dictionary<string, string>(){
                                { "jobNo", inbound.JobNo },
                                {"pid",  storageDetail.PID } }
                        }.ToJson());
                    #endregion

                    #region step 2.4 : Update Inventory (-Allocated;-Onhand;+Transit)
                    if (isReturn || isManual || storageDetail.LocationCode != Enum.GetName(typeof(ExtSystemLocation), (int)ExtSystemLocation.RETURN))
                    {
                        var inventory = pickingList.Value.Inventory;
                        if (inventory == null)
                            return new InvalidResult<bool>(new JsonResultError("FailToRetrieveInventoryInstance").ToJson());

                        inventory.OnHandQty -= pickingListItem.Qty;
                        inventory.AllocatedQty -= pickingListItem.Qty;

                        inventory.OnHandPkg -= Convert.ToInt32(Math.Ceiling(storageDetail.Qty / storageDetail.QtyPerPkg));
                        inventory.AllocatedPkg -= Convert.ToInt32(Math.Ceiling(storageDetail.Qty / storageDetail.QtyPerPkg));
                    }
                    #endregion

                    #region Step 2.5 : Update Storage Detail
                    storageDetail.Qty -= pickingListItem.Qty;
                    if (storageDetail.Qty < 0)
                        return new InvalidResult<bool>(new JsonResultError("InvalidCartonQtyForPID__", "pid", pickingListItem.PID).ToJson());

                    storageDetail.AllocatedQty -= pickingListItem.Qty;
                    if (storageDetail.AllocatedQty < 0)
                        return new InvalidResult<bool>(new JsonResultError("InvalidAllocatedQtyForPID__", "pid", pickingListItem.PID).ToJson());

                    if (isReturn || storageDetail.LocationCode != Enum.GetName(typeof(ExtSystemLocation), (int)ExtSystemLocation.RETURN))
                        storageDetail.Status = StorageStatus.Dispatched;
                    if (storageDetail.GroupID != null)
                    {
                        var storageDetailGroup = await repository.GetStorageDetailGroupAsync(storageDetail.GroupID);
                        if (storageDetailGroup == null)
                        {
                            return new InvalidResult<bool>(new JsonResultError("FailToRetrieveStorageDetailGroupInstance").ToJson());
                        }
                        storageDetailGroup.Quantity = storageDetailGroup.Quantity - 1;
                        if (storageDetailGroup.Quantity == 0)
                        {
                            storageDetailGroup.ClosedDate = DateTime.Now;
                        }
                    }
                    #endregion

                    // AU: fix - sometimes manual job is really ekanban, so try to create anyway
                    // TODO investigate if this operation should be done for all: manual, eKanban and return types
                    if (isManual)
                    {
                        #region Step 2.6: Insert EKanbanDetail
                        var existingEKanban = await repository.GetEKanbanDetailAsync(outbound.RefNo, pickingListItem.ProductCode, (j + 1).ToString());
                        if (existingEKanban == null)
                        {
                            var eKANBANDetail = new EKanbanDetail
                            {
                                OrderNo = outbound.RefNo,
                                ProductCode = pickingListItem.ProductCode,
                                SerialNo = (j + 1).ToString(),
                                SupplierID = pickingListItem.SupplierID,
                                Quantity = pickingListItem.Qty,
                                QuantitySupplied = pickingListItem.Qty,
                                QuantityReceived = pickingListItem.Qty,
                                DropPoint = "ZZ99"
                            };
                            await repository.AddEKanbanDetailAsync(eKANBANDetail);
                        }
                        #endregion

                        #region Step 2.7: Insert TT_PickingListEKanban
                        var existingplek = await repository.GetPickingListEKanbanAsync(outbound.JobNo, pickingListItem.LineItem, pickingListItem.SeqNo);
                        if (existingplek == null)
                        {
                            var pickingListEKanban = new PickingListEKanban
                            {
                                JobNo = outbound.JobNo,
                                LineItem = pickingListItem.LineItem,
                                SeqNo = pickingListItem.SeqNo,
                                OrderNo = outbound.RefNo,
                                SerialNo = (j + 1).ToString(),
                                ProductCode = pickingListItem.ProductCode
                            };
                            await repository.AddPickingListEKanbanAsync(pickingListEKanban);
                        }
                        #endregion
                    }
                }

                #region step 3 : Get Picking List Group By Product Group

                var pickingListForJobWithStorageDetail = outboundRelatedData
                    .Where(o => isReturn || isManual || o.StorageDetail.LocationCode != Enum.GetName(typeof(ExtSystemLocation), (int)ExtSystemLocation.RETURN))
                    .Select(o => new { pl = o.PickingList, sd = o.StorageDetail })
                    .ToList();

                var pickingListGroupByProductCode = pickingListForJobWithStorageDetail.GroupBy(g => g.pl.ProductCode)
                    .Select(g => new
                    {
                        ProductCode = g.Key,
                        TotalPickedQty = g.Sum(v => v.pl.Qty),
                        TotalPickedPkg = g.Sum(v => Math.Ceiling(v.pl.Qty / (v.sd.QtyPerPkg > 0 ? v.sd.QtyPerPkg : 1)))
                    })
                    .OrderBy(r => r.ProductCode);

                #endregion

                foreach (var pickingListGroupRow in pickingListGroupByProductCode)
                {
                    loggerService.LogInformation($"CompleteOutboundInternal (jobNo {string.Join(",", jobNos.ToArray())} - pickingListGroup for product {pickingListGroupRow.ProductCode} parsed");
                    #region step 3.1 : Insert into InventoryTransaction
                    var l_dstInvTrans = await repository.GetInventoryLastTransactionBalance(outbound.CustomerCode, pickingListGroupRow.ProductCode);
                    if (!l_dstInvTrans.HasValue)
                        return new InvalidResult<bool>(new JsonResultError("FailToRetrieveInventoryForProductCode__", "productCode", pickingListGroupRow.ProductCode).ToJson());

                    var l_oInvTrans = new InvTransaction
                    {
                        JobNo = jobNo,
                        ProductCode = pickingListGroupRow.ProductCode,
                        Qty = Convert.ToDouble(pickingListGroupRow.TotalPickedQty),
                        CustomerCode = outbound.CustomerCode,
                        Pkg = Convert.ToInt64(pickingListGroupRow.TotalPickedPkg),
                        JobDate = outbound.ETD,
                        Act = (int)InventoryTransactionType.Outbound,
                        BalanceQty = l_dstInvTrans.Value.BalanceQty - Convert.ToDouble(pickingListGroupRow.TotalPickedQty),
                        BalancePkg = l_dstInvTrans.Value.BalancePkg - Convert.ToInt64(pickingListGroupRow.TotalPickedPkg)
                    };

                    if (l_oInvTrans.BalanceQty < 0)
                        return new InvalidResult<bool>(new JsonResultError("InvalidBalanceQty").ToJson());

                    /* Reason for why this checking is being taken out;
                     * On MSN:
                     *	Zhixian says:
                            btw, regarding the logix cargo out prb, they have 1 label = many packages
                            so this feature cannot take out already  
                        Jason Tan says:
                            but we do not have to save the no of packages at this time
                        Zhixian says:
                            yeah, I suppose we do not have to, but we are anyways
                            the no of packages are saved in inventory (i think)
                        Jason Tan says:
                            remove calculation of no of packages for now
                            and do not display the no of packages in the inventory manager
                        Zhixian	says:
                            k
                    */
                    //							if (l_oInvTrans.BalancePkg < 0)
                    //								throw new Exception("Invalid BALANCE PACKAGE in inventory transaction");

                    await repository.AddInvTransactionAsync(l_oInvTrans, false);

                    #endregion

                    #region step 3.2 : Insert into InventoryTransactionPerWHS

                    var l_dstInvWHSTrans = await repository.GetInventoryLastTransactionPerWHSBalance(outbound.CustomerCode, pickingListGroupRow.ProductCode, outbound.WHSCode);
                    if (!l_dstInvWHSTrans.HasValue)
                        return new InvalidResult<bool>(new JsonResultError("FailToRetrieveInventoryForProductCode__", "productCode", pickingListGroupRow.ProductCode).ToJson());

                    var l_oInvWHSTrans = new InvTransactionPerWHS
                    {
                        JobNo = jobNo,
                        ProductCode = pickingListGroupRow.ProductCode,
                        Qty = Convert.ToDouble(pickingListGroupRow.TotalPickedQty),
                        CustomerCode = outbound.CustomerCode,
                        WHSCode = outbound.WHSCode,
                        Pkg = Convert.ToInt64(pickingListGroupRow.TotalPickedPkg),
                        JobDate = outbound.ETD,
                        Act = (int)InventoryTransactionType.Outbound,
                        BalanceQty = l_dstInvWHSTrans.Value.BalanceQty - Convert.ToDouble(pickingListGroupRow.TotalPickedQty),
                        BalancePkg = l_dstInvWHSTrans.Value.BalancePkg - Convert.ToInt64(pickingListGroupRow.TotalPickedPkg)
                    };

                    if (l_oInvWHSTrans.BalanceQty < 0)
                        return new InvalidResult<bool>(new JsonResultError("InvalidBalanceQty").ToJson());

                    // Tentatively Ignore this part, as user are free to change the no of package upon key in inbound, 
                    // that might be possible where BalancePkg fall under 0 as PickingList use different
                    /*
                    if (l_oInvWHSTrans.BalancePkg < 0)
                        throw new Exception("Invalid BALANCE PACKAGE in inventory warehouse transaction");
                    */

                    await repository.AddInvTransactionPerWHSAsync(l_oInvWHSTrans, false);
                    #endregion
                }

                #region step 4 : Get Picking List Group By Product Group By Supplier
                var pickingListGroupByProductCodeAndSupplier = pickingListForJobWithStorageDetail
                    .GroupBy(g => new { g.pl.ProductCode, g.pl.SupplierID, g.sd.Ownership })
                    .Select(g => new
                    {
                        g.Key.ProductCode,
                        g.Key.SupplierID,
                        g.Key.Ownership,
                        TotalPickedQty = g.Sum(v => v.pl.Qty),
                        TotalPickedPkg = g.Sum(v => Math.Ceiling(v.pl.Qty / v.sd.QtyPerPkg > 0 ? v.sd.QtyPerPkg : 1))
                    })
                    .OrderBy(r => r.ProductCode).ThenBy(r => r.SupplierID);

                #endregion

                foreach (var pickingListGroupRow in pickingListGroupByProductCodeAndSupplier)
                {
                    loggerService.LogInformation($"CompleteOutboundInternal (jobNo {string.Join(",", jobNos.ToArray())} - pickingListGroup for product {pickingListGroupRow.ProductCode}, supplierID {pickingListGroupRow.SupplierID} parsed");
                    #region step 4.1 : Insert into InvTransactionPerSupplier
                    var l_dstInvWHSTransSupplier = await repository.GetInventoryLastTransactionPerSupplierBalance(outbound.CustomerCode, pickingListGroupRow.ProductCode, pickingListGroupRow.SupplierID, pickingListGroupRow.Ownership);
                    if (!l_dstInvWHSTransSupplier.HasValue)
                        return new InvalidResult<bool>(new JsonResultError("FailToRetrieveLastInventoryTransaction").ToJson());

                    var l_oInvTransactionPerSupplier = new InvTransactionPerSupplier
                    {
                        JobNo = outbound.JobNo,
                        ProductCode = pickingListGroupRow.ProductCode,
                        SupplierID = pickingListGroupRow.SupplierID,
                        Ownership = pickingListGroupRow.Ownership,
                        CustomerCode = outbound.CustomerCode,
                        JobDate = outbound.CreatedDate,
                        Act = (int)InventoryTransactionType.Outbound,
                        Qty = pickingListGroupRow.TotalPickedQty,
                        BalanceQty = l_dstInvWHSTransSupplier.Value - pickingListGroupRow.TotalPickedQty
                    };

                    await repository.AddInvTransactionPerSupplierAsync(l_oInvTransactionPerSupplier, false);
                    #endregion
                }
                #region Step 3 : Update Outbound
                outbound.Status = OutboundStatus.Completed;
                outbound.DispatchedBy = userCode;
                outbound.DispatchedDate = DateTime.Now;

                #endregion
                if (!isReturn)
                {
                    var billingDetails = await repository.GetOutboundBillingDetail(outbound.RefNo);
                    loggerService.LogInformation($"CompleteOutboundInternal (jobNo {string.Join(",", jobNos.ToArray())} - retrieve billing details");
                    
                    foreach (var billingRow in billingDetails)
                    {
                        #region Write to BillingLog
                        await billingService.WriteToBillingLog(
                            billingRow.OutJobNo,
                            billingRow.FactoryID,
                            billingRow.SupplierID,
                            billingRow.ProductCode,
                            billingRow.OrderNo,
                            billingRow.Qty,
                            billingRow.BillingNo);
                        #endregion
                        loggerService.LogInformation($"CompleteOutboundInternal (jobNo {string.Join(",", jobNos.ToArray())} - write billing details {billingRow.BillingNo}");
                    }
                }
            }

            if (!isReturn)
            {
                #region Step 5 : Update EkanbanHeader
                loggerService.LogInformation($"CompleteOutboundInternal (jobNo {string.Join(",", jobNos.ToArray())} - Update EkanbanHeader");
                var orderNosToComplete = outbounds.Where(outbound =>
                    (outbound.RefNo.IndexOf("TTK") == 0 && appSettings.OwnerCode == "TESAH") ||
                    (appSettings.OwnerCode == "TESAG" && outbound.CustomerCode != "DGT") ||
                    (outbound.CustomerCode == "VRN"))
                    .Select(o => o.RefNo?.Trim()).Distinct();
                if (orderNosToComplete.Any())
                    await repository.EKanbanHeaderBatchUpdateStatus(EKanbanStatus.Completed, true, orderNosToComplete);

                var orderNosInTransit = outbounds.Select(o => o.RefNo?.Trim())
                    .Where(o => !orderNosToComplete.Contains(o))
                    .Distinct();

                if (orderNosInTransit.Any())
                    await repository.EKanbanHeaderBatchUpdateStatus(EKanbanStatus.InTransit, true, orderNosInTransit);
                #endregion

                #region Step 6 : Update EkanbanDetail
                loggerService.LogInformation($"CompleteOutboundInternal (jobNo {string.Join(",", jobNos.ToArray())} - Update EkanbanDetail");
                var ordernos = outbounds.Select(o => o.RefNo?.Trim()).Distinct().ToArray();
                await repository.EKanbanDetailsBatchUpdateQtyReceived(ordernos);
                #endregion
            }

            await repository.SaveChangesAsync();

            return new SuccessResult<bool>(true);
        }

        public async Task<Result<bool>> CargoInTransit(IEnumerable<string> jobNos, string userCode, bool withTransaction = true)
        {
            if (withTransaction)
                return await WithTransactionScope<bool>(async () =>
                {
                    return await CargoInTransitInternal(jobNos, userCode);
                });
            else
                return await CargoInTransitInternal(jobNos, userCode);
        }

        public async Task<Result<bool>> CargoInTransitInternal(IEnumerable<string> jobNos, string userCode)
        {
            foreach (var jobNo in jobNos)
            {
                #region Step 1 :  Get Ooutbound object
                var outbound = await repository.GetOutboundAsync(jobNo);
                #endregion

                #region Step 2 :  Get Picking list for cargo out checking
                var pickingListForCargoOutChecking = await repository.GetPickingListForCargoOutChecking(jobNo);

                if (!pickingListForCargoOutChecking.Any())
                {
                    if (outbound.TransType != OutboundType.EKanban)
                        return new InvalidResult<bool>(new JsonResultError("EmptyPickingListForJobNo__", "jobNo", jobNo).ToJson());
                }

                #endregion

                #region Step 3 :  Get outbounddetail list
                var outboundDetailRows = await repository.GetOutboundDetailList<OutboundDetailQueryResult>(jobNo);

                if (!outboundDetailRows.Any())
                {
                    //ekanban can return as empty (zero item picked)
                    if (outbound.TransType != OutboundType.EKanban)
                        return new InvalidResult<bool>(new JsonResultError("EmptyOutboundDetailForJobNo__", "jobNo", jobNo).ToJson());
                }
                #endregion

                int l_intProcessesPickingListItem = 0;

                foreach (var odrow in outboundDetailRows)
                {
                    var pickingListRowsForLineItem = pickingListForCargoOutChecking.Where(pl => pl.PickingList.LineItem == odrow.LineItem).ToList();

                    if (!pickingListRowsForLineItem.Any())
                        return new InvalidResult<bool>(new JsonResultError()
                        {
                            MessageKey = "NoPickingListForJobLine__",
                            Arguments = new Dictionary<string, string>(){
                                { "jobNo", jobNo },
                                {"lineItem", odrow.LineItem.ToString() }
                            }
                        }.ToJson());

                    decimal l_dblTotalPickedQty = 0;

                    #region Step 4 :  Check pickinglist
                    foreach (var pickingListRow in pickingListRowsForLineItem)
                    {
                        // Step 4.1 :  Check pickinglist (PID's pickedby != "")
                        if (String.IsNullOrEmpty(pickingListRow.PickingList.PickedBy))
                            return new InvalidResult<bool>(new JsonResultError
                            {
                                MessageKey = "CargoOutFailSomeItemMissing__",
                                Arguments = new Dictionary<string, string>
                                {
                                    {"productCode", pickingListRow.PickingList.ProductCode },
                                    { "supplierID", pickingListRow.PickingList.SupplierID
                                }
                                }
                            }.ToJson());
                        // Step 4.2 :  Check pickinglist (PID != "")
                        if (String.IsNullOrEmpty(pickingListRow.StockPID))
                            return new InvalidResult<bool>(new JsonResultError
                            {
                                MessageKey = "CargoOutFailInvalidPicked__",
                                Arguments = new Dictionary<string, string>
                                {
                                    {"pid",  pickingListRow.PickingList.PID },
                                    { "lineItem", pickingListRow.PickingList.LineItem.ToString() }
                                    }
                            }.ToJson());

                        // Step 4.3 :  Check pickinglist (PID's inbound == completed)
                        if (pickingListRow.InboundStatus != (int)InboundStatus.Completed)
                            return new InvalidResult<bool>(new JsonResultError
                            {
                                MessageKey = "CargoOutFailInvalidStatus__",
                                Arguments = new Dictionary<string, string>
                                {
                                    {"inbound",  pickingListRow.InboundJobNo },
                                    {"pid",  pickingListRow.PickingList.PID },
                                    { "status", pickingListRow.InboundStatus.HasValue ? Enum.GetName(typeof(InboundStatus), (int)pickingListRow.InboundStatus.Value) : string.Empty }
                                    }
                            }.ToJson());

                        //Step 4.4 :  Check pickinglist against Storagedetail (Storagedetail.AllocatedQty!=pickinglist.Qty Or Storagedetail.Qty!=pickinglist.Qty)
                        if (pickingListRow.PickingList.Qty != pickingListRow.StockQty || pickingListRow.PickingList.Qty != pickingListRow.StockAllocatedQty)
                            return new InvalidResult<bool>(new JsonResultError("CargoOutFailInvalidPickedQty__", "pid", pickingListRow.PickingList.PID).ToJson());

                        // Step 4.5 :  Check pickinglist (PID's whscode != Outbound.whscode)
                        if (pickingListRow.StockWhsCode.ToUpper() != outbound.WHSCode.ToString().Trim().ToUpper())
                            return new InvalidResult<bool>(new JsonResultError
                            {
                                MessageKey = "CargoOutFailInvalidWHSCode__",
                                Arguments = new Dictionary<string, string>
                                {
                                    {"outboundWhs",  outbound.WHSCode },
                                    {"plWhs",  pickingListRow.StockWhsCode },
                                }
                            }.ToJson());

                        // Step 4.6 :  Check pickinglist (PID's outjobno != Outbound.jobno)
                        if (pickingListRow.OutboundJobNo.Trim().ToUpper() != outbound.JobNo.ToString().Trim())
                            return new InvalidResult<bool>(new JsonResultError
                            {
                                MessageKey = "CargoOutFailInvalidOutboundJobNo__",
                                Arguments = new Dictionary<string, string>
                                {
                                    {"outbound",  outbound.JobNo },
                                    {"pid",  pickingListRow.OutboundJobNo },
                                }
                            }.ToJson());

                        // Step 4.7 :  Check pickinglist (PID's CustomerCode != Outbound.CustomerCode)
                        if (pickingListRow.CustomerCode.Trim().ToUpper() != outbound.CustomerCode.ToString().Trim())
                            return new InvalidResult<bool>(new JsonResultError
                            {
                                MessageKey = "CargoOutFailInvalidCustomerCode__",
                                Arguments = new Dictionary<string, string>
                                {
                                    {"outbound",  outbound.CustomerCode },
                                    {"pid",  pickingListRow.CustomerCode },
                                }
                            }.ToJson());

                        l_dblTotalPickedQty += pickingListRow.PickingList.Qty;
                        l_intProcessesPickingListItem++;
                    }
                    #endregion
                    #region Step 5 :  Check pickinglist vs outbounddetail 
                    // Step 5.1 :  Check pickinglist vs outbounddetail (SupplierID, LineItem, ProductCode)
                    if (pickingListRowsForLineItem[0].PickingList.SupplierID.Trim().ToUpper() != odrow.SupplierID ||
                        pickingListRowsForLineItem[0].PickingList.ProductCode.Trim().ToUpper() != odrow.ProductCode.Trim().ToUpper())
                        return new InvalidResult<bool>(new JsonResultError
                        {
                            MessageKey = "CargoOutFailInvalidItem__",
                            Arguments = new Dictionary<string, string>
                                {
                                    {"lineItem",  odrow.LineItem.ToString() },
                                    {"productCode",  pickingListRowsForLineItem[0].PickingList.ProductCode },
                                    {"supplierID",  pickingListRowsForLineItem[0].PickingList.SupplierID },
                                }
                        }.ToJson());

                    // Step 5.2 :  Check pickinglist vs outbounddetail  (outbounddetail.pickedQty = outbounddetail.Qty = (groupby Productcode, sum(pickinglist.Qty)))
                    if (odrow.Qty != odrow.PickedQty || odrow.Qty != l_dblTotalPickedQty)
                        return new InvalidResult<bool>(new JsonResultError
                        {
                            MessageKey = "CargoOutFailInvalidPickedQtyForOutbound__",
                            Arguments = new Dictionary<string, string>
                                {
                                    {"qty",  odrow.Qty.ToString() },
                                    {"pickedQty",  odrow.PickedQty.ToString() },
                                    {"totalQty",  l_dblTotalPickedQty.ToString() },
                                }
                        }.ToJson());

                    #endregion
                    #region Step 6 :  update inventory (-Allocated;-Onhand;+Transit)
                    var inventory = await repository.GetInventoryAsync(outbound.CustomerCode, odrow.SupplierID, odrow.ProductCode, outbound.WHSCode, 0);
                    if (inventory == null)
                        return new InvalidResult<bool>(new JsonResultError("FailToRetrieveInventoryInstance").ToJson());

                    inventory.OnHandQty -= l_dblTotalPickedQty;
                    inventory.AllocatedQty -= l_dblTotalPickedQty;
                    inventory.TransitQty = (inventory.TransitQty ?? 0) + l_dblTotalPickedQty;

                    if (inventory.AllocatedQty < 0 || inventory.OnHandQty < 0 || inventory.OnHandQty < inventory.AllocatedQty)
                    {
                        string strErrorMessage =
                            "Invalid inventory status. CustomerCode[" + inventory.CustomerCode + "], " +
                            "ProductCode[" + inventory.ProductCode1 + "], " +
                            "SupplierID[" + inventory.SupplierID + "].";

                        await loggerService.LogError(jobNo, "CargoInTransit", strErrorMessage, false);
                    }

                    inventory.OnHandPkg -= (int)odrow.PickedPkg;
                    inventory.AllocatedPkg -= (int)odrow.PickedPkg;
                    inventory.TransitPkg = (inventory.TransitPkg ?? 0) + (int)odrow.PickedPkg;

                    await repository.SaveChangesAsync();
                    #endregion
                }

                #region Step 7 :  Check pickinglist items are all processed (to spot the record which in PickingList but not in OutboundDetail)
                if (l_intProcessesPickingListItem != pickingListForCargoOutChecking.Count())
                    return new InvalidResult<bool>(new JsonResultError("CargoOutFailInvalidDetail").ToJson());

                #endregion
                #region Step 8 :  Update storagedetail (-AllocatedQty;-Qty;status=InTransit) 
                if (pickingListForCargoOutChecking.Any())
                {
                    var storageDetailRows = await repository.StorageDetails().Where(sd => sd.OutJobNo == outbound.JobNo).ToListAsync();
                    foreach (var sd in storageDetailRows)
                    {
                        sd.Status = StorageStatus.InTransit;
                        sd.Qty = 0;
                        sd.AllocatedQty = 0;
                    }
                }

                #endregion
                #region Step 9 :  Update Outbound (Status = Intransit; DispatchedBy;DispatchedDate)
                outbound.Status = OutboundStatus.InTransit;
                outbound.DispatchedBy = userCode;
                outbound.DispatchedDate = DateTime.Now;

                #endregion
                #region Step 10 :  Update ekanbanHeader (Status = Intransit)
                var header = await repository.GetEKanbanHeaderAsync(outbound.RefNo);
                header.Status = (int)EKanbanStatus.InTransit;
                #endregion

                await repository.SaveChangesAsync();
            }

            return new SuccessResult<bool>(true);
        }

        private async Task<Result<string>> ImportEKanbanEUCPartInternal(EKanbanHeader l_oEKanbanHeader, string whsCode, string userCode)
        {
            var orderNo = l_oEKanbanHeader.OrderNo;
            var factoryId = l_oEKanbanHeader.FactoryID;
            loggerService.LogInformation($"ImportEKanbanEUCPartInternal (EKanban {orderNo} - import started");

            #region Step 1: Check validity of Part Number
            var supplierMasterResult = await eKanbanService.GetSupplierMasterForEKanbanImport(orderNo, factoryId, l_oEKanbanHeader.Instructions);
            if (supplierMasterResult.ResultType != ResultType.Ok)
                return new InvalidResult<string>(supplierMasterResult.Errors.First());

            var l_oSupplierMaster = supplierMasterResult.Data.Item1;
            var companyName = l_oSupplierMaster?.CompanyName ?? string.Empty;

            #endregion

            #region Step 2: Create Outbound Header

            var outbound = new Outbound
            {
                CustomerCode = factoryId,
                WHSCode = whsCode,
                RefNo = orderNo,
                TransType = OutboundType.EKanban,
                ETD = DateTime.Now,
                CreatedBy = userCode,
                Status = (int)OutboundStatus.NewJob,
                Remark = companyName
            };

            var createOutboundResult = await CreateOutbound(outbound);
            if (createOutboundResult.ResultType != ResultType.Ok)
                return new InvalidResult<string>(new JsonResultError("OutboundJobCreateError").ToJson());

            var jobNo = createOutboundResult.Data;

            loggerService.LogInformation($"ImportEKanbanEUCPartInternal (EKanban {orderNo}) - created TT_Outbound {jobNo}");

            #endregion

            // Step 3 to 10: Create Outbound Detail and Autopick

            #region Step 3: Load the list of  Order Detail by Product Code/Supplier

            var eKanbanDetails = await repository.GetEKanbanDetailForPicking(new EKanbanForPickingQueryFilter { OrderNo = orderNo });
            var l_dstEKanbanForPicking = GetEKanbanDetailForPicking(eKanbanDetails, appSettings.OwnerCode.StartsWith(UtilityService.TESA_CODE));
            if (l_dstEKanbanForPicking.None())
            {
                return new InvalidResult<string>(new JsonResultError("ImportEKanbanListEmptyError").ToJson());
            }
            #endregion

            //Reset variable value
            var intLineItem = 1;
            var intSeqNo = 1;
            var dMasterPIDQty = 0M;
            var pickedPackages = 0;
            var l_strSupplierOwnershipSplit = new List<(string, string)>();
            var l_strELXOwnershipSplit = new List<(string, string)>();
            var stockFound = false;

            //Loop to select available stock from storagedetail by productcode, customer, supplier
            foreach (var ekanbanForPickingRow in l_dstEKanbanForPicking)
            {
                PartMaster l_oPartMaster = null;
                l_oPartMaster = await repository.GetPartMasterAsync(factoryId, ekanbanForPickingRow.SupplierID, ekanbanForPickingRow.ProductCode);
                if (l_oPartMaster == null)
                {
                    return new InvalidResult<string>(new JsonResultError("ImportEKanbanUnableToRetrievePartMaster").ToJson());
                }

                dMasterPIDQty = 0;
                var dSubTotalQty = 0M;
                var dSubTotalQtyEHP = 0M;
                var intSupplierPkg = 0;
                var intEHPPkg = 0;
                var dTotalQty = 0M;
                var dUnitQty = 0M;
                var normalPartAllocation = l_oPartMaster == null || l_oPartMaster.IsCPart == 0;

                if (normalPartAllocation)
                {
                    loggerService.LogInformation($"ImportEKanbanEUCPartInternal (EKanban {orderNo}) - normal part allocation");

                    var l_dstStorageDetail = await GetStorageDetailListEuroForImport(l_oEKanbanHeader, factoryId, whsCode, ekanbanForPickingRow.ProductCode, ekanbanForPickingRow.SupplierID, false);
                    var storageRowsToUpdateResult = await GetListOfStorageThatCanBeUsedInImport(orderNo, l_dstStorageDetail, ekanbanForPickingRow);
                    if (storageRowsToUpdateResult.ResultType != ResultType.Ok)
                    {
                        return new InvalidResult<string>(storageRowsToUpdateResult.Errors.First());
                    }
                    var storageRowsToUpdate = storageRowsToUpdateResult.Data;

                    // Update StorageDetail Status, AllocatedQty, OutJobNo
                    foreach (var storagePair in storageRowsToUpdate)
                    {
                        var storage = storagePair.Key;
                        storage.Status = StorageStatus.Allocated;
                        storage.OutJobNo = "";
                        storage.AllocatedQty = storage.Qty;
                    }
                    await repository.SaveChangesAsync();

                    if (storageRowsToUpdate.Any())
                    {
                        stockFound = true;
                        dSubTotalQty = 0;
                        intSeqNo = 1;

                        //Loop to write available stock to pickinglist and update eKanban detail
                        foreach (var storageDetailRowPair in storageRowsToUpdate)
                        {
                            var storageDetail = storageDetailRowPair.Key;
                            var eKanbanDetail = storageDetailRowPair.Value;

                            #region Step 6: Insert stock info into picking list; Insert into TT_PickingListEKanban
                            await AddPickingListForImport(outbound.JobNo, intLineItem, intSeqNo, orderNo, eKanbanDetail.SerialNo, storageDetail, ekanbanForPickingRow);
                            #endregion

                            #region Step 7: Load eKanban object and update QuantitySupplied
                            eKanbanDetail.QuantitySupplied = storageDetail.Qty;

                            await repository.SaveChangesAsync();
                            #endregion

                            dTotalQty += storageDetail.Qty;
                            if (storageDetail.Ownership == (int)Ownership.Supplier)
                            {
                                dSubTotalQty += storageDetail.Qty;
                                intSupplierPkg += 1;
                                l_strSupplierOwnershipSplit.Add((intLineItem.ToString(), intSeqNo.ToString()));
                                loggerService.LogInformation($"ImportEKanbanEUCPartInternal (EKanban {orderNo}) - supplier owned stock updated for {storageDetail.PID}");
                            }
                            else
                            {
                                if (storageDetail.LocationCode == Enum.GetName(typeof(ExtSystemLocation), (int)ExtSystemLocation.RETURN))
                                {
                                    dMasterPIDQty += storageDetail.Qty;
                                }

                                dSubTotalQtyEHP += storageDetail.Qty;
                                intEHPPkg += 1;

                                l_strELXOwnershipSplit.Add((intLineItem.ToString(), intSeqNo.ToString()));
                                loggerService.LogInformation($"ImportEKanbanEUCPartInternal (EKanban {orderNo}) - EHP owned stock updated for {storageDetail.PID}");
                            }
                            intSeqNo++;
                        }
                    }
                    pickedPackages = storageRowsToUpdate.Count;
                }
                // CPart allocation
                else
                {
                    loggerService.LogInformation($"ImportEKanbanEUCPartInternal (EKanban {orderNo}) - C part allocation");

                    var l_strPID = new List<string>();

                    #region Step 5: Get ekanban detail, to update the serial no
                    int l_intMaxNo = GetMaxSerialNoForImport(eKanbanDetails, ekanbanForPickingRow.ProductCode, ekanbanForPickingRow.SupplierID);
                    #endregion

                    #region Update available stocklist from Storagedetail to Allocating
                    var l_dstStorageDetail = await GetStorageDetailListEuroForImport(l_oEKanbanHeader, factoryId, whsCode, ekanbanForPickingRow.ProductCode, ekanbanForPickingRow.SupplierID, true);

                    var storageRowsToUpdateResult = await GetListOfStorageThatCanBeUsedInImportCPart(orderNo, outbound.JobNo,
                        l_intMaxNo,
                        (l_oPartMaster?.CPartSPQ ?? 0),
                        intLineItem,
                        l_dstStorageDetail.ToList(),
                        ekanbanForPickingRow,
                        (pid) => { if (!l_strPID.Contains(pid)) l_strPID.Add(pid); });

                    if (storageRowsToUpdateResult.ResultType != ResultType.Ok)
                        return new InvalidResult<string>(storageRowsToUpdateResult.Errors.First());
                    var storageRowsToUpdate = storageRowsToUpdateResult.Data;


                    IEnumerable<StorageDetail> updatedStorageDetails = new List<StorageDetail>();
                    if (l_strPID.Any())
                    {
                        updatedStorageDetails = storageRowsToUpdate.Where(sd => l_strPID.Contains(sd.StorageDetail.PID)).Select(r => r.StorageDetail).Distinct();
                        foreach (var storage in updatedStorageDetails)
                        {
                            storage.Status = StorageStatus.Allocated;
                            storage.OutJobNo = "";
                        }
                        await repository.SaveChangesAsync();
                        l_strPID = new List<string>();
                    }

                    #endregion Update available stocklist from Storagedetail to Allocating

                    if (storageRowsToUpdate.Any())
                    {
                        stockFound = true;
                        dSubTotalQty = 0;
                        intSeqNo = 1;

                        #region Step 5: Get ekanban detail, to update the serial no
                        #endregion

                        //Loop to write available stock to pickinglist and update eKanban detail
                        foreach (var allocatedRow in storageRowsToUpdate)
                        {
                            dUnitQty = allocatedRow.PickingAllocatedPID.AllocatedQty ?? 0;
                            var storageDetail = allocatedRow.StorageDetail;
                            var eKanbanDetail = allocatedRow.EKanbanDetail;

                            #region Step 6: Insert stock info into picking list
                            #region Step 6.1: Insert into TT_PickingListEKanban
                            await AddPickingListForImport(outbound.JobNo, intLineItem, intSeqNo, orderNo, eKanbanDetail.SerialNo, storageDetail, ekanbanForPickingRow, dUnitQty);
                            #endregion
                            #endregion

                            #region Step 7: Load eKanban object and update QuantitySupplied
                            //Update the eKanban detail
                            //User CPart SPQ for CPart
                            eKanbanDetail.QuantitySupplied = dUnitQty;

                            await repository.SaveChangesAsync();

                            #endregion

                            dTotalQty += dUnitQty;

                            if (storageDetail.Ownership == (int)Ownership.Supplier)
                            {
                                dSubTotalQty += dUnitQty;
                                l_strSupplierOwnershipSplit.Add((intLineItem.ToString(), intSeqNo.ToString()));
                                loggerService.LogInformation($"ImportEKanbanEUCPartInternal (EKanban {orderNo}) - supplier owned stock updated for {storageDetail.PID}");
                            }
                            else
                            {
                                if (storageDetail.LocationCode == Enum.GetName(typeof(ExtSystemLocation), (int)ExtSystemLocation.RETURN))
                                {
                                    dMasterPIDQty += dUnitQty;
                                }
                                dSubTotalQtyEHP += dUnitQty;

                                l_strELXOwnershipSplit.Add((intLineItem.ToString(), intSeqNo.ToString()));
                                loggerService.LogInformation($"ImportEKanbanEUCPartInternal (EKanban {orderNo}) - EHP owned stock updated for {storageDetail.PID}");
                            }
                            intSeqNo++;
                        }
                    }
                    pickedPackages = storageRowsToUpdate.Count;
                }

                //If any picking is done
                if (dSubTotalQty + dSubTotalQtyEHP > 0)
                {
                    #region Step 9: Insert picking summary into Outbound Detail

                    var l_oOutboundDetail = new OutboundDetail
                    {
                        JobNo = outbound.JobNo,
                        LineItem = intLineItem,
                        Qty = dSubTotalQty + dSubTotalQtyEHP,
                        PickedQty = dSubTotalQty + dSubTotalQtyEHP,
                        ProductCode = ekanbanForPickingRow.ProductCode,
                        SupplierID = ekanbanForPickingRow.SupplierID,
                        PickedPkg = pickedPackages,
                        Pkg = pickedPackages,
                        Status = (int)OutboundDetailStatus.Picked,
                        CreatedBy = userCode
                    };

                    await AddNewOutboundDetailEU(l_oOutboundDetail, dSubTotalQty, intSupplierPkg, dSubTotalQtyEHP, intEHPPkg, dMasterPIDQty);
                    #endregion
                    intLineItem++;
                }
            }
            if (!stockFound)
            {
                return new InvalidResult<string>(new JsonResultError("NoStockFoundForEKanban").ToJson());
            }

            #region Step 11: Update Kanban header status to "imported"
            l_oEKanbanHeader.Status = (int)EKanbanStatus.Imported;
            l_oEKanbanHeader.OutJobNo = outbound.JobNo;
            #endregion


            // Split Outbound if required
            if (l_strSupplierOwnershipSplit.Any() && l_strELXOwnershipSplit.Any())
            {
                loggerService.LogInformation($"ImportEKanbanEUCPartInternal (EKanban {orderNo}) - invoke split ownership");
                await SplitOutboundWithMixedOwnership(l_strSupplierOwnershipSplit, dMasterPIDQty, jobNo, companyName, userCode);
            }

            await repository.SaveChangesAsync();

            return new SuccessResult<string>(jobNo);
        }

        private async Task SplitOutboundWithMixedOwnership(List<(string, string)> l_strSupplierOwnershipSplit, decimal dMasterPIDQty, string jobNo, string companyName, string userCode)
        {
            var l_dstPickingList = (from pl in repository.PickingLists()
                                    where pl.JobNo == jobNo
                                    select pl).ToList()
                                    .Where(pl => l_strSupplierOwnershipSplit.Contains((pl.LineItem.ToString(), pl.SeqNo.ToString())))
                                    .ToList();
            await SplitOutboundAsync(jobNo, l_dstPickingList, true, userCode, dMasterPIDQty, companyName);
        }

        private async Task SplitEhpOutboundWithMultipleSuppliers(IList<string> ehpPalletsSuppliers, decimal dMasterPIDQty, string jobNo, string userCode, string factoryID)
        {
            foreach (var supplierId in ehpPalletsSuppliers.Skip(1))
            {
                var supplierMaster = await repository.GetSupplierMasterAsync(factoryID, supplierId);
                if (supplierMaster is null) { return; }

                var pickingList = (from pl in repository.PickingLists()
                                   where pl.JobNo == jobNo && pl.SupplierID == supplierId
                                   select pl).ToList();
                await SplitOutboundAsync(jobNo, pickingList, false, userCode, dMasterPIDQty, supplierMaster.CompanyName);
            }
        }

        private async Task<Result<string>> ImportEHPCallOff(EKanbanHeader eKanbanHeader, string whsCode, string userCode)
        {
            var orderNo = eKanbanHeader.OrderNo;
            var factoryId = eKanbanHeader.FactoryID;

            // Check validity of Part Number
            var supplierMasterResult = await eKanbanService.GetSupplierMasterForEKanbanImport(orderNo, factoryId, eKanbanHeader.Instructions);
            if (supplierMasterResult.ResultType != ResultType.Ok) { return new InvalidResult<string>(supplierMasterResult.Errors.First()); }

            var companyName = supplierMasterResult.Data.Item1?.CompanyName ?? string.Empty;

            // Create Outbound Header
            var outbound = new Outbound
            {
                CustomerCode = factoryId,
                WHSCode = whsCode,
                RefNo = orderNo,
                TransType = OutboundType.EKanban,
                ETD = DateTime.Now,
                CreatedBy = userCode,
                Status = (int)OutboundStatus.NewJob,
                Remark = companyName
            };

            var createOutboundResult = await CreateOutbound(outbound);
            if (createOutboundResult.ResultType != ResultType.Ok)
            {
                return new InvalidResult<string>(new JsonResultError("OutboundJobCreateError").ToJson());
            }

            var eKanbanDetails = await repository.GetEKanbanDetailForPicking(new EKanbanForPickingQueryFilter { OrderNo = orderNo });
            var eKanbanDetailsForPicking = GetEKanbanDetailForPicking(eKanbanDetails, appSettings.OwnerCode.StartsWith(UtilityService.TESA_CODE));
            if (eKanbanDetailsForPicking.None())
            {
                return new InvalidResult<string>(new JsonResultError("ImportEKanbanListEmptyError").ToJson());
            }

            var masterPIDQty = 0M;
            var totalPalletsAllocated = new List<(StorageDetail pallet, EKanbanDetail eKanbanDetail)>();

            // Selecting pallets based on the original eKanbanDetail groups
            foreach (var eKanbanDetailGroup in eKanbanDetailsForPicking)
            {
                var applicablePallets = await GetStorageDetailListEuroForImport(eKanbanHeader, factoryId, whsCode, eKanbanDetailGroup.ProductCode, string.Empty, false);

                // Selects pallets and also adds new eKanban details if needed. The result is: one EKanbanDetail per pallet!
                var allocationResult = await GetListOfStorageThatCanBeUsedInImport(orderNo, applicablePallets, eKanbanDetailGroup);
                if (allocationResult.ResultType != ResultType.Ok)
                {
                    return new InvalidResult<string>(allocationResult.Errors.First());
                }

                foreach (var result in allocationResult.Data)
                {
                    var pallet = result.Key;
                    var eKanbanDetail = result.Value;

                    // Allocate pallets
                    pallet.Status = StorageStatus.Allocated;
                    pallet.OutJobNo = "";
                    pallet.AllocatedQty = pallet.Qty;

                    // Update eKanbanDetail
                    eKanbanDetail.QuantitySupplied = pallet.Qty;
                    eKanbanDetail.SupplierID = pallet.SupplierID;

                    if (pallet.LocationCode == Enum.GetName(typeof(ExtSystemLocation), (int)ExtSystemLocation.RETURN))
                    {
                        masterPIDQty += pallet.Qty;
                    }

                    totalPalletsAllocated.Add((pallet, eKanbanDetail));
                }

                await repository.SaveChangesAsync();
            }

            if (totalPalletsAllocated.None())
            {
                return new InvalidResult<string>(new JsonResultError("NoStockFoundForEKanban").ToJson());
            }

            // Add Outbound Details and Picking List lines based on allocated pallets
            var eKanbanDemandGroups = totalPalletsAllocated
                .GroupBy(r => new { r.eKanbanDetail.OrderNo, r.eKanbanDetail.ProductCode, r.eKanbanDetail.SupplierID, r.eKanbanDetail.DropPoint, r.eKanbanDetail.ExternalLineItem })
                .Select(g => new
                {
                    Items = g.Select(i => i).ToList(),
                    g.Key.ProductCode,
                    g.Key.SupplierID,
                    g.First().eKanbanDetail.DropPoint,
                    SumQtySupplied = g.Sum(d => d.eKanbanDetail.QuantitySupplied),
                    Packages = g.Count()
                }).ToList();

            foreach (var (item, i) in eKanbanDemandGroups.WithIndex())
            {
                var lineItem = i + 1;

                var outboundDetail = new OutboundDetail
                {
                    JobNo = outbound.JobNo,
                    LineItem = lineItem,
                    Qty = item.SumQtySupplied,
                    PickedQty = item.SumQtySupplied,
                    ProductCode = item.ProductCode,
                    SupplierID = item.SupplierID,
                    Status = (int)OutboundDetailStatus.Picked,
                    CreatedBy = userCode,
                    PickedPkg = item.Packages,
                    Pkg = item.Packages
                };
                await AddNewOutboundDetailEU(outboundDetail, 0, 0, item.SumQtySupplied, item.Packages, masterPIDQty);

                foreach (var ((pallet, eKanbanDetail), j) in item.Items.WithIndex())
                {
                    var seqNo = j + 1;

                    // only necessary data filled in
                    var details = new EKanbanDetailForPickingQueryResult()
                    {
                        DropPoint = eKanbanDetail.DropPoint,
                        ProductCode = eKanbanDetail.ProductCode
                    };
                    await AddPickingListForImport(outbound.JobNo, lineItem, seqNo, orderNo, eKanbanDetail.SerialNo, pallet, details);
                }

                await repository.SaveChangesAsync();
            }

            // Update KanbanHeader status to "imported"
            eKanbanHeader.Status = (int)EKanbanStatus.Imported;
            eKanbanHeader.OutJobNo = outbound.JobNo;

            // Split Outbound
            var palletSuppliers = totalPalletsAllocated.GroupBy(p => p.pallet.SupplierID).Select(g => g.Key).ToList();
            if (palletSuppliers.Count > 1)
            {
                await SplitEhpOutboundWithMultipleSuppliers(palletSuppliers, masterPIDQty, outbound.JobNo, userCode, factoryId);
            }

            await repository.SaveChangesAsync();

            return new SuccessResult<string>(outbound.JobNo);
        }

        private IList<EKanbanDetailForPickingQueryResult> GetEKanbanDetailForPicking(IList<EKanbanDetail> result, bool byDropPoint)
        {
            return result.GroupBy(detail => new { detail.OrderNo, detail.ProductCode, detail.SupplierID, DropPoint = byDropPoint ? detail.DropPoint : String.Empty, detail.ExternalLineItem })
                  .Select(g => new EKanbanDetailForPickingQueryResult
                  {
                      EKanbanDetails = g.ToList(),
                      OrderNo = g.Key.OrderNo,
                      ProductCode = g.Key.ProductCode,
                      SupplierID = g.Key.SupplierID,
                      DropPoint = g.First().DropPoint,
                      ExternalLineItem = g.Key.ExternalLineItem,
                      SumQty = g.Sum(d => d.Quantity),
                      SumQtySupplied = g.Sum(d => d.QuantitySupplied),
                      NoOfKanban = g.Count()
                  }).ToList();
        }

        private int GetMaxSerialNoForImport(IList<EKanbanDetail> eKanbanDetails, string productCode, string supplierId)
        {
            var maxSNQuery = eKanbanDetails.Where(e =>
                              e.ProductCode == productCode
                              && e.SupplierID == supplierId
                              && !e.SerialNo.StartsWith("A")
                              && !e.SerialNo.StartsWith("K"))
               .OrderByDescending(e => e.SerialNo)
               .Select(e => e.SerialNo).FirstOrDefault();

            return maxSNQuery != null ? int.Parse(maxSNQuery) : 0;
        }

        private async Task<Result<IDictionary<StorageDetail, EKanbanDetail>>> GetListOfStorageThatCanBeUsedInImport(string orderNo, IEnumerable<StorageDetailExtendedQueryResult> l_dstStorageDetail, EKanbanDetailForPickingQueryResult ekanbanForPickingRow)
        {
            var storageRowsToUpdate = new Dictionary<StorageDetail, EKanbanDetail>();
            decimal dAccumulateQty = ekanbanForPickingRow.SumQty;
            var rowno = -1;
            foreach (var storageRow in l_dstStorageDetail)
            {
                rowno++;
                EKanbanDetail eKanbanRow = null;

                if (rowno >= ekanbanForPickingRow.NoOfKanban)
                {
                    var addEKanbanResult = await AddEKanbanDetailForRow(orderNo, ekanbanForPickingRow, $"A{rowno + 1}", false);
                    if (addEKanbanResult.ResultType != ResultType.Ok)
                        return new InvalidResult<IDictionary<StorageDetail, EKanbanDetail>>(addEKanbanResult.Errors.First());
                    eKanbanRow = addEKanbanResult.Data;
                }
                else
                {
                    eKanbanRow = ekanbanForPickingRow.EKanbanDetails[rowno];
                }
                dAccumulateQty -= storageRow.Qty;
                storageRowsToUpdate.Add(storageRow.StorageDetail, eKanbanRow);

                if (dAccumulateQty <= 0)
                {
                    break;
                }
            }
            return new SuccessResult<IDictionary<StorageDetail, EKanbanDetail>>(storageRowsToUpdate);
        }

        private async Task<Result<IList<StorageRowUpdatedForCPart>>> GetListOfStorageThatCanBeUsedInImportCPart(string orderNo, string jobNo,
            int l_intMaxNo,
            decimal cpartSPQ,
            int intLineItem,
            IList<StorageDetailExtendedQueryResult> l_dstStorageDetail,
            EKanbanDetailForPickingQueryResult ekanbanForPickingRow,
            Action<string> addToPidList)
        {
            var storageRowsToUpdate = new List<StorageRowUpdatedForCPart>();
            decimal dAccumulateQty = ekanbanForPickingRow.SumQty;
            var intCardSerial = 0;
            decimal dAllocatedQty = 0;
            int l_intKanbanCount = 1;

            int previousLine = 0;
            int previousSerial = 0;
            int iterationIndx = -1;
            for (int rowno = 0; rowno < l_dstStorageDetail.Count; rowno++)
            {
                iterationIndx++;
                var storageRow = l_dstStorageDetail[rowno];
                var dPIDCPartAllocatedQty = storageRow.AllocatedQty;

                if (storageRow.LocationCode == Enum.GetName(typeof(ExtSystemLocation), (int)ExtSystemLocation.RETURN)
                    && storageRow.Qty - dPIDCPartAllocatedQty - dAllocatedQty < cpartSPQ)
                    continue;

                intCardSerial += 1;

                decimal dUnitQty;
                if (storageRow.Qty - dPIDCPartAllocatedQty - dAllocatedQty >= cpartSPQ)
                    dUnitQty = cpartSPQ;
                else
                    dUnitQty = storageRow.Qty - dPIDCPartAllocatedQty - dAllocatedQty;

                EKanbanDetail eKanbanRow;
                if (l_intKanbanCount > ekanbanForPickingRow.NoOfKanban || iterationIndx >= ekanbanForPickingRow.NoOfKanban)
                {
                    var addEKanbanResult = await AddEKanbanDetailForRow(orderNo, ekanbanForPickingRow, $"{l_intMaxNo + 1}", true);
                    if (addEKanbanResult.ResultType != ResultType.Ok)
                        return new InvalidResult<IList<StorageRowUpdatedForCPart>>(addEKanbanResult.Errors.First());

                    l_intMaxNo += 1;
                    eKanbanRow = addEKanbanResult.Data;
                }
                else
                {
                    eKanbanRow = ekanbanForPickingRow.EKanbanDetails[iterationIndx];
                }

                dAccumulateQty -= dUnitQty;
                dAllocatedQty += dUnitQty;

                #region Insert Allocated PID Allocated Qty into TT_PickingAllocatedQty table

                var lineIsDifferent = previousLine != intLineItem;
                var l_oPickingAllocatedPID = new PickingAllocatedPID
                {
                    JobNo = jobNo,
                    LineItem = intLineItem,
                    PID = storageRow.PID,
                    AllocatedQty = dUnitQty,
                    SerialNo = lineIsDifferent ? intCardSerial : previousSerial + 1
                };
                previousLine = intLineItem;
                previousSerial = l_oPickingAllocatedPID.SerialNo;

                await repository.AddPickingAllocatedPIDAsync(l_oPickingAllocatedPID);

                storageRowsToUpdate.Add(new StorageRowUpdatedForCPart()
                {
                    StorageDetail = storageRow.StorageDetail,
                    EKanbanDetail = eKanbanRow,
                    PickingAllocatedPID = l_oPickingAllocatedPID
                });

                #endregion

                if (dAccumulateQty <= 0)
                {
                    storageRow.StorageDetail.AllocatedQty = dPIDCPartAllocatedQty + dAllocatedQty;
                    await repository.SaveChangesAsync();
                    //dAllocatedQty = 0;
                    //intCardSerial = 0;

                    addToPidList(storageRow.StorageDetail.PID);
                    break;
                }
                else if (storageRow.LocationCode == Enum.GetName(typeof(ExtSystemLocation), (int)ExtSystemLocation.RETURN)
                    && storageRow.Qty - dPIDCPartAllocatedQty - dAllocatedQty < cpartSPQ)
                {
                    storageRow.StorageDetail.AllocatedQty = dPIDCPartAllocatedQty + dAllocatedQty;
                    await repository.SaveChangesAsync();

                    dAllocatedQty = 0;
                    intCardSerial = 0;

                    addToPidList(storageRow.StorageDetail.PID);
                    continue;
                }
                else if (storageRow.Qty > dPIDCPartAllocatedQty + dAllocatedQty)
                {
                    rowno--;
                }
                else
                {
                    storageRow.StorageDetail.AllocatedQty = dPIDCPartAllocatedQty + dAllocatedQty;
                    await repository.SaveChangesAsync();

                    dAllocatedQty = 0;
                    intCardSerial = 0;

                    addToPidList(storageRow.StorageDetail.PID);
                }
                l_intKanbanCount += 1;
            }
            return new SuccessResult<IList<StorageRowUpdatedForCPart>>(storageRowsToUpdate);
        }

        private async Task<string> GetImportedJobNo(string orderNo)
        {
            return await repository.Outbounds().Where(ob => ob.RefNo == orderNo && ob.Status != OutboundStatus.Cancelled)
                .Select(ob => ob.JobNo).FirstOrDefaultAsync();
        }

        private async Task<IList<StorageDetailExtendedQueryResult>> GetStorageDetailListEuroForImport(EKanbanHeader l_oEKanbanHeader, string factoryId, string whsCode, string productCode, string supplierId, bool isCPart)
        {
            var storageQueryFilter = new StorageDetailExtendedQueryFilter()
            {
                ProductCode = productCode,
                CustomerCode = factoryId,
                QtyGreaterThan = 0,
                OutJobNo = string.Empty,
                Statuses = isCPart ? new StorageStatus[] { StorageStatus.Putaway, StorageStatus.Allocated } : new StorageStatus[] { StorageStatus.Putaway },
                WHSCode = whsCode,
                AllocatedQtyGreaterThanZero = isCPart
            };

            if (l_oEKanbanHeader.Instructions == "SAFETYSTOCK")
                storageQueryFilter.LocationTypes = new LocationType[] { LocationType.Normal };
            else
                storageQueryFilter.LocationTypes = new LocationType[] { LocationType.Normal, LocationType.ExtSystem };

            //Instruction will be set to EHP Stock or Supplier Stock when created by interface. Otherwise blank.
            if (l_oEKanbanHeader.Instructions == "EHP")
            {
                storageQueryFilter.Ownership = Ownership.EHP;
            }
            else if (l_oEKanbanHeader.Instructions == "Supplier")
            {
                storageQueryFilter.Ownership = Ownership.Supplier;
                storageQueryFilter.SupplierId = supplierId;
            }
            else
            {
                storageQueryFilter.SupplierId = supplierId;
            }
            return await repository.GetStorageDetailListEuro(storageQueryFilter);
        }

        private async Task<Result<EKanbanDetail>> AddEKanbanDetailForRow(string orderNo, EKanbanDetailForPickingQueryResult ekanbanForPickingRow, string serialNo, bool isCPart)
        {
            if ((await repository.GetEKanbanDetailAsync(ekanbanForPickingRow.OrderNo, ekanbanForPickingRow.ProductCode, serialNo)) != null)
            {
                return new InvalidResult<EKanbanDetail>(new JsonResultError("EKanbanExists").ToJson());
            }
            var l_oEKanbanDetail = new EKanbanDetail
            {
                OrderNo = ekanbanForPickingRow.OrderNo,
                ProductCode = ekanbanForPickingRow.ProductCode,
                SerialNo = serialNo,
                SupplierID = ekanbanForPickingRow.SupplierID,
                DropPoint = ekanbanForPickingRow.DropPoint,
                Quantity = 0,
                ExternalLineItem = !isCPart ? ekanbanForPickingRow.ExternalLineItem : null
            };
            await repository.AddEKanbanDetailAsync(l_oEKanbanDetail);

            var existingEOrder = await (from eorders in repository.EOrders()
                                        where eorders.PurchaseOrderNo == orderNo
                                        && eorders.PartNo == ekanbanForPickingRow.ProductCode
                                        orderby eorders.CardSerial descending
                                        select eorders).FirstOrDefaultAsync();

            if (existingEOrder != null)
            {
                var l_oEOrders = mapper.Map<EOrder>(existingEOrder);
                l_oEOrders.OrderQuantity = "0";
                l_oEOrders.CardSerial = serialNo;
                await repository.AddEOrderAsync(l_oEOrders);
            }
            return new SuccessResult<EKanbanDetail>(l_oEKanbanDetail);
        }

        private async Task AddPickingListForImport(string jobNo, int lineItem, int seqNo, string orderNo, string serialNo, StorageDetail l_oStorageDetail, EKanbanDetailForPickingQueryResult ekanbanForPickingRow, decimal? qty = null)
        {
            var l_oPickingList = new PickingList
            {
                JobNo = jobNo,
                LineItem = lineItem,
                SeqNo = seqNo,
                ProductCode = l_oStorageDetail.ProductCode,
                SupplierID = l_oStorageDetail.SupplierID,
                LocationCode = l_oStorageDetail.LocationCode,
                InboundJobNo = l_oStorageDetail.InJobNo,
                InboundDate = l_oStorageDetail.InboundDate,
                Qty = qty ?? l_oStorageDetail.Qty,
                WHSCode = l_oStorageDetail.WHSCode,
                Version = l_oStorageDetail.Version,
                DropPoint = ekanbanForPickingRow.DropPoint,
                ProductionLine = ekanbanForPickingRow.DropPoint.Substring(1, 1),
                AllocatedPid = l_oStorageDetail.PID
            };

            await repository.AddPickingListAsync(l_oPickingList);

            #region Step 6.1: Insert into TT_PickingListEKanban
            var l_oPickingListEKanban = new PickingListEKanban
            {
                JobNo = l_oPickingList.JobNo,
                LineItem = l_oPickingList.LineItem,
                SeqNo = l_oPickingList.SeqNo,
                OrderNo = orderNo,
                SerialNo = serialNo,
                ProductCode = ekanbanForPickingRow.ProductCode
            };
            await repository.AddPickingListEKanbanAsync(l_oPickingListEKanban);
            #endregion
        }

        public async Task<Result<int>> AddNewOutboundDetail(OutboundDetailAddDto data, string userCode)
        {
            if(await iLogConnect.IsProcessingOutbound(data.JobNo))
                return new InvalidResult<int>(new JsonResultError("ILogIsProcessingThisOutbound").ToJson());

            return await WithTransactionScope<int>(async () =>
            {
                //step 1 : Get Outbound
                var outbound = await repository.GetOutboundAsync(data.JobNo);
                if (outbound == null)
                    return new InvalidResult<int>(new JsonResultError("OutboundNotFoundForJob__", "jobNo", data.JobNo).ToJson());

                if (outbound.Status == OutboundStatus.TruckDeparture)
                    return Error<int>.Get("InvalidOutboundStatusToModify");

                if (await repository.OutboundDetails().Where(o => o.JobNo == data.JobNo
                     && o.ProductCode == data.ProductCode && o.SupplierID == data.SupplierID).AnyAsync())
                {
                    return new InvalidResult<int>(new JsonResultError
                    {
                        MessageKey = "DuplicateProductForSupplier__",
                        Arguments = new Dictionary<string, string>
                        {
                            {"productCode", data.ProductCode},
                            {"supplierID", data.SupplierID}
                        }
                    }.ToJson());
                }

                //Step 2 : Create OutboundDetail
                var outboundDetail = mapper.Map<OutboundDetail>(data);

                //Step 3 : Get SeqNo for Outbound Detail
                var lineItem = utilityService.GetAutoNum(AutoNumTable.OutboundDetail, data.JobNo);
                outboundDetail.LineItem = lineItem;
                outboundDetail.CreatedBy = userCode;
                outboundDetail.CreatedDate = new DateTime();

                //Step 3.1 AU : Insert Into Outbound Detail
                await repository.AddOutboundDetailAsync(outboundDetail);

                //Step 4 : Update Allocated Qty in Inventory 
                var inventoryEHP = await repository.GetInventoryAsync(outbound.CustomerCode, outboundDetail.SupplierID, outboundDetail.ProductCode, outbound.WHSCode, Ownership.EHP);
                var inventoryQty = inventoryEHP.OnHandQty - inventoryEHP.AllocatedQty - inventoryEHP.QuarantineQty;

                if (inventoryQty >= outboundDetail.Qty)
                {
                    inventoryEHP.AllocatedQty += outboundDetail.Qty;
                }
                else
                {
                    inventoryEHP.AllocatedQty += inventoryQty;
                    var inventorySupplier = await repository.GetInventoryAsync(outbound.CustomerCode, outboundDetail.SupplierID, outboundDetail.ProductCode, outbound.WHSCode, (byte)Ownership.Supplier);
                    inventorySupplier.AllocatedQty += outboundDetail.Qty - inventoryQty;
                }
                await CheckIfInventoryQtyValid(inventoryEHP, data.JobNo);

                //else// AU: non-manual items used to have this code, but it is not used anymore 
                //{
                //    var inventorySupplier = await repository.GetInventoryAsync(outbound.CustomerCode, outboundDetail.SupplierID, outboundDetail.ProductCode, outbound.WHSCode, (byte)Ownership.Supplier);
                //    inventorySupplier.AllocatedQty += outboundDetail.Qty;

                //    if (!appSettings.OwnerCode.StartsWith(UtilityService.TESA_CODE))
                //    {
                //        var inventoryEHP = await repository.GetInventoryAsync(outbound.CustomerCode, outboundDetail.SupplierID, outboundDetail.ProductCode, outbound.WHSCode, (byte)Ownership.EHP);
                //        inventoryEHP.AllocatedQty += outboundDetail.Qty;
                //    }
                //    await CheckIfInventoryQtyValid(inventorySupplier, data.JobNo);
                //}
                await repository.SaveChangesAsync();

                return new SuccessResult<int>(lineItem);
            });
        }

        private async Task CheckIfInventoryQtyValid(Inventory inventory, string jobNo)
        {
            //Should we check QuarantineQty and DiscrepancyQty as well? 
            if (inventory.AllocatedQty > inventory.OnHandQty)
            {
                string strErrorMessage =
                    "Invalid inventory (Allocated Qty > On Hand Qty)." +
                    "ProductCode[" + inventory.ProductCode1 + "], " +
                    "SupplierID[" + inventory.SupplierID + "].";

                await loggerService.LogError(jobNo, "AddNewOutboundDetail", strErrorMessage, false);
            }
        }

        private async Task AddNewOutboundDetailEU(OutboundDetail outboundDetail, decimal p_dSupplierQty, int p_intSupplierPkg, decimal p_dEHPQty, int p_intEHPPkg, decimal p_dMasterPIDQty)
        {
            //step 1 : Get Outbound
            Outbound outbound = await repository.GetOutboundAsync(outboundDetail.JobNo);

            //Step 2 : Get SeqNo for Outbound Detail
            outboundDetail.LineItem = utilityService.GetAutoNum(AutoNumTable.OutboundDetail, outboundDetail.JobNo);

            //Step 3 : Insert Into Outbound Detail
            await repository.AddOutboundDetailAsync(outboundDetail);

            //Step 4 : Update Allocated Qty in Inventory 
            // Use Batch Update instead of Object in case there is concurrency issue

            if (p_dSupplierQty > 0)
            {
                var inventorySupplier = await repository.GetInventoryAsync(outbound.CustomerCode, outboundDetail.SupplierID, outboundDetail.ProductCode, outbound.WHSCode, (byte)Ownership.Supplier);
                inventorySupplier.AllocatedQty += p_dSupplierQty;
                inventorySupplier.AllocatedPkg += p_intSupplierPkg;

                if (inventorySupplier.AllocatedQty > inventorySupplier.OnHandQty)
                {
                    string strErrorMessage = "Invalid inventory (Allocated Qty > On Hand Qty). ProductCode[" + inventorySupplier.ProductCode1 + "], SupplierID[" + inventorySupplier.SupplierID + "].";
                    await loggerService.LogError(outboundDetail.JobNo, "AddNewOutboundDetail", strErrorMessage, false);
                }
            }

            if (p_dEHPQty - p_dMasterPIDQty > 0)
            {
                var inventoryEHP = await repository.GetInventoryAsync(outbound.CustomerCode, outboundDetail.SupplierID, outboundDetail.ProductCode, outbound.WHSCode, Ownership.EHP);
                inventoryEHP.AllocatedQty += (p_dEHPQty - p_dMasterPIDQty);
                inventoryEHP.AllocatedPkg = (inventoryEHP.AllocatedPkg ?? 0) + p_intEHPPkg;

                if (inventoryEHP.AllocatedQty > inventoryEHP.OnHandQty)
                {
                    string strErrorMessage = "Invalid inventory (Allocated Qty > On Hand Qty). ProductCode[" + inventoryEHP.ProductCode1 + "], SupplierID[" + inventoryEHP.SupplierID + "].";
                    await loggerService.LogError(outboundDetail.JobNo, "AddNewOutboundDetail", strErrorMessage, false);
                }
            }
            await repository.SaveChangesAsync();
        }

        public async Task<Result<IEnumerable<string>>> SplitOutboundByDateOrInJobNo(string jobNo, bool splitByInboundDate, string userCode)
        {
            if(await iLogConnect.IsProcessingOutbound(jobNo))
                return new InvalidResult<IEnumerable<string>>(new JsonResultError("ILogIsProcessingThisOutbound").ToJson());

            return await WithTransactionScope<IEnumerable<string>>(async () =>
            {
                var pls = await repository.GetPickingListWithUOM<PickingListSimpleDto>(jobNo, null);
                var plsGrouped = splitByInboundDate
                    ? pls.GroupBy(g => g.InboundDate.ToString()).ToList()
                    : pls.GroupBy(g => g.InboundJobNo).OrderBy(g => g.Key).ToList();

                if (plsGrouped.None())
                {
                    return new InvalidResult<IEnumerable<string>>(new JsonResultError("NoPIDInPickingList").ToJson());
                }
                if (plsGrouped.Count == 1)
                {
                    var message = splitByInboundDate ? "SplitFailedOneInboudDateOnly" : "SplitFailedOneInboudJobOnly";
                    return new InvalidResult<IEnumerable<string>>(new JsonResultError(message).ToJson());
                }

                var splitted = Enumerable.Empty<string>();
                foreach (var group in plsGrouped.Skip(1))
                {
                    var supplierName = (await repository.GetSupplierMasterAsync(group.First().CustomerCode, group.First().SupplierID)).CompanyName;
                    var dataToSplit = new SplitOutboundDto()
                    {
                        JobNo = jobNo,
                        OwnershipSplit = false,
                        PickingListItemIds = group.Select(i => new PickingListItemId { JobNo = jobNo, LineItem = i.LineItem, SeqNo = i.SeqNo })
                    };
                    var result = await SplitOutboundInternal(dataToSplit, supplierName, userCode);
                    splitted = splitted.Concat(result.Data);
                    if (result.ResultType != ResultType.Ok) { return result; }
                }
                return new SuccessResult<IEnumerable<string>>(splitted);
            });
        }

        public async Task<Result<IEnumerable<string>>> SplitOutboundByOwnership(string jobNo, string userCode)
        {
            static InvalidResult<IEnumerable<string>> invalidResult(string messageKey) => new(new JsonResultError(messageKey).ToJson());

            if(await iLogConnect.IsProcessingOutbound(jobNo))
                return invalidResult("ILogIsProcessingThisOutbound");

            return await WithTransactionScope(async () =>
            {
                var list = await repository.GetPickingListWithUOM<PickingListSimpleDto>(jobNo, null);
                var groups = list.GroupBy(i => i.Ownership).OrderBy(i => i.Key).ToList();
                if (groups.None())
                {
                    return invalidResult("NoPIDInPickingList");
                }
                if (groups.Count == 1)
                {
                    return invalidResult("SplitFailedSameOwnership");
                }
                if (groups.Count > 2)
                {
                    return invalidResult("SplitFailedInvalidOwnership");
                }

                var newList = groups.First().Select(g => g).ToList();
                var splitData = new SplitOutboundDto()
                {
                    JobNo = jobNo,
                    OwnershipSplit = true,
                    PickingListItemIds = newList.Select(i => new PickingListItemId { JobNo = jobNo, LineItem = i.LineItem, SeqNo = i.SeqNo })
                };

                var (customer, supplier) = (newList.First().CustomerCode, newList.First().SupplierID);
                var supplierName = (await repository.GetSupplierMasterAsync(customer, supplier)).CompanyName;

                return await SplitOutboundInternal(splitData, supplierName, userCode);
            });
        }

        public async Task<Result<IEnumerable<string>>> SplitOutbound(SplitOutboundDto data, string userCode)
        {
            if(await iLogConnect.IsProcessingOutbound(data.JobNo))
                return new InvalidResult<IEnumerable<string>>(new JsonResultError("ILogIsProcessingThisOutbound").ToJson());

            return await WithTransactionScope(async () => await SplitOutboundInternal(data, null, userCode));
        }

        private async Task<Result<IEnumerable<string>>> SplitOutboundInternal(SplitOutboundDto data, string supplierName, string userCode)
        {
            var pickingLists = data.PickingListItemIds
                .Distinct(new PickingListItemId.PickingListItemIdEqualityComparer())
                .Select(pl => repository.GetPickingListAsync(pl.JobNo, pl.LineItem, pl.SeqNo))
                .Select(t => t.Result)
                .Where(t => t != null)
                .OrderBy(pl => pl.JobNo).ThenBy(pl => pl.LineItem).ThenBy(pl => pl.SeqNo)
                .ToList();

            return await SplitOutboundAsync(data.JobNo, pickingLists, data.OwnershipSplit, userCode, null, supplierName);
        }

        private async Task<Result<IEnumerable<string>>> SplitOutboundAsync(string originalJobNo, IList<PickingList> pickingLists, bool ownershipSplit, string userCode, decimal? masterPIDQty, string supplierName)
        {
            loggerService.LogInformation($"SplitOutboundAsync (JobNo {originalJobNo}) - start outbound split");

            if (pickingLists.None())
            {
                loggerService.LogInformation($"SplitOutboundAsync (JobNo {originalJobNo}) - no picking lists to split - end outbound split");
                return new SuccessResult<IEnumerable<string>>(Array.Empty<string>());
            }

            // get masterPIDQty
            if (!masterPIDQty.HasValue)
                masterPIDQty = pickingLists.Where(i => i.LocationCode.ToLower() == ExtSystemLocation.RETURN.ToString().ToLower()).Sum(i => i.Qty);

            //step 1 : Get current outbound
            var outbound = await repository.GetOutboundAsync(originalJobNo);
            if (outbound.TransType == OutboundType.WHSTransfer)
            {
                loggerService.LogInformation($"SplitOutboundAsync (JobNo {originalJobNo}) - outbound of WHS Transfer type cannot be split - end outbound split");
                return new InvalidResult<IEnumerable<string>>(new JsonResultError("SplitTypeError__", "type", Enum.GetName(typeof(OutboundType), outbound.TransType)).ToJson());
            }

            var originalOrderNo = outbound.RefNo;

            if (outbound.Status > OutboundStatus.Picked)
            {
                loggerService.LogInformation($"SplitOutboundAsync (JobNo {originalJobNo}) - incorrect outbound status - end outbound split");
                return new InvalidResult<IEnumerable<string>>(new JsonResultError("SplitError__", "status", Enum.GetName(typeof(OutboundStatus), outbound.Status)).ToJson());
            }
            //Step 3 : Insert into Outbound
            var newOutbound = new Outbound
            {
                CustomerCode = outbound.CustomerCode,
                WHSCode = outbound.WHSCode,
                RefNo = outbound.RefNo,
                OSNo = outbound.OSNo,
                ETD = outbound.ETD,
                TransType = outbound.TransType,
                Charged = outbound.Charged,
                Status = outbound.Status,
                CreatedBy = userCode,
                CommInvNo = outbound.CommInvNo,
                NoOfPallet = 0,
                TransportNo = outbound.TransportNo
            };
            if (ownershipSplit)
            {
                newOutbound.Remark = $"(Supplier) Auto Split by {userCode} from {originalJobNo}" + (string.IsNullOrEmpty(supplierName) ? "" : $" - {supplierName}");
                outbound.Remark = $"(EHP) Auto Split by {userCode} from {originalJobNo}" + (string.IsNullOrEmpty(supplierName) ? "" : $" - {supplierName}");
            }
            else
            {
                newOutbound.Remark = $"Splitted from {originalJobNo}" + (string.IsNullOrEmpty(supplierName) ? "" : $" - {supplierName}");
            }

            //step 2 : Generate Outbound JobNo
            var createOutboundResult = await CreateOutbound(newOutbound);
            if (createOutboundResult.ResultType != ResultType.Ok)
            {
                loggerService.LogInformation($"SplitOutboundAsync (JobNo {originalJobNo}) - create outbound failed - {createOutboundResult.Errors?.FirstOrDefault() ?? "(no error)"} - end outbound split");
                return new InvalidResult<IEnumerable<string>>(new JsonResultError("FailToGenerateOutboundJobNo").ToJson());
            }
            var newJobNo = createOutboundResult.Data;

            //Step 4 : Update Picking List
            // we need to add new and delete existing as we are updating a part of the PrimaryKey and it is not allowed
            foreach (var pl in pickingLists)
            {
                var newpl = mapper.Map<PickingList>(pl);
                newpl.JobNo = newJobNo;
                await repository.AddPickingListAsync(newpl);
                await repository.DeletePickingListAsync(pl);
            }
            await repository.SaveChangesAsync();

            //Step 5 : Update Picking List Allocated PID
            var plAllocatedDataToUpdate = pickingLists
                .Select(pl => repository.GetPickingAllocatedPIDAsync(originalJobNo, pl.LineItem, pl.SeqNo))
                .Select(t => t.Result)
                .Where(t => t != null)
                .ToList();

            if (plAllocatedDataToUpdate.Any())
            {
                foreach (var pl in plAllocatedDataToUpdate)
                {
                    var newpl = mapper.Map<PickingAllocatedPID>(pl);
                    newpl.JobNo = newJobNo;
                    await repository.AddPickingAllocatedPIDAsync(newpl);
                    await repository.DeletePickingAllocatedPIDAsync(pl);
                }
                await repository.SaveChangesAsync();
            }

            var pickingListSummary = pickingLists.GroupBy(g => new { g.JobNo, g.LineItem, g.ProductCode })
                  .Select(g => new PickingListSummaryDto()
                  {
                      LineItem = g.Key.LineItem,
                      ProductCode = g.Key.ProductCode,
                      JobNo = g.Key.JobNo,
                      Qty = g.Sum(v => v.Qty),
                      Pkg = g.Count(),
                      PickedQty = g.Where(v => !String.IsNullOrEmpty(v.PickedBy)).Sum(v => v.Qty),
                      PickedPkg = g.Count(v => !String.IsNullOrEmpty(v.PickedBy)),
                  });

            foreach (var pickingListSummaryRow in pickingListSummary)
            {
                var outboundDetail = await repository.GetOutboundDetailAsync(originalJobNo, pickingListSummaryRow.LineItem);
                if (outboundDetail == null)
                    return new InvalidResult<IEnumerable<string>>(new JsonResultError("FailToRetrieveOutboundDetail").ToJson());

                if (outboundDetail.Qty == pickingListSummaryRow.Qty)
                {
                    await repository.DeleteOutboundDetailAsync(outboundDetail);
                }
                else
                {
                    outboundDetail.Qty -= pickingListSummaryRow.Qty;
                    outboundDetail.PickedQty -= pickingListSummaryRow.Qty;
                    outboundDetail.Pkg -= pickingListSummaryRow.Pkg;
                    outboundDetail.PickedPkg -= pickingListSummaryRow.Pkg;
                    await repository.SaveChangesAsync();
                }
                var outboundDetailClone = mapper.Map<OutboundDetail>(outboundDetail);
                outboundDetailClone.JobNo = newJobNo;
                outboundDetailClone.LineItem = pickingListSummaryRow.LineItem;
                outboundDetailClone.Qty = pickingListSummaryRow.Qty;
                outboundDetailClone.PickedQty = pickingListSummaryRow.Qty;
                outboundDetailClone.Pkg = pickingListSummaryRow.Pkg;
                outboundDetailClone.PickedPkg = pickingListSummaryRow.Pkg;
                await repository.AddOutboundDetailAsync(outboundDetailClone);
            }

            //Step 5: Get Current EKanban
            var eKANBANHeader = await repository.GetEKanbanHeaderAsync(outbound.RefNo);

            //step 6 : Generate EKanban OrderNo
            var newOrderNo = eKANBANHeader.OrderNo.IndexOf("TTK") == 0 ?
                await utilityService.GetNextOrderNo("TTK", 4) : await utilityService.GetNextOrderNo(UtilityService.MANUAL_ORDER_PREFIX, 3);

            //step 6 : Create New Ekanban
            var newEKANBANHeader = mapper.Map<EKanbanHeader>(eKANBANHeader);
            newEKANBANHeader.OrderNo = newOrderNo;
            newEKANBANHeader.OutJobNo = newJobNo;
            newEKANBANHeader.RefNo = eKANBANHeader.OrderNo;
            await repository.AddEKanbanHeaderAsync(newEKANBANHeader);


            newOutbound.RefNo = newOrderNo;
            await repository.SaveChangesAsync();

            if (outbound.TransType != OutboundType.ManualEntry
                && outbound.TransType != OutboundType.WHSTransfer
                && outbound.TransType != OutboundType.ScannerManualEntry)
            {
                //Step 7 : Update Picking List EKanban
                var plEKanbanDataToUpdate = pickingLists
                    .Select(pl => repository.GetPickingListEKanbanAsync(originalJobNo, pl.LineItem, pl.SeqNo))
                    .Select(t => t.Result)
                    .Where(t => t != null)
                    .ToList();

                foreach (var pl in plEKanbanDataToUpdate)
                {
                    var newPlEKanban = mapper.Map<PickingListEKanban>(pl);
                    newPlEKanban.JobNo = newJobNo;
                    newPlEKanban.OrderNo = newOrderNo;
                    await repository.AddPickingListEKanbanAsync(newPlEKanban);

                    var ekanbanDetailsToUpdate = repository.EKanbanDetails().Where(e =>
                        e.OrderNo == originalOrderNo
                        && e.ProductCode == pl.ProductCode
                        && e.SerialNo == pl.SerialNo).ToList();
                    foreach (var ek in ekanbanDetailsToUpdate)
                    {
                        var newEKanbanDetail = mapper.Map<EKanbanDetail>(ek);
                        newEKanbanDetail.OrderNo = newOrderNo;
                        await repository.AddEKanbanDetailAsync(newEKanbanDetail);
                        await repository.DeleteEKanbanDetailAsync(ek);
                    }

                    var eordersToUpdate = repository.EOrders().Where(e =>
                        e.PurchaseOrderNo == originalOrderNo
                        && e.PartNo == pl.ProductCode
                        && e.CardSerial == pl.SerialNo).ToList();
                    foreach (var eo in eordersToUpdate)
                    {
                        var newEorder = mapper.Map<EOrder>(eo);
                        newEorder.PurchaseOrderNo = newOrderNo;
                        await repository.AddEOrderAsync(newEorder);
                        await repository.DeleteEOrderAsync(eo);
                    }
                    await repository.DeletePickingListEKanbanAsync(pl);
                }
            }
            var pids = pickingLists.Where(pl => !string.IsNullOrEmpty(pl.PID)).Select(pl => pl.PID).Distinct().ToList();
            if (pids.Any())
            {
                var storageToUpdate = repository.StorageDetails().Where(s => pids.Contains(s.PID)).ToList();
                foreach (var s in storageToUpdate)
                {
                    s.OutJobNo = newJobNo;
                }
            }
            await repository.SaveChangesAsync();
            if (ownershipSplit)
            {
                await UpdateOutboundStatusAsync(outbound, Ownership.Supplier, 0);
                await UpdateOutboundStatusAsync(newOutbound, Ownership.Supplier, 0);
            }
            else
            {
                await UpdateOutboundStatusAsync(outbound, Ownership.EHP, masterPIDQty.Value);
                await UpdateOutboundStatusAsync(newOutbound, Ownership.Supplier, 0);
            }

            await repository.SaveChangesAsync();
            loggerService.LogInformation($"SplitOutboundAsync (JobNo {originalJobNo}) - successfully end outbound split");
            return new SuccessResult<IEnumerable<string>>(new string[] { newJobNo });
        }

        public async Task<Result<bool>> ReleaseBondedStock(OutboundDto outboundDto, string userCode)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                var jobNo = outboundDto.JobNo;

                if (!(await storageService.HasBondedStock(jobNo)))
                    return new InvalidResult<bool>(new JsonResultError("NoBondedStock").ToJson());

                if (String.IsNullOrWhiteSpace(outboundDto.CommInvNo))
                    return new InvalidResult<bool>(new JsonResultError("FailToReleaseBondedStockCommInvNoEmpty").ToJson());

                var pickingList = await repository.GetPickingLists(new string[] { jobNo });
                if (!pickingList.Any())
                    return new InvalidResult<bool>(new JsonResultError("EmptyPickingListForJobNo__", "jobNo", jobNo).ToJson());

                #region Step 1 : Get Outbound
                var outboundResult = await UpdateOutboundAsync(jobNo, outboundDto, false);
                if (outboundResult.ResultType != ResultType.Ok)
                    return new InvalidResult<bool>(outboundResult.Errors.First());

                var outbound = outboundResult.Data;
                var allowedTranTypes = new OutboundType[] { OutboundType.EKanban, OutboundType.ManualEntry, OutboundType.ScannerManualEntry, OutboundType.WHSTransfer };
                if (!allowedTranTypes.Contains(outbound.TransType))
                    return new InvalidResult<bool>(new JsonResultError("InvalidJobTypeForJobNo__", "jobNo", jobNo).ToJson());
                #endregion

                #region Step 2 : Get Picking List
                #endregion

                foreach (var plrow in pickingList)
                {
                    #region Step 2.1 : Perform Checking

                    if (String.IsNullOrEmpty(plrow.PickedBy))
                        return new InvalidResult<bool>(new JsonResultError("CompleteReturnFailProductNotScanned").ToJson());
                    #endregion

                    #region Step 2.2 : Get Storage Detail Instance
                    var storageDetail = await repository.GetStorageDetailAsync(plrow.PID);
                    if (storageDetail == null)
                        return new InvalidResult<bool>(new JsonResultError("FailToRetrieveStorageDetailForPID__", "pid", plrow.PID).ToJson());
                    #endregion

                    #region Step 2.3 : Checking for Inbound Status
                    var inbound = await repository.GetInboundAsync(storageDetail.InJobNo);
                    if (inbound == null)
                        return new InvalidResult<bool>(new JsonResultError("FailToRetrieveInboundForPID__", "pid", storageDetail.PID).ToJson());
                    if (inbound.Status != InboundStatus.Completed)
                        return new InvalidResult<bool>(new JsonResultError
                        {
                            MessageKey = "InboundJobNotCompletedForPID__",
                            Arguments = new Dictionary<string, string>
                            {
                                { "jobNo", inbound.JobNo },
                                { "pid", storageDetail.PID }
                            }
                        }.ToJson());
                    #endregion

                    #region Step 2.4 : Update Storage Detail
                    if (storageDetail.BondedStatus == (int)BondedStatus.Bonded)
                    {
                        storageDetail.BondedStatus = (int)BondedStatus.NonBonded;
                        await repository.SaveChangesAsync();

                        #region Step 2.5 : Insert TT_OutboundReleaseBondedLog
                        var outboundReleaseBondedLog = new OutboundReleaseBondedLog
                        {
                            JobNo = jobNo.Trim(),
                            PID = plrow.PID,
                            ReleasedBy = userCode
                        };
                        await repository.AddOutboundReleaseBondedLogAsync(outboundReleaseBondedLog);
                        #endregion
                    }
                    #endregion
                }
                return new SuccessResult<bool>(true);
            });
        }

        public async Task<Result<bool>> CancelAllocation(CancelAllocationDto data)
        {
            if(await iLogConnect.IsProcessingOutbound(data.JobNo))
                return new InvalidResult<bool>(new JsonResultError("ILogIsProcessingThisOutbound").ToJson());

            if (data.ItemsToCancel.None())
            {
                return new InvalidResult<bool>(new JsonResultError("CancelAllocationMustHaveItems").ToJson());
            }

            return await WithTransactionScope<bool>(async () =>
            {
                string jobNo = data.JobNo;
                int lineItem = data.LineItem;

                var isTesaCode = appSettings.OwnerCode.StartsWith(UtilityService.TESA_CODE);
                //Step 1 : Get Outbound 
                var outbound = await repository.GetOutboundAsync(jobNo);
                if (isTesaCode && outbound.Status > OutboundStatus.Packed)
                {
                    return new InvalidResult<bool>(new JsonResultError("OutboundStatusCannotCancel__", "status", Enum.GetName(typeof(OutboundStatus), outbound.Status)).ToJson());
                }
                //Step 2 : Get Outbound Detail
                var outboundDetail = await repository.GetOutboundDetailAsync(jobNo, lineItem);
                if (outboundDetail == null)
                {
                    return new InvalidResult<bool>(new JsonResultError
                    {
                        MessageKey = "OutboundDetailNotFound__",
                        Arguments = new Dictionary<string, string> { { "jobNo", jobNo }, { "lineItem", lineItem.ToString() } }
                    }.ToJson());
                }
                //Step 3 : Get Part Master
                var partMaster = await repository.GetPartMasterAsync(outbound.CustomerCode.Trim(), outboundDetail.SupplierID.Trim(), outboundDetail.ProductCode.Trim());
                //step 4 : Get Inventory  - not used until step 5 
                //var inventory = await repository.GetInventoryAsync(outbound.CustomerCode.Trim(), outboundDetail.SupplierID.Trim(), outboundDetail.ProductCode.Trim(), outbound.WHSCode.Trim(), 0);
                //Step 5 : Get EKanbanHeader
                var ekanbanHeader = await repository.GetEKanbanHeaderAsync(outbound.RefNo);

                //step 6 : Remove from picking list and revert storagedetail
                decimal dblTotal = 0;
                decimal dblTotalEHP = 0;
                var nonCPartsAndReturns = (partMaster.IsCPart == 0 || outbound.TransType == OutboundType.Return || ekanbanHeader?.Instructions == "EHP");

                #region Cancel Allocation for Normal Part & CPart
                foreach (var rowToCancel in data.ItemsToCancel)
                {
                    var pickingList = await repository.GetPickingListAsync(rowToCancel.JobNo, rowToCancel.LineItem, rowToCancel.SeqNo);

                    if (isTesaCode && nonCPartsAndReturns && !String.IsNullOrEmpty(pickingList.PickedBy) && !String.IsNullOrEmpty(pickingList.PID))
                        return new InvalidResult<bool>(new JsonResultError("ItemPickedPleaseUnpick").ToJson());

                    PickingAllocatedPID l_oPickingAllocatedPID = null;
                    if (isTesaCode && !nonCPartsAndReturns)
                    {
                        l_oPickingAllocatedPID = await repository.GetPickingAllocatedPIDAsync(pickingList.JobNo, pickingList.LineItem, pickingList.SeqNo);
                    }
                    var storageDetail = await repository.StorageDetails().Where(s =>
                                           s.Status == StorageStatus.Allocated
                                           && s.ProductCode == pickingList.ProductCode
                                           && s.SupplierID == pickingList.SupplierID
                                           && ((nonCPartsAndReturns && s.Qty == pickingList.Qty) || (!nonCPartsAndReturns && s.AllocatedQty >= pickingList.Qty))
                                           //TODO condition makes no sense; outbound job is not filled in on import, but will be in case of manual allocation
                                           && String.IsNullOrEmpty(s.OutJobNo)
                                           && (nonCPartsAndReturns || s.PID == l_oPickingAllocatedPID.PID)
                                           )
                            .OrderBy(s => s.Ownership).ThenByDescending(s => s.InboundDate)
                            .FirstOrDefaultAsync();

                    if (storageDetail == null)
                    {
                        return new InvalidResult<bool>(new JsonResultError("NoAllocatedPIDToRemove").ToJson());
                    }

                    var l_dstEKanban = await repository.EKanbanDetails().Where(k => k.OrderNo == outbound.RefNo
                                             && k.ProductCode == pickingList.ProductCode
                                             && k.SupplierID == pickingList.SupplierID
                                             && k.DropPoint == pickingList.DropPoint
                                             && k.QuantitySupplied == pickingList.Qty).ToListAsync();
                    if (l_dstEKanban.Count() == 0)
                    {
                        return new InvalidResult<bool>(new JsonResultError("Not enough supplied quantity in ekanban detail.").ToJson());
                    }

                    // To get TT_PickingListEKanban record
                    var l_oPickingListEKanban = await repository.GetPickingListEKanbanAsync(pickingList.JobNo, pickingList.LineItem, pickingList.SeqNo);
                    var l_oEKanbanDetail = await repository.GetEKanbanDetailAsync(l_oPickingListEKanban.OrderNo, l_oPickingListEKanban.ProductCode, l_oPickingListEKanban.SerialNo);

                    //Special handling for EHP stock
                    if (isTesaCode && nonCPartsAndReturns && ekanbanHeader.Instructions == "EHP")
                    {
                        l_oEKanbanDetail.SupplierID = "";
                    }

                    l_oEKanbanDetail.QuantitySupplied = 0;

                    await repository.SaveChangesAsync();

                    var l_oStorageDetail = await repository.GetStorageDetailAsync(storageDetail.PID);
                    if (nonCPartsAndReturns)
                        l_oStorageDetail.AllocatedQty = 0;
                    else
                        l_oStorageDetail.AllocatedQty -= pickingList.Qty;

                    l_oStorageDetail.OutJobNo = "";

                    if (l_oStorageDetail.AllocatedQty < 0)
                        return new InvalidResult<bool>(new JsonResultError("NegativeAllocatedQtyForPID__", "pid", l_oStorageDetail.PID).ToJson());

                    if (nonCPartsAndReturns || l_oStorageDetail.AllocatedQty == 0)
                        l_oStorageDetail.Status = StorageStatus.Putaway;

                    //Revert storagedetail
                    await repository.SaveChangesAsync();

                    //Remove from picking list
                    if (l_oStorageDetail.Ownership == (int)Ownership.Supplier)
                        dblTotal += pickingList.Qty;
                    else
                        dblTotalEHP += pickingList.Qty;

                    //Remove from TT_PickingAllocatedPID
                    if (l_oPickingAllocatedPID != null)
                        await repository.DeletePickingAllocatedPIDAsync(l_oPickingAllocatedPID);
                    await repository.DeletePickingListAsync(pickingList);
                }
                #endregion

                //step 5 : Update Inventory intransit Qty
                if (dblTotal > 0)
                {
                    var inventorySupplier = await repository.GetInventoryAsync(outbound.CustomerCode.Trim(), outboundDetail.SupplierID.Trim(), outboundDetail.ProductCode.Trim(), outbound.WHSCode.Trim(), Ownership.Supplier);
                    inventorySupplier.AllocatedQty -= dblTotal;
                }

                if (dblTotalEHP > 0)
                {
                    var inventoryEHP = await repository.GetInventoryAsync(outbound.CustomerCode.Trim(), outboundDetail.SupplierID.Trim(), outboundDetail.ProductCode.Trim(), outbound.WHSCode.Trim(), Ownership.EHP);
                    inventoryEHP.AllocatedQty -= dblTotalEHP;
                }

                //step 6 : Update Outbound Detail if qty > 0, else delete
                if (outboundDetail.Qty != (dblTotal + dblTotalEHP))
                {
                    outboundDetail.PickedQty -= (dblTotal + dblTotalEHP);
                    outboundDetail.PickedPkg -= data.ItemsToCancel.Count();
                    outboundDetail.Qty -= (dblTotal + dblTotalEHP);
                    outboundDetail.Pkg -= data.ItemsToCancel.Count();
                }
                else
                    await repository.DeleteOutboundDetailAsync(outboundDetail);

                await repository.SaveChangesAsync();

                //Step 7 : Update Outbound Status
                var pickingLists = await repository.GetPickingLists(new string[] { outboundDetail.JobNo });
                if (pickingLists.None())
                {
                    outbound.Status = (int)OutboundStatus.NewJob;
                }
                else if (!pickingLists.Any(pl => String.IsNullOrEmpty(pl.PID)))
                {
                    outbound.Status = OutboundStatus.Picked;
                }

                await repository.SaveChangesAsync();

                return new SuccessResult<bool>(true);
            });
        }

        public async Task<Result<byte[]>> GetOutboundQRCodeImage(string jobNo, string userCode)
        {
            var qrcode = await repository.GetOutboundQRCodeAsync(jobNo);
            if (qrcode == null)
            {
                var outbound = await repository.GetOutboundAsync(jobNo);
                if (outbound == null)
                    return new InvalidResult<byte[]>(new JsonResultError("CannotFindOutboundForJobNo__", "jobNo", jobNo).ToJson());

                qrcode = new OutboundQRCode
                {
                    JobNo = jobNo,
                    CreatedBy = userCode
                };

                var date = outbound.ETD.ToString("dd MMM yyyy");

                var code = "<P1>" +
                    FillString(jobNo, 15) + Convert.ToString(UtilityService.DELIMITER) +
                    FillString("0", 1) + Convert.ToString(UtilityService.DELIMITER) +
                    FillString(outbound.CustomerCode, 20) + Convert.ToString(UtilityService.DELIMITER) +
                    FillString(outbound.WHSCode, 7) + Convert.ToString(UtilityService.DELIMITER) +
                    FillString(date, 8) + Convert.ToString(UtilityService.DELIMITER);

                var qrCodeGenerator = new QRCodeGenerator();
                var qrCodeData = qrCodeGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
                var qr = new QRCode(qrCodeData);
                using (Bitmap bitMap = qr.GetGraphic(10))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                        qrcode.PickingList = ms.ToArray();
                    }
                }
                await repository.AddOutboundQRCodeAsync(qrcode);
            }
            return new SuccessResult<byte[]>(qrcode.PickingList);
        }

        public async Task<IEnumerable<OutboundPickableListDto>> GetOutboundPickableList(OutboundPickableListQueryFilter filter)
        {
            return await repository.GetOutboundPickableList<OutboundPickableListDto>(filter);
        }

        public async Task<Result<bool>> DeleteOutboundDetail(string jobNo, int lineItem)
        {
            if(await iLogConnect.IsProcessingOutbound(jobNo))
                return new InvalidResult<bool>(new JsonResultError("ILogIsProcessingThisOutbound").ToJson());

            return await WithTransactionScope<bool>(async () =>
            {
                //Step 2 : Get Outbound Detail
                var outboundDetail = await repository.GetOutboundDetailAsync(jobNo, lineItem);
                if (outboundDetail == null)
                    return new InvalidResult<bool>(new JsonResultError
                    {
                        MessageKey = "OutboundOrderItemForJobLineNotFound__",
                        Arguments = new Dictionary<string, string>
                    {
                        { "jobNo", jobNo },
                        { "lineItem", lineItem.ToString() }
                    }
                    }.ToJson());

                if (await repository.PickingLists().AnyAsync(pl => pl.JobNo == jobNo && pl.LineItem == lineItem))
                    return new InvalidResult<bool>(new JsonResultError("FailedToDeleteOutboundOrderItem").ToJson());

                //Step 1 : Get Outbound 
                var outbound = await repository.GetOutboundAsync(jobNo);
                if (outbound == null)
                    return new InvalidResult<bool>(new JsonResultError("OutboundNotFoundForJob__", "jobNo", jobNo).ToJson());

                var isManual = outbound.TransType == OutboundType.ManualEntry
                    || outbound.TransType == OutboundType.WHSTransfer
                    || outbound.TransType == OutboundType.ScannerManualEntry;

                if (isManual && outbound.Status == OutboundStatus.Completed)
                    return new InvalidResult<bool>(new JsonResultError("CannotDeleteOutboundJobCompleted").ToJson());

                if (outbound.Status == OutboundStatus.TruckDeparture)
                    return Error<bool>.Get("InvalidOutboundStatusToModify");

                //Step 3 : Get Part Master // AU not used
                //var partMaster = await repository.GetPartMasterAsync(outbound.CustomerCode, outboundDetail.SupplierID, outboundDetail.ProductCode);

                //step 4 : Get Inventory 
                var inventorySupplier = await repository.GetInventoryAsync(outbound.CustomerCode, outboundDetail.SupplierID, outboundDetail.ProductCode, outbound.WHSCode, (byte)Ownership.Supplier);

                //step 5 : Update Inventory Allocated Qty/Pkg
                if (isManual)
                {
                    if (inventorySupplier.AllocatedQty >= outboundDetail.Qty)
                    {
                        inventorySupplier.AllocatedQty -= outboundDetail.Qty;
                    }
                    else
                    {
                        var allocatedSupplierQty = inventorySupplier.AllocatedQty;
                        inventorySupplier.AllocatedQty = 0;
                        var inventoryEHP = await repository.GetInventoryAsync(outbound.CustomerCode, outboundDetail.SupplierID, outboundDetail.ProductCode, outbound.WHSCode, Ownership.EHP);
                        inventoryEHP.AllocatedQty -= (outboundDetail.Qty - allocatedSupplierQty);
                    }
                }
                else
                {
                    inventorySupplier.AllocatedQty -= outboundDetail.Qty;
                }

                //step 6 : Delete Outbound Detail
                await repository.DeleteOutboundDetailAsync(outboundDetail);

                if(!await repository.OutboundDetails().AnyAsync(od => od.JobNo == jobNo))
                {
                    outbound.Status = OutboundStatus.NewJob;
                }
                else
                {
                    var pickingLists = await repository.GetPickingLists(new string[] { outboundDetail.JobNo });
                    if(pickingLists.Any() && pickingLists.All(pl => !string.IsNullOrEmpty(pl.PID)))
                    {
                        outbound.Status = OutboundStatus.Picked;
                    }
                }

                await repository.SaveChangesAsync();

                return new SuccessResult<bool>(true);
            });
        }

        public async Task<Result<bool>> CompleteDiscrepancyOutbound(string jobNo, string name)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                var outbound = await repository.GetOutboundAsync(jobNo);
                if (outbound == null)
                    return new InvalidResult<bool>(new JsonResultError("OutboundNotFoundForJob__", "jobNo", jobNo).ToJson());

                outbound.Status = OutboundStatus.Completed;
                await repository.SaveChangesAsync();
                return new SuccessResult<bool>(true);
            });
        }

        public async Task<Result<bool>> UndoPicking(string outJobNo, IEnumerable<string> PIDs)
        {
            if(await iLogConnect.IsProcessingOutbound(outJobNo))
                return new InvalidResult<bool>(new JsonResultError("ILogIsProcessingThisOutbound").ToJson());

            return await WithTransactionScope<bool>(async () =>
            {
                var storageDetails = await repository.StorageDetails().Where(s => s.OutJobNo == outJobNo && PIDs.Contains(s.PID)).ToListAsync();
                if (storageDetails.Any(s => s.Status != StorageStatus.Picked && s.Status != StorageStatus.Packed))
                    return new InvalidResult<bool>(new JsonResultError("SelectedPIDsNotPickedOrPacked").ToJson());

                //Step 1 : Load Picking List
                var allpickingLists = await repository.GetPickingLists(new string[] { outJobNo });//, PIDs);
                foreach (var pl in allpickingLists.Where(p => PIDs.Contains(p.PID)))
                {
                    //Step 2 : Update Storage Detail
                    var storageDetail = storageDetails.SingleOrDefault(s => s.PID == pl.PID);
                    if (storageDetail == null)
                        return new InvalidResult<bool>(new JsonResultError("CannotFindPickingItemFromStorageMaster").ToJson());
                    if (storageDetail.OutJobNo != pl.JobNo)
                        return new InvalidResult<bool>(new JsonResultError
                        {
                            MessageKey = "PidNotAllocatedByOutJob__",
                            Arguments = new Dictionary<string, string>
                            {
                                { "pid", pl.PID },
                                { "jobNo", pl.JobNo }
                            }
                        }.ToJson());
                    if (storageDetail.Status >= StorageStatus.Packed)
                        return new InvalidResult<bool>(new JsonResultError
                        {
                            MessageKey = "PidInvalidStatus__",
                            Arguments = new Dictionary<string, string>
                            {
                                { "pid", pl.PID },
                                { "status", ((StorageStatus)storageDetail.Status).ToString() }
                            }
                        }.ToJson());

                    storageDetail.OutJobNo = "";
                    storageDetail.Status = StorageStatus.Allocated;

                    //step 4 : Update Picking List
                    pl.PID = "";
                    pl.PickedBy = "";
                    pl.PickedDate = null;
                    pl.PackageID = "";
                    pl.PackedBy = "";
                    pl.PackedDate = null;

                    var plAllocatedPID = await repository.GetPickingAllocatedPIDAsync(pl.JobNo, pl.LineItem, pl.SeqNo);
                    if (plAllocatedPID != null)
                        plAllocatedPID.PickedQty = 0;

                    await repository.SaveChangesAsync();
                }

                #region Update OutboundDetail (multiple)

                var outboundDetails = await repository.GetOutboundDetailPickingResultList(outJobNo);
                bool partialPicked = false;
                foreach (var od in outboundDetails)
                {
                    if (od.OutboundDetail.Qty == od.TotalPickedQty)
                    {
                        od.OutboundDetail.Status = (int)OutboundDetailStatus.Picked;
                    }
                    else
                    {
                        od.OutboundDetail.Status = (int)OutboundDetailStatus.Picking;
                        partialPicked = true;
                    }
                }
                #endregion

                #region  Update Outbound Status                    
                var outbound = await repository.GetOutboundAsync(outJobNo);
                if (partialPicked)
                    outbound.Status = OutboundStatus.PartialPicked;
                else
                    outbound.Status = OutboundStatus.NewJob;

                if (!allpickingLists.Any(pl => !String.IsNullOrEmpty(pl.PID)))
                    outbound.Status = OutboundStatus.NewJob;
                else
                    outbound.Status = OutboundStatus.PartialPicked;

                #endregion
                await repository.SaveChangesAsync();

                return new SuccessResult<bool>(true);
            });
        }

        public async Task<Result<bool>> CompleteWHSTransfer(IEnumerable<string> jobNos, string userCode)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                var allpickingLists = await repository.GetPickingLists(jobNos);

                foreach (var jobNo in jobNos)
                {
                    #region Step 1 : Get Outbound
                    var outbound = await repository.GetOutboundAsync(jobNo);
                    if (outbound.TransType != OutboundType.WHSTransfer)
                        return new InvalidResult<bool>(new JsonResultError("InvalidJobTypeForJobNo__", "jobNo", jobNo).ToJson());

                    if (appSettings.OwnerCode == OwnerCode.GE && outbound.Status != OutboundStatus.TruckDeparture)
                        return Error<bool>.Get("CannotCompleteNotDispatchedOutbound");
                    #endregion


                    #region Step 2 : Get Picking List

                    var pickingLists = allpickingLists.Where(pl => pl.JobNo == jobNo).ToList();
                    if (!pickingLists.Any())
                        return new InvalidResult<bool>(new JsonResultError("EmptyPickingListForJobNo__", "jobNo", jobNo).ToJson());
                    #endregion

                    WHSTransfer whsTransfer = null;

                    for (int i = 0; i < pickingLists.Count; i++)
                    {
                        var pickingList = pickingLists[i];
                        #region Step 2.1 : Perform Checking
                        if (String.IsNullOrEmpty(pickingList.PickedBy))
                            return new InvalidResult<bool>(new JsonResultError("CompleteReturnFailProductNotScanned").ToJson());
                        #endregion

                        #region Step 2.2 : Get Storage Detail Instance
                        var storageDetail = await repository.GetStorageDetailAsync(pickingList.PID);
                        if (storageDetail == null)
                            throw new Exception("Fail to retrive storage detail for PID " + pickingList.PID);
                        if (String.IsNullOrEmpty(storageDetail.InJobNo))
                            throw new Exception("No In Job No on storage detail " + pickingList.PID);

                        #endregion

                        #region Step 2.3 : Checking for Inbound Status
                        var inbound = await repository.GetInboundAsync(storageDetail.InJobNo);
                        if (inbound == null)
                            return new InvalidResult<bool>(new JsonResultError("FailToRetrieveInboundForPID__", "pid", storageDetail.PID).ToJson());
                        if (inbound.Status != InboundStatus.Completed)
                            return new InvalidResult<bool>(new JsonResultError
                            {
                                MessageKey = "InboundJobNotCompletedForPID__",
                                Arguments = new Dictionary<string, string>
                            {
                                { "jobNo", inbound.JobNo },
                                { "pid", storageDetail.PID }
                            }
                            }.ToJson());

                        #endregion

                        #region Step 2.4: Insert WHSTransfer
                        if (i == 0)
                        {
                            whsTransfer = new WHSTransfer
                            {
                                JobNo = outbound.JobNo,
                                CustomerCode = outbound.CustomerCode,
                                WHSCode = outbound.WHSCode,
                                NewWHSCode = outbound.NewWHSCode,
                                Remark = outbound.Remark,
                                Status = (int)WHSTransferStatus.Completed,
                                CreatedBy = userCode,
                                ConfirmedBy = userCode
                            };
                            await repository.AddWHSTransferAsync(whsTransfer);
                        }
                        #endregion

                        #region Step 2.5: Insert WHSTransferDetail
                        var whsTransferDetail = new WHSTransferDetail
                        {
                            JobNo = outbound.JobNo,
                            PID = pickingList.PID,
                            Qty = pickingList.Qty,
                            OldWHSCode = outbound.WHSCode,
                            OldLocationCode = pickingList.LocationCode,
                            NewWHSCode = outbound.NewWHSCode,
                            NewLocationCode = whsLocation_RECEIVING,
                            TransferredBy = userCode,
                            Ownership = storageDetail.Ownership
                        };
                        await repository.AddWHSTransferDetailAsync(whsTransferDetail);

                        #endregion

                        #region Step 2.6 : Update Storage Detail

                        var location = await repository.GetLocationAsync(whsLocation_RECEIVING, outbound.NewWHSCode);

                        if (location == null)
                        {
                            location = new Location
                            {
                                Code = whsLocation_RECEIVING,
                                AreaCode = "Rack",
                                WHSCode = outbound.WHSCode,
                                Name = whsLocation_RECEIVING,
                                M3 = 0.1m,
                                Status = 1,
                                Type = LocationType.Normal,
                                CreatedBy = userCode,
                                IsPriority = 0
                            };
                            await repository.AddLocationAsync(location);
                        }

                        storageDetail.PID = pickingList.PID;
                        storageDetail.Status = StorageStatus.Putaway;
                        storageDetail.AllocatedQty = 0;
                        storageDetail.OutJobNo = "";
                        storageDetail.LocationCode = location.Code;
                        storageDetail.WHSCode = outbound.NewWHSCode;
                        await repository.SaveChangesAsync();
                        #endregion

                        #region Step 2.7 : Get Storage Detail
                        // no need to get it we already have it 
                        #endregion

                        #region step 2.8a : Update Inventory (- Old WHS Inventory)
                        var oldWHSInventory = await repository.GetInventoryAsync(outbound.CustomerCode, pickingList.SupplierID, pickingList.ProductCode, outbound.WHSCode, storageDetail.Ownership);
                        if (oldWHSInventory == null)
                            return new InvalidResult<bool>(new JsonResultError("FailToRetrieveInventoryInstance").ToJson());

                        oldWHSInventory.OnHandQty -= pickingList.Qty;
                        oldWHSInventory.AllocatedQty -= pickingList.Qty;

                        oldWHSInventory.OnHandPkg -= Convert.ToInt32(Math.Ceiling(storageDetail.Qty / storageDetail.QtyPerPkg));
                        oldWHSInventory.AllocatedPkg -= Convert.ToInt32(Math.Ceiling(storageDetail.Qty / storageDetail.QtyPerPkg));

                        #endregion

                        #region step 2.8b : Update Inventory (+ New WHS Inventory)

                        var newWHSInventory = await repository.GetInventoryAsync(outbound.CustomerCode, pickingList.SupplierID, pickingList.ProductCode, outbound.NewWHSCode, storageDetail.Ownership);
                        if (newWHSInventory == null)
                            return new InvalidResult<bool>(new JsonResultError("FailToRetrieveInventoryInstance").ToJson());

                        newWHSInventory.OnHandQty += pickingList.Qty;
                        // TODO is this a mistake? we should add the packages, not subtract them
                        newWHSInventory.OnHandPkg -= Convert.ToInt32(Math.Ceiling(storageDetail.Qty / storageDetail.QtyPerPkg));

                        #endregion

                        #region Step 2.9: Insert EKanbanDetail
                        // TODO what if there is already an eKanbanDetail? 
                        var eKANBANDetail = new EKanbanDetail
                        {
                            OrderNo = outbound.RefNo,
                            ProductCode = pickingList.ProductCode,
                            SerialNo = (i + 1).ToString(),
                            SupplierID = pickingList.SupplierID,
                            Quantity = pickingList.Qty,
                            QuantitySupplied = pickingList.Qty,
                            QuantityReceived = pickingList.Qty,
                            DropPoint = "ZZ99"
                        };
                        await repository.AddEKanbanDetailAsync(eKANBANDetail);
                        #endregion

                        #region Step 2.10: Insert TT_PickingListEKanban
                        var pickingListEKanban = new PickingListEKanban
                        {
                            JobNo = outbound.JobNo,
                            LineItem = pickingList.LineItem,
                            SeqNo = pickingList.SeqNo,
                            OrderNo = outbound.RefNo,
                            SerialNo = (i + 1).ToString(),
                            ProductCode = pickingList.ProductCode
                        };
                        await repository.AddPickingListEKanbanAsync(pickingListEKanban);
                        #endregion

                        pickingList.PID = "x" + pickingList.PID;
                        await repository.SaveChangesAsync();
                    }

                    #region 2.8. Retrieve WHSTransferDetail group by product code
                    var whsTransferSummary = await repository.GetWHSTransferSummaryList(outbound.JobNo);
                    #endregion

                    foreach (var whsTransferSummaryRow in whsTransferSummary)
                    {
                        #region 2.9. Update Inventory Transaction Per WHS

                        #region 2.9.1 Update InvTransactionPerWHS for old whs

                        var lastTransaction = await repository.GetInventoryLastTransactionPerWHSBalance(outbound.CustomerCode, whsTransferSummaryRow.ProductCode, outbound.WHSCode);
                        if (!lastTransaction.HasValue)
                            return new InvalidResult<bool>(new JsonResultError("FailToRetireveInvTransactionPerWHS.").ToJson());

                        var invTransactionPerWHS = new InvTransactionPerWHS
                        {
                            JobNo = outbound.JobNo,
                            ProductCode = whsTransferSummaryRow.ProductCode,
                            WHSCode = outbound.WHSCode,
                            CustomerCode = outbound.CustomerCode,
                            JobDate = whsTransfer.CreatedDate,
                            Act = (byte)InventoryTransactionType.WHSTransferOut,
                            Qty = (double)whsTransferSummaryRow.TotalQty,
                            Pkg = whsTransferSummaryRow.TotalPkg,
                            BalanceQty = lastTransaction.Value.BalanceQty - (double)whsTransferSummaryRow.TotalQty,
                            BalancePkg = lastTransaction.Value.BalancePkg - whsTransferSummaryRow.TotalPkg
                        };

                        await repository.AddInvTransactionPerWHSAsync(invTransactionPerWHS);

                        #endregion

                        #region 2.9.2 Update InvTransactionPerWHS for new whs


                        var newInvTransactionPerWHS = new InvTransactionPerWHS
                        {
                            JobNo = outbound.JobNo,
                            ProductCode = whsTransferSummaryRow.ProductCode,
                            WHSCode = outbound.NewWHSCode,
                            CustomerCode = outbound.CustomerCode,
                            JobDate = whsTransfer.CreatedDate,
                            Act = (byte)InventoryTransactionType.WHSTransferIn,
                            Qty = (double)whsTransferSummaryRow.TotalQty,
                            Pkg = whsTransferSummaryRow.TotalPkg
                        };

                        lastTransaction = await repository.GetInventoryLastTransactionPerWHSBalance(outbound.CustomerCode, whsTransferSummaryRow.ProductCode, outbound.NewWHSCode);
                        if (!lastTransaction.HasValue)
                        {
                            newInvTransactionPerWHS.BalanceQty = newInvTransactionPerWHS.Qty;
                            newInvTransactionPerWHS.BalancePkg = newInvTransactionPerWHS.Pkg;
                        }
                        else
                        {
                            newInvTransactionPerWHS.BalanceQty = lastTransaction.Value.BalanceQty + (double)whsTransferSummaryRow.TotalQty;
                            newInvTransactionPerWHS.BalancePkg = lastTransaction.Value.BalancePkg + whsTransferSummaryRow.TotalPkg;
                        }

                        await repository.AddInvTransactionPerWHSAsync(newInvTransactionPerWHS);

                        #endregion
                        #endregion
                    }

                    #region Step 3.2 : Update Outbound Status
                    outbound.Status = OutboundStatus.Completed;
                    outbound.DispatchedBy = userCode;
                    outbound.DispatchedDate = DateTime.Now;
                    #endregion

                    #region Step 4 : Update EkanbanHeader
                    var header = await repository.GetEKanbanHeaderAsync(outbound.RefNo);
                    header.Status = (int)EKanbanStatus.Completed;
                    header.ConfirmationDate = DateTime.Now;
                    #endregion

                    await repository.SaveChangesAsync();
                }

                return new SuccessResult<bool>(true);
            });
        }

        public async Task<Stream> PickingListReport(string whsCode, string userCode, string outJobNo)
        {
            var reportName = "PickingListReportEU.rpt";
            if (!appSettings.OwnerCode.StartsWith(UtilityService.TESA_CODE))
            {
                reportName = "PickingListReport.rpt";
            }
            var title = "Picking List - " + outJobNo;
            var formula = "{TT_Outbound.JobNo} = '" + outJobNo + "'";
            var stream = await Task.Run(() => reportService.GenerateReport(reportName, whsCode, title, formula));
            await AddReportPrintingLog(userCode, ReportNames.PickingListReportEU, outJobNo);
            return stream;
        }

        public async Task<Stream> PickingInstructionReport(string whsCode, string userCode, string outJobNo)
        {
            var fileName = "PickingInstructionReportRFEuro.rpt";
            var title = "Picking Instruction - " + outJobNo;
            var formula = "{TT_Outbound.JobNo} = '" + outJobNo + "'";
            await GetOutboundQRCodeImage(outJobNo, userCode);
            var stream = await Task.Run(() => reportService.GenerateReport(fileName, whsCode, title, formula));
            await AddReportPrintingLog(userCode, ReportNames.PickingInstructionReportRFEuro, outJobNo);
            return stream;
        }

        public async Task<Result<Stream>> OutboundReport(string whsCode, string userCode, string outJobNo)
        {
            var outbound = await repository.GetOutboundAsync(outJobNo);
            if (outbound == null)
                return new NotFoundResult<Stream>(new JsonResultError("RecordNotFound").ToJson());

            var fileName = "LoadingOutboundReportEuro.rpt";
            var title = "Outbound Report - " + outJobNo;
            var formula = "{TT_Outbound.JobNo} = '" + outJobNo + "'";
            var reportStream = await Task.Run(() => reportService.GenerateReport(fileName, whsCode, title, formula));
            await AddReportPrintingLog(userCode, ReportNames.LoadingOutboundReportEuroWHS, outJobNo);
            return new SuccessResult<Stream>(reportStream);
        }

        public async Task<Stream> PackingListReport(string whsCode, string userCode, string outJobNo)
        {
            var fileName = "PackingListReport.rpt";
            var title = "Outbound Report - " + outJobNo;
            var formula = "{TT_PackingMaster.JobNo} = '" + outJobNo + "' ";
            var subreportFormula = "{TT_PackingMaster.JobNo} = '" + outJobNo + "' ";
            var stream = await Task.Run(() => reportService.GenerateReport(fileName, whsCode, title, formula, subreportFormula));
            await AddReportPrintingLog(userCode, ReportNames.PackingListReport, outJobNo);
            return stream;
        }

        public async Task<Stream> DeliveryDocketReport(string whsCode, string userCode, string outJobNo)
        {
            var fileName = "DeliveryDocketReportEuro.rpt";
            var title = "Delivery Docket - " + outJobNo;
            var formula = "{TT_Outbound.JobNo} = '" + outJobNo + "'";
            var shouldShowLocationName = appSettings.OwnerCode.In(OwnerCode.PL, OwnerCode.GE, OwnerCode.IT);
            var parameters = new JObject() { { "showLocationName", shouldShowLocationName } };

            var stream = await Task.Run(() => reportService.GenerateReport(fileName, whsCode, title, formula, null, parameters));
            await AddReportPrintingLog(userCode, ReportNames.DeliveryDocketReportEuro, outJobNo);
            return stream;
        }

        public async Task<Stream> DeliveryDocketWithPIDReport(string whsCode, string userCode, string outJobNo)
        {
            var fileName = "DeliveryDocketReportWithPIDEuro.rpt";
            var title = "Delivery Docket - " + outJobNo;
            var formula = "{TT_Outbound.JobNo} = '" + outJobNo + "'";
            var shouldShowLocationName = appSettings.OwnerCode.In(OwnerCode.PL, OwnerCode.GE, OwnerCode.IT);
            var parameters = new JObject() { { "showLocationName", shouldShowLocationName } };
            var stream = await Task.Run(() => reportService.GenerateReport(fileName, whsCode, title, formula, null, parameters));
            await AddReportPrintingLog(userCode, ReportNames.DeliveryDocketReportWithPIDEuro, outJobNo);
            return stream;
        }

        public async Task<Result<Stream>> DownloadEDTToCSV(string jobNo, string userCode)
        {
            IEnumerable<EDTDataDto> data = null;
            await repository.GetOutboundEDTDataAsync(jobNo, (r) =>
            {
                data = mapper.Map<IDataReader, IEnumerable<EDTDataDto>>(r);
            });
            await AddReportPrintingLog(userCode, ReportNames.OutboundEDTToCSV, jobNo);
            return reportService.EDTToCSV(data);
        }

        public async Task<Result<bool>> DispatchWarehouseTransfer(string jobNo, string userCode)
        {
            if (appSettings.OwnerCode != OwnerCode.GE) { return Error<bool>.Get("WHTransferDepartureIsNotAvailable"); }

            return await WithTransactionScope<bool>(async () =>
            {
                var outbound = await repository.GetOutboundAsync(jobNo);
                if (outbound == null) { return Error<bool>.Get("OutboundNotFoundForJob__", "jobNo", jobNo); }
                if (outbound.TransType != OutboundType.WHSTransfer) { return Error<bool>.Get("InvalidJobTypeForJobNo__", "jobNo", jobNo); }
                if (outbound.Status.NotIn(OutboundStatus.Picked, OutboundStatus.Packed)) { return Error<bool>.Get("InvalidOutboundStatusToDispatch"); }

                outbound.RevisedBy = userCode;
                outbound.RevisedDate = DateTime.Now;
                outbound.Status = OutboundStatus.TruckDeparture;
                await repository.SaveChangesAsync();
                return new SuccessResult<bool>(true);
            });
        }

        public AllowedOutboundCreationMethodsDto GetAllowedOutboundCreationMethods()
        {
            return appSettings.OwnerCode switch
            {
                OwnerCode.PL => new AllowedOutboundCreationMethodsDto()
                {
                    AllowEKanbanImport = true,
                    AllowManual = true
                },
                OwnerCode.GE => new AllowedOutboundCreationMethodsDto()
                {
                    AllowEKanbanImport = false,
                    AllowManual = true
                },
                OwnerCode.IT => new AllowedOutboundCreationMethodsDto()
                {
                    AllowEKanbanImport = true,
                    AllowManual = true
                },
                OwnerCode.HU => new AllowedOutboundCreationMethodsDto()
                {
                    AllowEKanbanImport = true,
                    AllowManual = true
                },
                OwnerCode.NA => new AllowedOutboundCreationMethodsDto()
                {
                    AllowEKanbanImport = true,
                    AllowManual = true
                },
                _ => new AllowedOutboundCreationMethodsDto()
                {
                    AllowEKanbanImport = true,
                    AllowManual = true
                },
            };
        }

        public async Task<List<OutboundOrderSummaryQueryResult>> GetOrderSummary(string jobNo) 
            => await repository.GetOutboundOrderSummary(jobNo);

        private string FillString(string text, int length)
        {
            if (text == null) text = "";
            return length > text.Length ? text.Trim().PadRight(length) : text.Substring(0, length);
        }

        private async Task AddReportPrintingLog(string userCode, string reportName, string jobno)
        {
            await repository.AddReportPrintingLogAsync(new ReportPrintingLog
            {
                PrintedBy = userCode,
                JobNo = jobno,
                ReportName = reportName
            });
        }

        private readonly ITTLogixRepository repository;
        private readonly AppSettings appSettings;
        private readonly IMapper mapper;
        private readonly IUtilityService utilityService;
        private readonly IEKanbanService eKanbanService;
        private readonly ILoggerService loggerService;
        private readonly IStorageService storageService;
        private readonly IBillingService billingService;
        private readonly IReportService reportService;
        private readonly IILogConnect iLogConnect;
        private readonly OutboundImportEKanbanFastService outboundImportEKanbanFastService = null;


        private readonly string whsLocation_RECEIVING = "RECEIVING";

        private class StorageRowUpdatedForCPart
        {
            public StorageDetail StorageDetail { get; set; }
            public EKanbanDetail EKanbanDetail { get; set; }
            public PickingAllocatedPID PickingAllocatedPID { get; set; }
        }
    }
}
