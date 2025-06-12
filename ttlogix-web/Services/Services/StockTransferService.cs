using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceResult;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TT.Common;
using TT.Core.Entities;
using TT.Core.Enums;
using TT.Core.Interfaces;
using TT.Core.QueryFilters;
using TT.Core.QueryResults;
using TT.Services.Interfaces;
using TT.Services.Models;
using TT.Services.Services.Utilities;

namespace TT.Services.Services
{
    public class StockTransferService : ServiceBase<StockTransferService>, IStockTransferService
    {
        public StockTransferService(ITTLogixRepository repository,
            IUtilityService utilityService,
            IEKanbanService eKanbanService,
            IEStockTransferService eStockTransferService,
            IBillingService billingService,
            IReportService reportService,
            IOptions<AppSettings> appSettings,
            ILocker locker,
            IMapper mapper,
            ILogger<StockTransferService> logger,
            IILogConnect iLogConnect) : base(locker, logger)
        {
            this.repository = repository;
            this.utilityService = utilityService;
            this.eKanbanService = eKanbanService;
            this.eStockTransferService = eStockTransferService;
            this.billingService = billingService;
            this.reportService = reportService;
            this.mapper = mapper;
            this.iLogConnect = iLogConnect;
            this.appSettings = appSettings.Value;
        }

        public async Task<StockTransferListDto> GetStockTransferList(StockTransferListQueryFilter filter)
        {
            var query = repository.GetStockTransferList<StockTransferListItemDto>(filter);

            var pagedQuery = query.Skip(filter.PageSize * (filter.PageNo - 1)).Take(filter.PageSize);
            var total = await query.CountAsync();
            var data = await pagedQuery.ToListAsync();

            return new StockTransferListDto
            {
                Data = data,
                PageSize = filter.PageSize,
                PageNo = filter.PageNo,
                Total = total
            };
        }

        public async Task<StockTransferDto> GetStockTransfer(string jobNo)
        {
            var entity = await repository.GetStockTransferAsync(jobNo);
            return await MapStockTransferDto(entity);
        }

        public async Task<IEnumerable<StockTransferDetailDto>> GetStockTransferDetailList(string jobNo)
        {
            return await repository.GetStockTransferDetailList<StockTransferDetailDto>(jobNo);
        }

        public async Task<IEnumerable<StockTransferSummaryDto>> GetStockTransferSummaryList(string jobNo)
        {
            var orderNo = (await repository.GetStockTransferAsync(jobNo))?.RefNo;
            return await repository.GetStockTransferSummaryList<StockTransferSummaryDto>(orderNo);
        }

        public async Task<Result<StockTransferDto>> CreateStockTransfer(string customerCode, string userCode, string whsCode)
        {
            return await WithTransactionScope<StockTransferDto>(async () =>
            {
                var jobNo = await utilityService.GenerateJobNo(JobType.StockTransfer);
                if (jobNo.ResultType != ResultType.Ok)
                    return new InvalidResult<StockTransferDto>(jobNo.Errors[0]);

                var entity = new StockTransfer
                {
                    JobNo = jobNo.Data,
                    RefNo = jobNo.Data,
                    CustomerCode = customerCode,
                    WHSCode = whsCode,
                    CreatedBy = userCode,
                    TransferType = StockTransferType.Over90Days,
                    Status = StockTransferStatus.New
                };
                await repository.AddStockTransferAsync(entity);
                return new SuccessResult<StockTransferDto>(await MapStockTransferDto(entity));
            });
        }

        public async Task<Result<StockTransferDto>> UpdateStockTransfer(string jobNo, StockTransferDto stockTransferDto)
        {
            return await WithTransactionScope<StockTransferDto>(async () =>
            {
                if (jobNo != stockTransferDto.JobNo)
                    return new InvalidResult<StockTransferDto>(new JsonResultError("RecordNotFound").ToJson());

                var entity = await repository.GetStockTransferAsync(jobNo);
                if (entity == null)
                {
                    return new NotFoundResult<StockTransferDto>(new JsonResultError("RecordNotFound").ToJson());
                }
                mapper.Map(stockTransferDto, entity);
                await repository.SaveChangesAsync();

                return new SuccessResult<StockTransferDto>(await MapStockTransferDto(entity));
            });
        }

        public async Task<Result<string>> ImportEKanbanEUCPart(string orderNo, string whsCode, string userCode)
        {
            return await WithTransactionScope(async () =>
            {
                return await ImportEKanbanEUCPartInternal(orderNo, whsCode, userCode);
            });
        }

