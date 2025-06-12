using AutoMapper;
using Microsoft.Extensions.Options;
using ServiceResult;
using System;
using System.Collections.Generic;
using System.Data;
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
    // implemented to handle just EKanban Import with performance improvements
    // not affecting other parts of the system
    public class OutboundImportEKanbanFastService
    {
        public OutboundImportEKanbanFastService(ITTLogixRepositoryForOutboundImportEKanban repository,
            IOptions<AppSettings> appSettings,
            IMapper mapper,
            IUtilityService utilityService,
            IEKanbanService eKanbanService,
            ILoggerService loggerService)
        {
            this.repository = repository;
            this.appSettings = appSettings.Value;
            this.mapper = mapper;
            this.utilityService = utilityService;
            this.eKanbanService = eKanbanService;
            this.loggerService = loggerService;
        }

        private async Task<Result<string>> CreateOutbound(Outbound entity)
        {
            //step 1 : Generate Outbound JobNo
            await repository.SaveChangesAsyncFinal();
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
                //await repository.SaveChangesAsync();
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

            //await repository.SaveChangesAsync();
        }

        public async Task<Result<string>> ImportEKanbanEUCPartInternal(EKanbanHeader l_oEKanbanHeader, string whsCode, string userCode)
        {
            repository.ChangeTrackingOff();

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

            repository.PreloadDataToLocal(
                createOutboundResult.Data,
                orderNo,
                factoryId,
                supplierMasterResult.Data.Item1.SupplierID,
                false,
                whsCode
            );

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
                    //await repository.SaveChangesAsync();

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

                            //await repository.SaveChangesAsync();
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
                        //await repository.SaveChangesAsync();
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

                            //await repository.SaveChangesAsync();

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

            await repository.SaveChangesAsyncFinal();

            // Split Outbound if required
            if (l_strSupplierOwnershipSplit.Any() && l_strELXOwnershipSplit.Any())
            {
                loggerService.LogInformation($"ImportEKanbanEUCPartInternal (EKanban {orderNo}) - invoke split ownership");
                await SplitOutboundWithMixedOwnership(l_strSupplierOwnershipSplit, dMasterPIDQty, jobNo, companyName, userCode);
            }

            await repository.SaveChangesAsyncFinal();

            repository.ChangeTrackingOn();

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
                    //await repository.SaveChangesAsync();
                    //dAllocatedQty = 0;
                    //intCardSerial = 0;

                    addToPidList(storageRow.StorageDetail.PID);
                    break;
                }
                else if (storageRow.LocationCode == Enum.GetName(typeof(ExtSystemLocation), (int)ExtSystemLocation.RETURN)
                    && storageRow.Qty - dPIDCPartAllocatedQty - dAllocatedQty < cpartSPQ)
                {
                    storageRow.StorageDetail.AllocatedQty = dPIDCPartAllocatedQty + dAllocatedQty;
                    //await repository.SaveChangesAsync();

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
                    //await repository.SaveChangesAsync();

                    dAllocatedQty = 0;
                    intCardSerial = 0;

                    addToPidList(storageRow.StorageDetail.PID);
                }
                l_intKanbanCount += 1;
            }
            return new SuccessResult<IList<StorageRowUpdatedForCPart>>(storageRowsToUpdate);
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
            if (await repository.EKanbanDetailExistsAsync(ekanbanForPickingRow.OrderNo, ekanbanForPickingRow.ProductCode, serialNo))
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

            var existingEOrder = await repository.GetFirstEOrder(orderNo, ekanbanForPickingRow.ProductCode);

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

        private async Task AddNewOutboundDetailEU(OutboundDetail outboundDetail, decimal p_dSupplierQty, int p_intSupplierPkg, decimal p_dEHPQty, int p_intEHPPkg, decimal p_dMasterPIDQty)
        {
            //step 1 : Get Outbound
            Outbound outbound = await repository.GetOutboundAsync(outboundDetail.JobNo);

            //Step 2 : Get SeqNo for Outbound Detail
            //outboundDetail.LineItem = lineItem;// utilityService.GetAutoNum(AutoNumTable.OutboundDetail, outboundDetail.JobNo);

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
            //await repository.SaveChangesAsync();
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
            //await repository.SaveChangesAsync();

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
                //await repository.SaveChangesAsync();
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
                    //await repository.SaveChangesAsync();
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
            //await repository.SaveChangesAsync();

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

                    //TODO: compile
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

                    //TODO: compile
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
                //TODO: compile
                var storageToUpdate = repository.StorageDetails().Where(s => pids.Contains(s.PID)).ToList();
                foreach (var s in storageToUpdate)
                {
                    s.OutJobNo = newJobNo;
                }
            }
            //await repository.SaveChangesAsync();
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

            //await repository.SaveChangesAsync();
            loggerService.LogInformation($"SplitOutboundAsync (JobNo {originalJobNo}) - successfully end outbound split");
            return new SuccessResult<IEnumerable<string>>(new string[] { newJobNo });
        }

        private readonly ITTLogixRepositoryForOutboundImportEKanban repository;
        private readonly AppSettings appSettings;
        private readonly IMapper mapper;
        private readonly IUtilityService utilityService;
        private readonly IEKanbanService eKanbanService;
        private readonly ILoggerService loggerService;


        private class StorageRowUpdatedForCPart
        {
            public StorageDetail StorageDetail { get; set; }
            public EKanbanDetail EKanbanDetail { get; set; }
            public PickingAllocatedPID PickingAllocatedPID { get; set; }
        }
    }
}