        public async Task<Result<string>> ImportEKanbanEUCPartMulti(IEnumerable<string> orderNumbers, string whsCode, string userCode)
        {
            var results = new Dictionary<string, Result<string>>();
            foreach (var orderNo in orderNumbers)
            {
                var result = await ImportEKanbanEUCPart(orderNo, whsCode, userCode);
                results.Add(orderNo, result);
            }
            if (results.Any(r => r.Value.ResultType != ResultType.Ok))
            {
                var errors = new List<string>();
                errors.Add(new JsonResultError("ImportErrorsOccurred__", "orderNos", String.Join(", ", results.Where(r => r.Value.ResultType != ResultType.Ok).Select(r => r.Key))).ToJson());

                foreach (var res in results.Where(r => r.Value.ResultType != ResultType.Ok).ToList())
                    errors.Add(res.Value.Errors[0]);
                return new ComplexInvalidResult<string>(errors);
            }
            else
            {
                return new SuccessResult<string>(string.Join(", ", results.Values.Select(r => r.Data)));
            }
        }

        public async Task<Result<string>> ImportEStockTransfer(string orderNo, string whsCode, string userCode)
        {
            return await WithTransactionScope<string>(async () =>
            {
                #region Step 0: Check Kanban header status, only "New" is accepted
                var eSTResult = await eStockTransferService.GetEStockTransferHeaderForImport(orderNo, whsCode);
                if (eSTResult.ResultType != ResultType.Ok)
                    return new InvalidResult<string>(eSTResult.Errors.First());

                var eStockTransferHeader = eSTResult.Data;

                var imported = await CheckIsImported(orderNo);
                if (imported.ResultType != ResultType.Ok)
                    return imported;
                #endregion

                #region Step 1: Check validity of Part Number
                var orderQuantities = await repository.GetEStockTransferDistinctProductCodeList<EKanbanDetailDistinctProductCodeQueryResult>(orderNo, null, null);
                if (!orderQuantities.Any())
                {
                    return new InvalidResult<string>(new JsonResultError("NoPartNoFoundForOrder__", "orderNo", orderNo).ToJson());
                }

                var factoryId = eStockTransferHeader.FactoryID;

                foreach (var row in orderQuantities)
                {
                    var partMaster = await repository.GetPartMasterAsync(factoryId, row.SupplierId, row.ProductCode);
                    if (partMaster == null)
                        return new InvalidResult<string>(new JsonResultError("ProductCodeNotFoundInPartMaster__", "productCode", row.ProductCode).ToJson());
                }
                var supplierID = orderQuantities.Last().SupplierId;
                var productCodes = orderQuantities.Select(o => o.ProductCode).ToList();
                #endregion

                #region Step 2: Create TT_StockTransfer Header
                var jobNo = await utilityService.GenerateJobNo(JobType.StockTransfer);
                if (jobNo.ResultType != ResultType.Ok)
                    return new InvalidResult<string>(jobNo.Errors[0]);

                var stockTransfer = new StockTransfer
                {
                    JobNo = jobNo.Data,
                    CustomerCode = factoryId,
                    WHSCode = whsCode,
                    RefNo = orderNo,
                    CreatedBy = userCode,
                    TransferType = StockTransferType.EStockTransfer,
                    Status = StockTransferStatus.New
                };
                await repository.AddStockTransferAsync(stockTransfer);
                #endregion

                #region Step 3: Update EStockTransfer header status to "imported"
                eStockTransferHeader.Status = EStockTransferStatus.Imported;
                eStockTransferHeader.StockTransferJobNo = stockTransfer.JobNo;
                #endregion

                #region Step 4: Insert Expired PID
                var expiredStorage = await repository.GetExpiredStorageDetails(whsCode, supplierID, factoryId, productCodes);
                var stockTranserDetails = expiredStorage.Select((s, idx) =>
                new StockTransferDetail
                {
                    JobNo = jobNo.Data,
                    LineItem = idx + 1,
                    PID = s.PID,
                    OriginalSupplierID = supplierID,
                    OriginalWHSCode = whsCode,
                    OriginalLocationCode = s.LocationCode,
                    WHSCode = whsCode,
                    LocationCode = s.LocationCode,
                    Qty = s.Qty,
                    TransferredBy = userCode
                }).ToList();
                if (stockTranserDetails.Any())
                    await repository.BatchAddStockTransferDetailAsync(stockTranserDetails);
                #endregion

                #region Step 5: Update StorageDetail PID Status
                foreach (var storage in expiredStorage)
                    storage.Status = StorageStatus.Transferring;
                #endregion

                #region Step 6: Update StockTransfer Status
                if (stockTranserDetails.Any())
                    stockTransfer.Status = StockTransferStatus.Processing;
                #endregion

                await repository.SaveChangesAsync();

                var transferredPids = expiredStorage.Select(p => p.PID);
                iLogConnect.PidAddedToStockTransfer(transferredPids);

                return new SuccessResult<string>(jobNo.Data);
            });
        }

        public async Task<Result<string>> ImportEStockTransferMulti(IEnumerable<string> orderNumbers, string whsCode, string userCode)
        {
            var results = new Dictionary<string, Result<string>>();
            foreach (var orderNo in orderNumbers)
            {
                var result = await ImportEStockTransfer(orderNo, whsCode, userCode);
                results.Add(orderNo, result);
            }
            if (results.Any(r => r.Value.ResultType != ResultType.Ok))
            {
                var errors = new List<string>
                {
                    new JsonResultError("ImportErrorsOccurred__", "orderNos", String.Join(", ", results.Where(r => r.Value.ResultType != ResultType.Ok).Select(r => r.Key))).ToJson()
                };

                foreach (var res in results.Where(r => r.Value.ResultType != ResultType.Ok).ToList())
                    errors.Add(res.Value.Errors[0]);
                return new ComplexInvalidResult<string>(errors);
            }
            else
            {
                return new SuccessResult<string>(string.Join(", ", results.Values.Select(r => r.Data)));
            }
        }

        public async Task<Result<bool>> AddStockTransferDetailByPID(StockTransferDetailByPIDDto dto, string userCode)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                var stockTransfer = await repository.GetStockTransferAsync(dto.JobNo);
                if (stockTransfer == null)
                {
                    return new NotFoundResult<bool>(new JsonResultError("RecordNotFound").ToJson());
                }

                var storageDetailsTotransfer = (await repository.StorageDetails()
                    .Where(s => dto.PIDs.Contains(s.PID) && s.Status == StorageStatus.Putaway && s.Ownership == Ownership.Supplier)
                    .ToListAsync());
                var transferredPids = storageDetailsTotransfer.Select(p => p.PID).ToList();

                if (!storageDetailsTotransfer.Any())
                    return new NotFoundResult<bool>(new JsonResultError("StorageNotFoundOrNotPutawayOrNotOwnedBySupplier").ToJson());

                var nextLineItem = utilityService.GetAutoNum(AutoNumTable.StockTransferDetail, dto.JobNo);

                var newLines = storageDetailsTotransfer.Select((storage, idx) => new StockTransferDetail
                {
                    JobNo = dto.JobNo,
                    LineItem = nextLineItem++,
                    PID = storage.PID,
                    OriginalSupplierID = storage.SupplierID,
                    OriginalWHSCode = storage.WHSCode,
                    OriginalLocationCode = storage.LocationCode,
                    WHSCode = storage.WHSCode,
                    LocationCode = storage.LocationCode,
                    Qty = storage.Qty,
                    TransferredBy = userCode
                }).ToList();
                await repository.BatchAddStockTransferDetailAsync(newLines);
                storageDetailsTotransfer.ForEach(storage => { storage.Status = StorageStatus.Transferring; });

                #region Step 6: Update StockTransfer Status
                if (await repository.StockTransferDetails().Where(s => s.JobNo == dto.JobNo).AnyAsync())
                    stockTransfer.Status = StockTransferStatus.Processing;
                #endregion

                await repository.SaveChangesAsync();

                iLogConnect.PidAddedToStockTransfer(transferredPids);

                return new SuccessResult<bool>(true);
            });
        }

        public async Task<Result<bool>> DeleteStockTransferDetailByPID(StockTransferDetailByPIDDto dto)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                var stockTransfer = await repository.GetStockTransferAsync(dto.JobNo);
                if (stockTransfer == null)
                {
                    return new NotFoundResult<bool>(new JsonResultError("RecordNotFound").ToJson());
                }
                if (new[] { StockTransferStatus.Completed, StockTransferStatus.Cancelled }.Contains(stockTransfer.Status))
                {
                    return new NotFoundResult<bool>(new JsonResultError("CannotRemovePIDFromClosedStockTransfer").ToJson());
                }

                #region Step 1 : Change PID Status in storagedetail
                var storageDetails = await repository.StorageDetails().Where(s => dto.PIDs.Contains(s.PID)).ToListAsync();
                storageDetails.ForEach(s => s.Status = StorageStatus.Putaway);
                #endregion

                #region Step 2 : Delete StockTransferDetail from Database
                var stds = await repository.StockTransferDetails().Where(s => s.JobNo == dto.JobNo && dto.PIDs.Contains(s.PID)).ToListAsync();
                await repository.BatchDeleteStockTransferDetailAsync(stds);
                #endregion

                #region Step 3b : Update Stock Transfer
                if (!await repository.StockTransferDetails().Where(s => s.JobNo == dto.JobNo).AnyAsync())
                    stockTransfer.Status = StockTransferStatus.New;
                #endregion

                await repository.SaveChangesAsync();

                iLogConnect.PidRemovedFromStockTransfer(dto.PIDs);

                return new SuccessResult<bool>(true);
            });
        }

        public async Task<Result<bool>> DeleteStockTransferDetail(string jobNo, int lineItem)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                #region Step 0 : Initialise
                var stockTransfer = await repository.GetStockTransferAsync(jobNo);
                if (stockTransfer == null)
                {
                    return new NotFoundResult<bool>(new JsonResultError("RecordNotFound").ToJson());
                }
                if (new[] { StockTransferStatus.Completed, StockTransferStatus.Cancelled }.Contains(stockTransfer.Status))
                {
                    return new NotFoundResult<bool>(new JsonResultError("CannotRemovePIDFromClosedStockTransfer").ToJson());
                }
                var stockTransferDetail = await repository.GetStockTransferDetailAsync(jobNo, lineItem);
                if (stockTransferDetail == null)
                {
                    return new NotFoundResult<bool>(new JsonResultError("RecordNotFound").ToJson());
                }
                #endregion

                #region Step 1 : Change PID Status in storagedetail
                var storageDetail = await repository.GetStorageDetailAsync(stockTransferDetail.PID);
                if (storageDetail == null)
                {
                    return new NotFoundResult<bool>(new JsonResultError("RecordNotFound").ToJson());
                }
                storageDetail.Status = StorageStatus.Putaway;
                #endregion

                #region Step 2 : Delete StockTransferDetail from Database
                await repository.DeleteStockTransferDetailAsync(stockTransferDetail);
                #endregion

                #region Step 3 : Get Stock Transfer Detail & : Update Stock Transfer
                if (!await repository.StockTransferDetails().Where(s => s.JobNo == jobNo).AnyAsync())
                    stockTransfer.Status = StockTransferStatus.New;
                #endregion

                await repository.SaveChangesAsync();

                iLogConnect.PidRemovedFromStockTransfer(new List<string> { stockTransferDetail.PID });

                return new SuccessResult<bool>(true);
            });
        }

        public async Task<Result<bool>> Cancel(string jobNo, string userCode)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                var stockTransfer = await repository.GetStockTransferAsync(jobNo);
                if (stockTransfer == null)
                {
                    return new NotFoundResult<bool>(new JsonResultError("RecordNotFound").ToJson());
                }
                if (stockTransfer.Status == StockTransferStatus.Completed)
                    return new InvalidResult<bool>(new JsonResultError("StockTransferCannotBeCancelledCompleted").ToJson());
                if (stockTransfer.Status == StockTransferStatus.Cancelled)
                    return new InvalidResult<bool>(new JsonResultError("StockTransferCannotBeCancelledCancelled").ToJson());
                if (stockTransfer.Status == StockTransferStatus.Processing)
                    return new InvalidResult<bool>(new JsonResultError("StockTransferCannotBeCancelledProcessing").ToJson());

                #region Step 1: Check for outstanding PID in Stock Transfer Detail
                if (await repository.StockTransferDetails().Where(s => s.JobNo == jobNo).AnyAsync())
                {
                    return new InvalidResult<bool>(new JsonResultError("FailToCancelStockTransferRemovePIDs", "jobNo", jobNo).ToJson());
                }
                #endregion

                stockTransfer.Status = StockTransferStatus.Cancelled;
                stockTransfer.CancelledBy = userCode;
                stockTransfer.CancelledDate = DateTime.Now;

                if (stockTransfer.TransferType == StockTransferType.EStockTransfer)
                {
                    var eStockTransferHeader = await repository.GetEStockTransferHeaderAsync(stockTransfer.RefNo);
                    if (eStockTransferHeader == null)
                    {
                        return new NotFoundResult<bool>(new JsonResultError("FailToRetrieveEStockTransferForRefNo", "refNo", stockTransfer.RefNo).ToJson());
                    }
                    eStockTransferHeader.Status = EStockTransferStatus.New;
                    eStockTransferHeader.StockTransferJobNo = string.Empty;
                }
                else if (stockTransfer.RefNo != stockTransfer.JobNo)
                {
                    var eKanbanHeader = await repository.GetEKanbanHeaderAsync(stockTransfer.RefNo);
                    if (eKanbanHeader != null && eKanbanHeader.Status == (int)EKanbanStatus.Imported)
                    {
                        eKanbanHeader.Status = (int)EKanbanStatus.New;
                        eKanbanHeader.OutJobNo = string.Empty;
                    }
                }
                await repository.SaveChangesAsync();
                return new SuccessResult<bool>(true);
            });
        }

        public async Task<Result<bool>> Complete(string jobNo, string userCode)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                var stockTransfer = await repository.GetStockTransferAsync(jobNo);
                if (stockTransfer == null)
                {
                    return new NotFoundResult<bool>(new JsonResultError("RecordNotFound").ToJson());
                }
                if (stockTransfer.Status == StockTransferStatus.New)
                    return new InvalidResult<bool>(new JsonResultError("StockTransferCannotBeCompletedDetailsEmpty").ToJson());
                if (stockTransfer.Status == StockTransferStatus.Completed)
                    return new InvalidResult<bool>(new JsonResultError("StockTransferCannotBeCompletedConfirmed").ToJson());
                if (stockTransfer.Status == StockTransferStatus.Cancelled)
                    return new InvalidResult<bool>(new JsonResultError("StockTransferCannotBeCompletedCancelled").ToJson());

                #region Step 3 : Get Stock Transfer Detail List
                var stockTransferDetail = await repository.GetStockTransferDetailList<StockTransferDetailQueryResult>(jobNo);
                #endregion

                #region Additional validation 
                var storageRowsToUpdate = new Dictionary<StorageDetail, StockTransferDetailQueryResult>();
                foreach (var strow in stockTransferDetail)
                {
                    #region Step 4.1 : Get Original Storage Detail
                    var storageDetail = await repository.GetStorageDetailAsync(strow.PID);
                    if (storageDetail == null)
                        return new NotFoundResult<bool>(new JsonResultError("FailToRetrieveStorageDetailForPID", "pid", strow.PID).ToJson());
                    #endregion

                    // if any stock transfer is in status Incoming, return error
                    if (storageDetail.Status == StorageStatus.Incoming)
                        return new InvalidResult<bool>(new JsonResultError("CannotCompleteStockTranferStorageIsInStatusIncoming", "pid", strow.PID).ToJson());

                    // check if related inbounds are completed 
                    var inbound = await repository.GetInboundAsync(storageDetail.InJobNo);
                    if (inbound != null && inbound.Status != InboundStatus.Completed)
                        return new InvalidResult<bool>(new JsonResultError("CannotCompleteStockTranferInboundNotCompleted", "jobNo", inbound.JobNo).ToJson());

                    storageRowsToUpdate.Add(storageDetail, strow);
                }
                #endregion

                var stockTransferDetailGroup = await repository.GetStockTransferDetailGroupList(jobNo);
                var stGroupItemIdx = 0;
                foreach (var stgroup in stockTransferDetailGroup)
                {
                    #region Step 2.1 : Adjust Inventory Detail(Sales Of Aged Stock)
                    var inventory = await repository.GetInventoryAsync(stgroup.CustomerCode, stgroup.SupplierID, stgroup.ProductCode, stockTransfer.WHSCode, Ownership.Supplier);
                    if (inventory == null)
                        return new NotFoundResult<bool>(new JsonResultError("RecordNotFound").ToJson());

                    inventory.OnHandQty -= stgroup.TotalQty;
                    inventory.OnHandPkg -= stgroup.TotalPkg;

                    await repository.SaveChangesAsync();
                    #endregion

                    var supplierID = appSettings.OwnerCode.StartsWith(UtilityService.TESA_CODE) ? stgroup.SupplierID : UtilityService.STR_EHP_SUPPLIER_ID;

                    #region Step 2.2 : Adjust Inventory Detail(Purchase Of Aged Stock)  
                    var inventoryEHP = await repository.GetInventoryAsync(stgroup.CustomerCode, supplierID, stgroup.ProductCode, stockTransfer.WHSCode, Ownership.EHP);
                    if (inventoryEHP == null)
                        return new NotFoundResult<bool>(new JsonResultError("RecordNotFound").ToJson());

                    inventoryEHP.OnHandQty += stgroup.TotalQty;
                    inventoryEHP.OnHandPkg += stgroup.TotalPkg;

                    await repository.SaveChangesAsync();
                    #endregion

                    #region step 2.3 : Insert into InvTransactionPerSupplier (Sales Of Aged Stock)
                    var lastTransactionPerSupplier = await repository.GetInventoryLastTransactionPerSupplierBalance(stgroup.CustomerCode, stgroup.ProductCode, stgroup.SupplierID, Ownership.Supplier);
                    var invTransactionPerSupplier = new InvTransactionPerSupplier
                    {
                        JobNo = jobNo,
                        ProductCode = stgroup.ProductCode,
                        SupplierID = stgroup.SupplierID,
                        CustomerCode = stgroup.CustomerCode,
                        Ownership = Ownership.Supplier,
                        JobDate = stockTransfer.CreatedDate,
                        Act = (int)InventoryTransactionType.SalesOfAgedStock,
                        Qty = stgroup.TotalQty,
                        BalanceQty = lastTransactionPerSupplier.HasValue ?
                        lastTransactionPerSupplier.Value - stgroup.TotalQty : stgroup.TotalQty
                    };
                    await repository.AddInvTransactionPerSupplierAsync(invTransactionPerSupplier);
                    #endregion

                    #region step 2.4 : Insert into InvTransactionPerSupplier (Purchase Of Aged Stock)

                    var lastTransactionPerSupplierEHP = await repository.GetInventoryLastTransactionPerSupplierBalance(stgroup.CustomerCode, stgroup.ProductCode, supplierID, Ownership.EHP);

                    var invTransactionPerSupplierEHP = new InvTransactionPerSupplier
                    {
                        JobNo = jobNo,
                        ProductCode = stgroup.ProductCode,
                        SupplierID = supplierID,
                        CustomerCode = stgroup.CustomerCode,
                        Ownership = Ownership.EHP,
                        JobDate = stockTransfer.CreatedDate,
                        Act = (int)InventoryTransactionType.PurchaseOfAgedStock,
                        Qty = stgroup.TotalQty,
                        BalanceQty = lastTransactionPerSupplierEHP.HasValue ?
                            lastTransactionPerSupplierEHP.Value + stgroup.TotalQty : stgroup.TotalQty
                    };
                    await repository.AddInvTransactionPerSupplierAsync(invTransactionPerSupplierEHP);
                    #endregion


                    #region step 2.3 : Insert to billing log
                    //Insert to billing log if VMI supplier
                    var supplierMaster = await repository.GetSupplierMasterAsync(stgroup.CustomerCode, stgroup.SupplierID);
                    if ((supplierMaster.SupplyParadigm.Substring(1, 1).ToUpper() == "V"))
                    {
                        await billingService.WriteToBillingLog(jobNo, stgroup.CustomerCode, stgroup.SupplierID, stgroup.ProductCode, jobNo, stgroup.TotalQty);
                    }
                    #endregion

                    stGroupItemIdx++;
                }

                #region	step 4: Perform Each PID Update

                foreach (var strow in storageRowsToUpdate)
                {
                    var storageDetail = strow.Key;
                    #region Step 4.2 : Update Storage Detail        
                    storageDetail.LocationCode = strow.Value.LocationCode;
                    storageDetail.SupplierID = appSettings.OwnerCode.StartsWith(UtilityService.TESA_CODE) ? strow.Value.SupplierID : UtilityService.STR_EHP_SUPPLIER_ID;
                    storageDetail.Ownership = Ownership.EHP;
                    storageDetail.IsVMI = 0;
                    storageDetail.Status = StorageStatus.Putaway;
                    storageDetail.BondedStatus = (byte)BondedStatus.NonBonded;
                    #endregion
                }
                #endregion

                #region Step 3 : Update Stock Transfer 
                stockTransfer.Status = StockTransferStatus.Completed;
                stockTransfer.ConfirmedBy = userCode;
                stockTransfer.ConfirmedDate = DateTime.Now;

                await repository.SaveChangesAsync();

                #endregion

                if (stockTransfer.TransferType == StockTransferType.EStockTransfer)
                {
                    var eStockTransferHeader = await repository.GetEStockTransferHeaderAsync(stockTransfer.RefNo);
                    if (eStockTransferHeader == null)
                        return new NotFoundResult<bool>(new JsonResultError("FailToRetrieveEStockTransferForRefNo", "refNo", stockTransfer.RefNo).ToJson());

                    eStockTransferHeader.Status = EStockTransferStatus.InTransit;
                    eStockTransferHeader.ConfirmationDate = DateTime.Now;

                    var stockTransferSummary = await repository.GetStockTransferSummaryList<StockTransferSummaryQueryResult>(eStockTransferHeader.OrderNo);
                    foreach (var std in stockTransferSummary)
                    {
                        var eStockTransferDetail = await repository.GetEStockTransferDetailAsync(std.OrderNo, std.ProductCode, null);
                        if (eStockTransferDetail != null)
                        {
                            eStockTransferDetail.QuantitySupplied = std.PickedQty;
                        }
                    }
                    await repository.SaveChangesAsync();
                }

                iLogConnect.StockTransferCompleted(jobNo);

                return new SuccessResult<bool>(true);
            });
        }

        public async Task<Result<bool>> SplitByInboundDate(string jobNo, string userCode)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                var stockTransfer = await repository.GetStockTransferAsync(jobNo);
                if (stockTransfer == null)
                {
                    return new NotFoundResult<bool>(new JsonResultError("RecordNotFound").ToJson());
                }
                if (!(stockTransfer.Status == StockTransferStatus.Processing || stockTransfer.Status == StockTransferStatus.New))
                {
                    return new InvalidResult<bool>(new JsonResultError("CannotSplitInvalidStatus").ToJson());
                }
                var stLinesGroupedByDate = (await repository.GetStockTransferDetailList<StockTransferDetailQueryResult>(jobNo))
                    .GroupBy(s => s.InboundDate).ToDictionary(g => g.Key, g => g.ToList());
                if (!stLinesGroupedByDate.Any() || stLinesGroupedByDate.Count == 1)
                {
                    return new InvalidResult<bool>(new JsonResultError("CannotSplitNoDetails").ToJson());
                }

                foreach (var gr in stLinesGroupedByDate)
                {
                    #region Step 2: Create StockTransfer Header
                    var newJobNo = await utilityService.GenerateJobNo(JobType.StockTransfer);
                    if (newJobNo.ResultType != ResultType.Ok)
                        return new InvalidResult<bool>(newJobNo.Errors[0]);

                    var newStocktransfer = new StockTransfer
                    {
                        JobNo = newJobNo.Data,
                        CustomerCode = stockTransfer.CustomerCode,
                        WHSCode = stockTransfer.WHSCode,
                        CreatedBy = userCode,
                        RefNo = stockTransfer.RefNo,
                        TransferType = StockTransferType.Over90Days,
                        Status = stockTransfer.Status,
                        Remark = $"{stockTransfer.Remark} Split: {DateTime.Now:dd.MM.yyyy}"
                    };
                    await repository.AddStockTransferAsync(newStocktransfer);
                    #endregion

                    #region Add detail rows 
                    await repository.BatchAddStockTransferDetailAsync(gr.Value.Select((s, idx) =>
                        new StockTransferDetail
                        {
                            JobNo = newStocktransfer.JobNo,
                            LineItem = idx + 1,
                            PID = s.PID,
                            OriginalSupplierID = s.SupplierID,
                            OriginalWHSCode = s.WHSCode,
                            OriginalLocationCode = s.LocationCode,
                            WHSCode = s.WHSCode,
                            LocationCode = s.LocationCode,
                            Qty = s.Qty,
                            TransferredBy = userCode
                        }).ToList());
                    #endregion
                }

                var rowsToDelete = await repository.StockTransferDetails().Where(s => s.JobNo == jobNo).ToListAsync();
                await repository.BatchDeleteStockTransferDetailAsync(rowsToDelete);

                stockTransfer.Status = StockTransferStatus.Cancelled;
                await repository.SaveChangesAsync();

                return new SuccessResult<bool>(true);
            });
        }

        public async Task<Stream> StockTransferReport(string whsCode, string jobNo)
        {
            var fileName = "StockTransferReportEU.rpt";
            var title = $"Stock Transfer Report - {jobNo}";
            var formula = $"{{TT_StockTransfer.JobNo}} = '{jobNo}'";
            return await Task.Run(() => reportService.GenerateReport(fileName, whsCode, title, formula));
        }

        public async Task<string> StockTransferReportFileName(string jobNo)
        {
            var stockTransfer = await repository.GetStockTransferAsync(jobNo);
            string fileName = jobNo + "_" + stockTransfer.Remark;
            return MakeValidFileName(fileName) + ".pdf";
        }

        public async Task<Result<Stream>> DownloadEDTToCSV(string jobNo)
        {
            IEnumerable<EDTDataDto> data = null;
            await repository.GetStockTransferEDTDataAsync(jobNo, (r) =>
            {
                data = mapper.Map<IDataReader, IEnumerable<EDTDataDto>>(r);
            });
            return reportService.EDTToCSV(data);
        }

        public AllowedSTFImportMethodsDto GetAllowedSTFImportMethods()
        {
            return appSettings.OwnerCode switch
            {
                OwnerCode.PL => new AllowedSTFImportMethodsDto()
                {
                    AllowEKanbanImport = true,
                    AllowEStockTransferImport = true
                },
                OwnerCode.GE => new AllowedSTFImportMethodsDto()
                {
                    AllowEKanbanImport = false,
                    AllowEStockTransferImport = true
                },
                OwnerCode.IT => new AllowedSTFImportMethodsDto()
                {
                    AllowEKanbanImport = true,
                    AllowEStockTransferImport = true
                },
                OwnerCode.HU => new AllowedSTFImportMethodsDto()
                {
                    AllowEKanbanImport = true,
                    AllowEStockTransferImport = false
                },
                OwnerCode.NA => new AllowedSTFImportMethodsDto()
                {
                    AllowEKanbanImport = true,
                    AllowEStockTransferImport = false
                },
                _ => new AllowedSTFImportMethodsDto()
                {
                    AllowEKanbanImport = false,
                    AllowEStockTransferImport = false
                },
            };
        }

        private async Task<Result<string>> CheckIsImported(string orderNo)
        {
            var importedSTJobNo = await repository.StockTransfers().Where(ob => ob.RefNo == orderNo && ob.Status != StockTransferStatus.Cancelled)
                .Select(ob => ob.JobNo).FirstOrDefaultAsync();

            if (importedSTJobNo != null)
            {
                return new InvalidResult<string>(new JsonResultError
                {
                    MessageKey = "UnableToImportOrderImported__",
                    Arguments = new Dictionary<string, string>
                    {
                        { "orderNo", orderNo },
                        {"importedJobNo", importedSTJobNo }
                    }
                }.ToJson());
            }
            return new SuccessResult<string>(null);
        }

        private async Task<Result<string>> ImportEKanbanEUCPartInternal(string orderNo, string whsCode, string userCode)
        {
            #region Step 0: Check Kanban header status, only "New" is accepted

            var eKanbanHeaderResult = await eKanbanService.GetEKanbanHeaderForImport(orderNo, whsCode);
            if (eKanbanHeaderResult.ResultType != ResultType.Ok)
                return new InvalidResult<string>(eKanbanHeaderResult.Errors.First());

            var eKanbanHeader = eKanbanHeaderResult.Data;

            var imported = await CheckIsImported(orderNo);
            if (imported.ResultType != ResultType.Ok)
                return imported;
            #endregion

            #region Step 1: Check validity of Part Number

            var factoryId = eKanbanHeader.FactoryID;

            var supplierMasterResult = await eKanbanService.GetSupplierMasterForEKanbanImport(orderNo, factoryId, eKanbanHeader.Instructions);
            if (supplierMasterResult.ResultType != ResultType.Ok)
                return new InvalidResult<string>(supplierMasterResult.Errors.First());

            var supplierMaster = supplierMasterResult.Data.Item1;
            var eKanbanDetails = supplierMasterResult.Data.Item2;

            #endregion

            #region Step 2: Create StockTransfer Header

            var jobNo = await utilityService.GenerateJobNo(JobType.StockTransfer);
            if (jobNo.ResultType != ResultType.Ok)
                return new InvalidResult<string>(jobNo.Errors[0]);

            var stockTransfer = new StockTransfer
            {
                JobNo = jobNo.Data,
                CustomerCode = factoryId,
                WHSCode = whsCode,
                RefNo = orderNo,
                CreatedBy = userCode,
                TransferType = StockTransferType.Over90Days,
                Status = StockTransferStatus.New,
                Remark = $"{supplierMaster.CompanyName} {DateTime.Now:dd.MM.yyyy}"
            };
            await repository.AddStockTransferAsync(stockTransfer);

            #endregion

            #region Step 3: Load the list of  Order Detail by Product Code/Supplier
            var transferredPids = new List<string>();
            int intLine = 0;
            foreach (var row in eKanbanDetails)
            {
                var dAccumulateQty = row.SumQty;

                var storageDetailRows = await repository.StorageDetails().Where(s =>
                    s.CustomerCode == factoryId &&
                    s.Status == StorageStatus.Putaway &&
                    s.WHSCode == whsCode &&
                    s.SupplierID == row.SupplierId &&
                    s.Ownership == (eKanbanHeader.Instructions != "EHP" ? Ownership.Supplier : Ownership.EHP) &&
                    s.ProductCode == row.ProductCode)
                    .OrderBy(s => s.InboundDate)
                    .ToListAsync();

                foreach (var storage in storageDetailRows)
                {
                    intLine++;
                    var stockTransferDetail = new StockTransferDetail
                    {
                        PID = storage.PID,
                        JobNo = stockTransfer.JobNo,
                        LineItem = intLine,
                        OriginalSupplierID = storage.SupplierID,
                        OriginalWHSCode = storage.WHSCode,
                        OriginalLocationCode = storage.LocationCode,
                        WHSCode = storage.WHSCode,
                        LocationCode = storage.LocationCode,
                        Qty = storage.Qty,
                        TransferredBy = userCode
                    };
                    await repository.AddStockTransferDetailAsync(stockTransferDetail);

                    #region Step 5: Update StorageDetail PID Status
                    storage.Status = StorageStatus.Transferring;
                    transferredPids.Add(storage.PID);
                    #endregion

                    dAccumulateQty -= storage.Qty;
                    if (dAccumulateQty <= 0)
                        break;
                }
            }
            #endregion

            #region Step 6: Update StockTransfer Status
            if (await repository.StockTransferDetails().Where(s => s.JobNo == jobNo.Data).AnyAsync())
                stockTransfer.Status = StockTransferStatus.Processing;
            #endregion

            #region Step 11: Update Kanban header status to "imported"

            eKanbanHeader.Status = (int)EKanbanStatus.Imported;
            eKanbanHeader.OutJobNo = stockTransfer.JobNo;

            #endregion

            await repository.SaveChangesAsync();

            iLogConnect.PidAddedToStockTransfer(transferredPids);

            return new SuccessResult<string>(jobNo.Data);
        }

        private async Task<StockTransferDto> MapStockTransferDto(StockTransfer entity)
        {
            if (entity != null)
            {
                var additionalData = await repository.GetStockTransferTotalValueQueryResult(entity.JobNo);
                return mapper.Map<StockTransferDto>(entity, opts =>
                {
                    opts.AfterMap((src, dest) =>
                    {
                        dest.OutboundTotalValue = additionalData.OutboundTotalValue;
                        dest.Currency = additionalData.Currency;
                        dest.IsMixedCurrency = additionalData.MixedCurrency;
                    });
                });
            }
            return null;
        }

        private static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }

        private readonly ITTLogixRepository repository;
        private readonly IUtilityService utilityService;
        private readonly IEKanbanService eKanbanService;
        private readonly IEStockTransferService eStockTransferService;
        private readonly IBillingService billingService;
        private readonly IReportService reportService;
        private readonly IMapper mapper;
        private readonly IILogConnect iLogConnect;
        private readonly AppSettings appSettings;
    }
}
