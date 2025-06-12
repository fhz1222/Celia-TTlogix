using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class PickingListService : ServiceBase<PickingListService>, IPickingListService
    {
        public PickingListService(ITTLogixRepository repository,
            IOutboundService outboundService,
            IUtilityService utilityService,
            IILogConnect iLogConnect,
            IOptions<AppSettings> appSettings,
            IMapper mapper,
            ILocker locker,
            ILogger<PickingListService> logger) : base(locker, logger)
        {
            this.repository = repository;
            this.outboundService = outboundService;
            this.utilityService = utilityService;
            this.iLogConnect = iLogConnect;
            this.appSettings = appSettings.Value;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<PickingListSimpleDto>> GetPickingListWithUOM(string jobNo, int? lineItem)
        {
            return await repository.GetPickingListWithUOM<PickingListSimpleDto>(jobNo, lineItem);
        }

        //public async Task<IEnumerable<PickingListSummaryDto>> GetPickingListSummary(string jobNo)
        //{
        //    return await (from pl in repository.PickingLists()
        //                  where pl.JobNo == jobNo
        //                  group pl by new { pl.JobNo, pl.LineItem, pl.ProductCode }
        //                            into g
        //                  select new PickingListSummaryDto()
        //                  {
        //                      LineItem = g.Key.LineItem,
        //                      ProductCode = g.Key.ProductCode,
        //                      JobNo = g.Key.JobNo,
        //                      Qty = g.Sum(v => v.Qty),
        //                      Pkg = g.Count(),
        //                      PickedQty = g.Where(v => !String.IsNullOrEmpty(v.PickedBy)).Sum(v => v.Qty),
        //                      PickedPkg = g.Where(v => !String.IsNullOrEmpty(v.PickedBy)).Count(),
        //                  }).ToListAsync();
        //}


        public async Task<IList<string>> GetPickingDataToDownload(PickingListToDownloadQueryFilter queryFilter)
        {
            var resultData = new List<string>();
            var pickingListQueryResult = await repository.GetPickingListDataToDownload(queryFilter);
            StringBuilder sbHeaderInfo = new StringBuilder();

            // Generate the header
            // OK, after we retrieve the data, we can generate the header information
            // Columns	Item Description
            // 1) 01-12	Filename								(12 char
            // 2) 13-17	Number of rows of data		(05 char
            // 3) 18-19	Total number of fields, m		(02 char
            // 4) 20-21	Length of datafield 1			(02 char
            // 5) 22-23	Length of datafield ...m		(02 char
            sbHeaderInfo.Append("TPICK.DAT   ");	// FileName
            sbHeaderInfo.Append(pickingListQueryResult.Count.ToString("00000")); // Number of rows of data
            sbHeaderInfo.Append("16");	// that's right, 16 fields (MAX = 16 fields)
            sbHeaderInfo.Append("15");	// 01) Outbound Job No
            sbHeaderInfo.Append("20");	// 02) PID
            sbHeaderInfo.Append("30");	// 03) Product Code 1
            sbHeaderInfo.Append("30");	// 04) Product Code 2
            sbHeaderInfo.Append("15");	// 05) Location Code
            sbHeaderInfo.Append("07");	// 06) Warehouse Code
            sbHeaderInfo.Append("10");	// 07) No of Packages changed to SupplierID
            sbHeaderInfo.Append("10");	// 08) SPQ
            sbHeaderInfo.Append("10");	// 09) Qty to Pick
            sbHeaderInfo.Append("14");	// 10) Total Picking Qty
            sbHeaderInfo.Append("04");	// 11) Version 'Revised by boon'
            sbHeaderInfo.Append("08");	// 12) FIFO In Date
            sbHeaderInfo.Append("26");	// 13) Control Code Name
            sbHeaderInfo.Append("25");	// 14) Control Code Value
            sbHeaderInfo.Append("08");	// 15) Control Date
            sbHeaderInfo.Append("01");	// 17) Flag / Status of the Picking 1 = Partial, 2 = Complete

            // Create the data string array for scanner, then dump picking data into the first array item
            resultData.Add(sbHeaderInfo.ToString());

            int rowNumber = 1;
            foreach (var row in pickingListQueryResult)
            {
                // ok now, we can begin to insert the picking list
                string str = "";
                str += rowNumber.ToString("00000"); // ? do  we still need this doi
                str += FillString(row.JobNo, 15);					// Outbound Job No
                str += FillString(row.PID, 20);						// PID
                str += FillString(row.ProductCode1, 30);	// ProductCode1
                str += FillString(row.ExternalID, 30);	// ProductCode2
                str += FillString(row.LocationCode, 15);		// Location Code
                str += FillString(row.WHSCode, 07);			// WHS Code
                str += FillString(row.SupplierID, 10);			// Supplier ID
                // if SPQ
                if (row.IsStandardPackaging == 1)
                {
                    //str += FillString("1", 10);																											// No of Packages

                    string strFormat = new String('0', row.DecimalNum);
                    strFormat = row.DecimalNum == 0 ? "#" : String.Concat("######0.", strFormat);
                    //? = String.Format(String.Concat("{0:" ,strFormat ,"}"),Convert.ToDouble(?));

                    str += FillString(row.SPQ.ToString(strFormat), 10);					// SPQ
                    str += FillString(row.Qty.ToString(strFormat), 10);//Picking Qty
                    str += FillString(row.TotalPickQty.ToString(strFormat), 14);//Total Picking Qty
                    str += FillString(row.Version.ToString(), 4);//version
                    str += FillString(row.InboundDate.ToString("ddMMyyyy"), 8);//InDate	
                    str += FillString(row.ControlCodeType.ToString().Trim() + row.ControlCodeName?.Trim(), 26);//Control Code Name
                    str += FillString(row.ControlCodeValue.ToString(), 25);//Control Code Value

                    if (row.ControlDate.HasValue) //Control Date
                        str += FillString(row.ControlDate.Value.ToString("ddMMyyyy"), 8);//Control Date
                    else
                        str += FillString("", 8);

                    str += FillString("0", 1); // Flag

                }
                else // for Non-SPQ
                {
                    //str += FillString(l_odsPickingList.Tables[0].Rows[index]["JobNo , 10);					// No of Packages // count none spq
                    str += FillString(row.SPQ.ToString(), 10);					// SPQ
                    str += FillString(row.Pkg.ToString(), 25);						//Total Qty to Pick // = no Of packages
                    str += FillString(row.PickedPkg.ToString(), 25);			// Qty Picked
                }

                // add into string array
                resultData.Add(str);
                rowNumber++;
            }
            return resultData;
        }

        private string FillString(string p_str, int intWidth)
        {
            return p_str?.PadRight(intWidth, ' ');
        }

        public async Task<bool> HasPickingLists(string jobNo, int lineItem)
        {
            return await repository.PickingLists().AnyAsync(pl => pl.JobNo == jobNo && pl.LineItem == lineItem);
        }

        public async Task<Result<bool>> AutoAllocate(AllocationDto dto)
        {
            if(await iLogConnect.IsProcessingOutbound(dto.JobNo))
                return new InvalidResult<bool>(new JsonResultError("ILogIsProcessingThisOutbound").ToJson());

            return await WithTransactionScopeAndLock(async () =>
            {
                var outbound = await repository.GetOutboundAsync(dto.JobNo);

                if (outbound.TransType == OutboundType.EKanban)
                {
                    return new InvalidResult<bool>(new JsonResultError("AutoallocationNotAllowed").ToJson());
                }

                var outboundDetail = await repository.GetOutboundDetailAsync(dto.JobNo, dto.LineItem);

                if (outboundDetail == null)
                    return new InvalidResult<bool>(new JsonResultError("PleaseAddPartForDelivery").ToJson());

                var pm = await repository.GetPartMasterAsync(outbound.CustomerCode, outboundDetail.SupplierID, outboundDetail.ProductCode);

                // existing picking lists for this outbound detail
                var p_odsPickingList = await repository.PickingLists().Where(pl =>
                                pl.JobNo == dto.JobNo
                                && pl.LineItem == dto.LineItem).ToListAsync();
                //if (!l_oPickingListController.GetPickingListWithUOM(ref l_oFilter, ref m_odsPickingList))

                // Get available stock from TT_StorageDetail
                #region Filter construction

                var storageQueryFilter = new StorageDetailExtendedQueryFilter()
                {
                    CustomerCode = outbound.CustomerCode,
                    ProductCode = outboundDetail.ProductCode,
                    SupplierId = outboundDetail.SupplierID,
                    WHSCode = outbound.WHSCode,
                    LocationCodeNotEmpty = true,
                    OutJobNo = "",
                    Ownership = outbound.TransType == OutboundType.Return ? Ownership.Supplier : new Ownership?()
                };
                if (pm.IsCPart == 1)
                {
                    storageQueryFilter.QtyGreaterThan = 0;
                    storageQueryFilter.QtyGreaterThanZero = true;
                    storageQueryFilter.Statuses = new StorageStatus[] { StorageStatus.Putaway, StorageStatus.Allocated };
                }
                else
                {
                    storageQueryFilter.Statuses = new StorageStatus[] { StorageStatus.Putaway };
                }
                #endregion

                // current storage of the product
                var m_odsAvailable = (await repository.GetStorageDetailWithPartInfo(storageQueryFilter)).ToList();

                // Algorithm:
                // Ok, Supposely, at this stage, when we enter into the pickinglist of this customer for this product
                // it will show me a list of available pickable items. To auto-pick
                // a) We know the amount of the quantity to pick:
                // so while we have not reach // OR exceed the quantity to pick
                // add first line as picked
                if (pm.IsCPart == 1)
                {
                    return await AutoAllocatePickingListItemCPart(outbound, outboundDetail, pm, storageQueryFilter, m_odsAvailable, p_odsPickingList);
                }
                else
                {
                    return await AutoAllocatePickingListItem(outbound, outboundDetail, storageQueryFilter, m_odsAvailable, p_odsPickingList);
                }
            });
        }

        private async Task<Result<bool>> AutoAllocatePickingListItemCPart(Outbound outbound, OutboundDetail outboundDetail, PartMaster partMaster,
                                       StorageDetailExtendedQueryFilter p_oFilter,
                                       IList<StorageDetailWithPartInfoQueryResult> p_odsAvailable,
                                       IList<PickingList> p_odsPickingList,
                                       int p_intInDateType = 0, string p_strControlCode = "", string p_strControlCodeType = "", string p_strControlCodeValue = "")
        {
            var p_dblCPartSPQ = partMaster.CPartSPQ;
            decimal p_dblPickedQty = outboundDetail.PickedQty;
            decimal p_dblToPickQty = outboundDetail.Qty;

            //Step 1 : Get Spare Picking List 
            var pickingListItemNewRows = await AutoPickCPart(p_oFilter, p_dblPickedQty, p_dblToPickQty, p_dblCPartSPQ, p_odsAvailable);
            var newAndExistingPickingLists = p_odsPickingList.Select(pl => new PickingListItemRow()
            {
                PID = pl.PID,
                Qty = pl.Qty,
                ControlDate = pl.ControlDate,
                WHSCode = pl.WHSCode,
                LocationCode = pl.LocationCode,
                InboundDate = pl.InboundDate,
                InboundJobNo = pl.InboundJobNo,
                AllocatedPid = pl.AllocatedPid
            }).Union(pickingListItemNewRows);

            // do db operation here
            #region Update StorageDetail if we know PID
            foreach (var pidQty in newAndExistingPickingLists.Where(pl => !String.IsNullOrEmpty(pl.PID))
                .GroupBy(g => g.PID).ToDictionary(g => g.Key, g => g.Sum(i => i.Qty)))
            {
                var storageDetail = await repository.GetStorageDetailAsync(pidQty.Key);
                if (storageDetail == null)
                    return new InvalidResult<bool>(new JsonResultError("ErrorRetrievingStorageDetail").ToJson());

                storageDetail.Status = StorageStatus.Allocated;
                storageDetail.AllocatedQty += pidQty.Value;
                await repository.SaveChangesAsync();
            }
            #endregion

            #region Add To Pickingllist
            var pickedPkg = 0;
            foreach (var newpl in pickingListItemNewRows)
            {
                decimal pickedQty = newpl.Qty;
                decimal unitQty = 0;
                while (pickedQty > 0)
                {
                    if (pickedQty >= p_dblCPartSPQ)
                        unitQty = p_dblCPartSPQ;
                    else
                        unitQty = pickedQty;

                    var l_intSeqNo = utilityService.GetAutoNum(AutoNumTable.PickingList, outboundDetail.JobNo, outboundDetail.LineItem);
                    var l_oPickingList = new PickingList
                    {
                        JobNo = outboundDetail.JobNo,
                        LineItem = outboundDetail.LineItem,
                        ProductCode = outboundDetail.ProductCode,
                        SupplierID = outboundDetail.SupplierID,
                        SeqNo = l_intSeqNo,
                        Qty = unitQty,
                        WHSCode = newpl.WHSCode,
                        LocationCode = newpl.LocationCode,
                        InboundDate = newpl.InboundDate,
                        AllocatedPid = newpl.AllocatedPid
                    };

                    if (p_intInDateType == 1)
                        l_oPickingList.ControlDate = newpl.ControlDate;

                    if (p_strControlCodeValue.Length > 0)
                    {
                        l_oPickingList.ControlCodeType = Convert.ToByte(p_strControlCodeType.Substring(2, 1));
                        l_oPickingList.ControlCode = p_strControlCode;
                        l_oPickingList.ControlCodeValue = p_strControlCodeValue;
                    }
                    else
                        l_oPickingList.ControlCodeType = Convert.ToByte(0);


                    await repository.AddPickingListAsync(l_oPickingList);

                    #region Insert Allocated PID Allocated Qty into TT_PickingAllocatedQty table
                    var l_oPickingAllocatedPID = new PickingAllocatedPID()
                    {
                        JobNo = outboundDetail.JobNo,
                        LineItem = outboundDetail.LineItem,
                        SerialNo = l_intSeqNo,
                        PID = newpl.PID,
                        AllocatedQty = unitQty,
                        PickedQty = 0
                    };
                    await repository.AddPickingAllocatedPIDAsync(l_oPickingAllocatedPID);
                    #endregion

                    pickedPkg += 1;
                    pickedQty -= unitQty;
                }
            }

            #endregion

            #region Update Outbound Detail

            var currentPickedQty = newAndExistingPickingLists.Sum(pl => pl.Qty);
            var originalQty = outboundDetail.Qty;
            if (outboundDetail.Qty < currentPickedQty)
                outboundDetail.Qty = currentPickedQty;

            outboundDetail.PickedQty = currentPickedQty;
            outboundDetail.PickedPkg += pickedPkg;
            await repository.SaveChangesAsync();

            #endregion

            #region update Inventory Allocated Qty
            await UpdateInventoryAllocatedQty(outbound, outboundDetail);
            #endregion

            return new SuccessResult<bool>(false);
        }

        private async Task<IList<PickingListItemRow>> AutoPickCPart(StorageDetailExtendedQueryFilter storageDetailQueryFilter,
                      decimal pickedQty, decimal toPickQty, decimal cPartSPQ,
                      IList<StorageDetailWithPartInfoQueryResult> p_odsAvailable)
        {
            IList<PickingListItemRow> newPickingListRows = new List<PickingListItemRow>();
            decimal pickedSum = pickedQty;
            //Step 1 : Get the sum of the pickable items in StorageDetail, group by
            var availableStorage = await repository.GetStorageDetailListEuro(storageDetailQueryFilter);

            if (!availableStorage.Any())
            {
                return newPickingListRows;
            }
            else
            {
                decimal allocatedQty = 0;
                decimal storageRemainQty = 0;
                decimal balanceQty = toPickQty - pickedQty;

                foreach (var storageRow in availableStorage)
                {
                    // if the qty on this FIFO date can fit or is less than the order qty, pick everything on this date
                    if (pickedSum < toPickQty)
                    {
                        allocatedQty = 0;
                        var storageDetailRow = p_odsAvailable.Where(i => i.StorageDetail.PID == storageRow.PID).SingleOrDefault();
                        if(storageDetailRow != null)
                        {
                            var initialStorageRemainQty = storageRow.Qty - storageRow.AllocatedQty;
                            if (initialStorageRemainQty > cPartSPQ)
                            {
                                storageRemainQty = initialStorageRemainQty;
                                while (initialStorageRemainQty - allocatedQty > 0 &&
                                    balanceQty > 0 && storageRemainQty > 0)
                                {
                                    if (storageRemainQty > cPartSPQ)
                                    {
                                        allocatedQty += cPartSPQ;
                                        storageRemainQty -= cPartSPQ;
                                        balanceQty -= cPartSPQ;
                                    }
                                    else
                                    {
                                        if (!storageRow.IsReturn)
                                        {
                                            allocatedQty += storageRemainQty;
                                            balanceQty -= storageRemainQty;
                                            storageRemainQty = 0;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                allocatedQty = storageRow.Qty - storageRow.AllocatedQty;
                                balanceQty -= allocatedQty;
                            }

                            newPickingListRows.Add(new PickingListItemRow()
                            {
                                PID = storageDetailRow.StorageDetail.PID,
                                Qty = allocatedQty,
                                WHSCode = storageDetailRow.StorageDetail.WHSCode,
                                LocationCode = storageDetailRow.StorageDetail.LocationCode,
                                InboundDate = storageDetailRow.StorageDetail.InboundDate,
                                InboundJobNo = storageDetailRow.StorageDetail.InJobNo,
                                ControlDate = storageDetailRow.StorageDetail.ControlDate,
                                AllocatedPid = storageDetailRow.StorageDetail.PID
                            });
                            p_odsAvailable.Remove(storageDetailRow);
                        }
                    }
                    else
                    {
                        break;
                    }
                    pickedSum += allocatedQty;
                }
            }
            return newPickingListRows;
        }

        private async Task<Result<bool>> AutoAllocatePickingListItem(Outbound outbound, OutboundDetail outboundDetail,
                               StorageDetailExtendedQueryFilter p_oFilter,
                               IList<StorageDetailWithPartInfoQueryResult> p_odsAvailable,
                               IList<PickingList> p_odsPickingList,
                               int p_intInDateType = 0, string p_strControlCode = "", string p_strControlCodeType = "", string p_strControlCodeValue = "")
        {
            decimal p_dblPickedQty = outboundDetail.PickedQty;
            decimal p_dblToPickQty = outboundDetail.Qty;

            //Step 1 : Get Spare Picking List 
            // existing picking lists for this outbound detail for product
            var l_odsPickingList = p_odsPickingList.Where(pl => pl.ProductCode == outboundDetail.ProductCode).ToList();

            var pickingListItemNewRows = await AutoPick(p_oFilter, p_dblPickedQty, p_dblToPickQty, p_odsAvailable);
            var newAndExistingPickingLists = p_odsPickingList.Select(pl => new PickingListItemRow()
            {
                PID = pl.PID,
                Qty = pl.Qty,
                ControlDate = pl.ControlDate,
                WHSCode = pl.WHSCode,
                LocationCode = pl.LocationCode,
                InboundDate = pl.InboundDate,
                InboundJobNo = pl.InboundJobNo,
                AllocatedPid = pl.AllocatedPid
            }).Union(pickingListItemNewRows);

            // do db operation here
            #region Update StorageDetail

            foreach (var existingpl in newAndExistingPickingLists)
            {
                if (!String.IsNullOrEmpty(existingpl.PID))
                {
                    var l_oStorageDetail = await repository.GetStorageDetailAsync(existingpl.PID);
                    if (l_oStorageDetail == null)
                        return new InvalidResult<bool>(new JsonResultError("ErrorRetrievingStorageDetail").ToJson());

                    l_oStorageDetail.Status = StorageStatus.Allocated;
                    l_oStorageDetail.AllocatedQty = existingpl.Qty;
                    await repository.SaveChangesAsync();
                }
            }
            #endregion

            #region Add To Pickingllist
            foreach (var newpl in pickingListItemNewRows)
            {
                if (!l_odsPickingList.Where(pl => pl.PID == newpl.PID).Any())
                {
                    var l_intSeqNo = utilityService.GetAutoNum(AutoNumTable.PickingList, outboundDetail.JobNo, outboundDetail.LineItem);
                    var l_oPickingList = new PickingList
                    {
                        JobNo = outboundDetail.JobNo,
                        LineItem = outboundDetail.LineItem,
                        ProductCode = outboundDetail.ProductCode,
                        SupplierID = outboundDetail.SupplierID,
                        SeqNo = l_intSeqNo,
                        Qty = newpl.Qty,
                        WHSCode = newpl.WHSCode,
                        LocationCode = newpl.LocationCode,
                        InboundDate = newpl.InboundDate,
                        InboundJobNo = newpl.InboundJobNo,
                        AllocatedPid = newpl.AllocatedPid
                    };

                    if (p_intInDateType == 1)
                        l_oPickingList.ControlDate = newpl.ControlDate;

                    if (p_strControlCodeValue.Length > 0)
                    {
                        l_oPickingList.ControlCodeType = Convert.ToByte(p_strControlCodeType.Substring(2, 1));
                        l_oPickingList.ControlCode = p_strControlCode;
                        l_oPickingList.ControlCodeValue = p_strControlCodeValue;
                    }
                    else
                        l_oPickingList.ControlCodeType = Convert.ToByte(0);

                    await repository.AddPickingListAsync(l_oPickingList);
                }
            }

            #endregion

            #region Update Outbound Detail

            var l_decPickedQty = newAndExistingPickingLists.Sum(pl => pl.Qty);

            var originalQty = outboundDetail.Qty;
            if (outboundDetail.Qty < l_decPickedQty)
                outboundDetail.Qty = l_decPickedQty;

            outboundDetail.PickedQty = l_decPickedQty;
            outboundDetail.PickedPkg = newAndExistingPickingLists.Count();

            if (!appSettings.OwnerCode.StartsWith(UtilityService.TESA_CODE))
            {
                var inventory = await repository.GetInventoryAsync(outbound.CustomerCode, outboundDetail.SupplierID, outboundDetail.ProductCode, outbound.WHSCode, (byte)Ownership.Supplier);
                inventory.AllocatedQty = inventory.AllocatedQty - originalQty + outboundDetail.Qty;
            }
            await repository.SaveChangesAsync();

            #endregion

            #region update Inventory Allocated Qty
            if (outbound.TransType == OutboundType.ManualEntry
                || outbound.TransType == OutboundType.WHSTransfer
                || outbound.TransType == OutboundType.ScannerManualEntry)
            {
                var inventoryEhp = await repository.GetInventoryAsync(outbound.CustomerCode, outboundDetail.SupplierID, outboundDetail.ProductCode, outbound.WHSCode, Ownership.EHP);
                inventoryEhp.AllocatedQty = 0;
                inventoryEhp.AllocatedPkg = 0;
                var inventorySupplier = await repository.GetInventoryAsync(outbound.CustomerCode, outboundDetail.SupplierID, outboundDetail.ProductCode, outbound.WHSCode, (byte)Ownership.Supplier);
                inventorySupplier.AllocatedQty = 0;
                inventorySupplier.AllocatedPkg = 0;

                await repository.SaveChangesAsync();

                var l_odsAllocatedPID = await repository.GetAllocatedStorageDetailSummaryList(new AllocatedStorageDetailSummaryQueryFilter()
                {
                    CustomerCode = outbound.CustomerCode,
                    SupplierID = outboundDetail.SupplierID,
                    ProductCode = outboundDetail.ProductCode,
                    WHSCode = outbound.WHSCode
                });

                foreach (var dr in l_odsAllocatedPID)
                {
                    var inventory = await repository.GetInventoryAsync(dr.CustomerCode, dr.SupplierID, dr.ProductCode, dr.WHSCode, dr.Ownership);
                    inventory.AllocatedQty = dr.AllocatedQty;
                    inventory.AllocatedPkg = dr.AllocatedPkg;
                    await repository.SaveChangesAsync();
                }

                var l_odsUnallocateQty = await repository.GetOutboundDetailUnallocatedQty(outbound.CustomerCode, outboundDetail.SupplierID, outboundDetail.ProductCode, outbound.WHSCode);

                if (l_odsUnallocateQty.HasValue && l_odsUnallocateQty.Value > 0)
                {
                    //Step 4 : Update Allocated Qty in Inventory 
                    // Use Batch Update instead of Object in case there is concurrency issue
                    var l_oInventory = await repository.GetInventoryAsync(outbound.CustomerCode, outboundDetail.SupplierID, outboundDetail.ProductCode, outbound.WHSCode, Ownership.EHP);
                    var l_dblInv = l_oInventory.OnHandQty - l_oInventory.AllocatedQty - l_oInventory.QuarantineQty;

                    if (l_dblInv >= l_odsUnallocateQty.Value)
                    {
                        l_oInventory.AllocatedQty = l_odsUnallocateQty.Value;
                    }
                    else if (l_dblInv < l_odsUnallocateQty.Value)
                    {
                        l_oInventory.AllocatedQty = l_dblInv;

                        var supplierInventory = await repository.GetInventoryAsync(outbound.CustomerCode, outboundDetail.SupplierID, outboundDetail.ProductCode, outbound.WHSCode, (byte)Ownership.Supplier);
                        supplierInventory.AllocatedQty = l_odsUnallocateQty.Value - l_dblInv;
                    }
                    await repository.SaveChangesAsync();
                }
            }
            #endregion

            return new SuccessResult<bool>(false);
        }

        private async Task<IList<PickingListItemRow>> AutoPick(StorageDetailExtendedQueryFilter storageDetailQueryFilter,
              decimal p_dblPickedQty, decimal p_dblToPickQty, IList<StorageDetailWithPartInfoQueryResult> p_odsAvailable)
        {
            IList<PickingListItemRow> newPickingListRows = new List<PickingListItemRow>();
            decimal l_dblPickedSum = p_dblPickedQty;

            //Step 1 : Get the sum of the pickable items in StorageDetail, group by
            var availableStorage = await repository.GetStorageDetailListEuro(storageDetailQueryFilter);

            //await Task.Delay((int)(new Random().NextDouble() * 100)); //this line is for unit test 

            if (!availableStorage.Any())
            {
                return newPickingListRows;
            }
            else
            {
                foreach (var storageRow in availableStorage)
                {
                    // if the qty on this FIFO date can fit or is less than the order qty, pick everything on this date
                    if (l_dblPickedSum < p_dblToPickQty)
                    {
                        //l_dblAllocatedQty = 0;
                        var storageDetailRows = p_odsAvailable.Where(i => i.StorageDetail.PID == storageRow.PID).ToList();

                        foreach (var storageDetailRow in storageDetailRows)
                        {
                            newPickingListRows.Add(new PickingListItemRow()
                            {
                                PID = storageDetailRow.StorageDetail.PID,
                                Qty = storageDetailRow.StorageDetail.Qty,
                                WHSCode = storageDetailRow.StorageDetail.WHSCode,
                                LocationCode = storageDetailRow.StorageDetail.LocationCode,
                                InboundDate = storageDetailRow.StorageDetail.InboundDate,
                                InboundJobNo = storageDetailRow.StorageDetail.InJobNo,
                                ControlDate = storageDetailRow.StorageDetail.ControlDate,
                                AllocatedPid = storageDetailRow.StorageDetail.PID
                            });
                            p_odsAvailable.Remove(storageDetailRow);
                        }
                    }
                    else
                    {
                        break;
                    }
                    l_dblPickedSum += storageRow.Qty;
                }
            }
            return newPickingListRows;
        }

        public async Task<Result<bool>> AllocatePickingListBatch(IEnumerable<PickingListAllocateDto> pickingList)
        {
            foreach (var pl in pickingList)
            {
                var allocationResult = await AllocatePickingListItem(pl);
                if (allocationResult.ResultType != ResultType.Ok) { return allocationResult; }
            }
            return new SuccessResult<bool>(true);
        }

        private async Task<Result<bool>> AllocatePickingListItem(PickingListAllocateDto pickingListDto)
        {
            if(await iLogConnect.IsProcessingOutbound(pickingListDto.JobNo))
                return new InvalidResult<bool>(new JsonResultError("ILogIsProcessingThisOutbound").ToJson());

            return await WithTransactionScopeAndLock<bool>(async () =>
            {
                var outbound = await repository.GetOutboundAsync(pickingListDto.JobNo);
                var outboundDetail = await repository.GetOutboundDetailAsync(pickingListDto.JobNo, pickingListDto.LineItem);
                if (outbound == null || outboundDetail == null)
                    return new InvalidResult<bool>(new JsonResultError("ErrorRetrievingOutboundDetail").ToJson());

                var storageDetail = await repository.GetStorageDetailAsync(pickingListDto.PID);
                if (storageDetail == null)
                    return new InvalidResult<bool>(new JsonResultError("ErrorRetrievingStorageDetail").ToJson());

                if (PickedMoreThanOrderQty(pickingListDto.JobNo, pickingListDto.LineItem, outboundDetail.Qty, storageDetail.Qty))
                {
                    return new InvalidResult<bool>(new JsonResultError("YouHavePickedMoreThanOrderQty").ToJson());
                }

                var pidAvaliableForPicking = await IsPIDAvailableForAllocation(pickingListDto.PID, outbound.WHSCode, outbound.CustomerCode, outboundDetail.ProductCode, outboundDetail.SupplierID, outbound.TransType);
                if (!pidAvaliableForPicking)
                    return new InvalidResult<bool>(new JsonResultError("ItemAlreadyPicked__", "pid", pickingListDto.PID).ToJson());

                var pickingList = mapper.Map<PickingList>(pickingListDto);
                pickingList.ProductCode = outboundDetail.ProductCode;
                pickingList.WHSCode = outbound.WHSCode;
                pickingList.SupplierID = outboundDetail.SupplierID;
                pickingList.Qty = storageDetail.Qty;
                pickingList.LocationCode = storageDetail.LocationCode;
                pickingList.InboundDate = storageDetail.InboundDate;
                pickingList.InboundJobNo = storageDetail.InJobNo;
                pickingList.AllocatedPid = storageDetail.PID;
                //TODO: 
                //pickingList.DownloadDate = DateTime.Now;               
                pickingList.SeqNo = utilityService.GetAutoNum(AutoNumTable.PickingList, pickingList.JobNo, pickingList.LineItem);

                //Step 2 : Add Entry to Picking List
                await repository.AddPickingListAsync(pickingList);

                //Step 3 : Update Storage Detail
                // Update storagedetail
                // For manual pick, we need to add the OutJobNo and PID AND update AllocatedQty
                storageDetail.OutJobNo = pickingList.JobNo;
                storageDetail.AllocatedQty = storageDetail.Qty;
                storageDetail.Status = StorageStatus.Allocated;

                //Step 4 : Update Outbound Detail

                outboundDetail.PickedQty += pickingList.Qty;
                outboundDetail.PickedPkg += 1;

                await repository.SaveChangesAsync();
                #region update Inventory Allocated Qty
                await UpdateInventoryAllocatedQty(outbound, outboundDetail);
                #endregion

                return new SuccessResult<bool>(true);
            });
        }

        private async Task<bool> IsPIDAvailableForAllocation(string pid, string whsCode, string customerCode, string productCode, string supplierID, OutboundType transactionType)
        {
            var filter = new StorageDetailQueryFilter()
            {
                WHSCode = whsCode,
                CustomerCode = customerCode,
                ProductCode = productCode,
                SupplierId = supplierID,
                LocationType = LocationType.Normal,
                Statuses = new StorageStatus[] { StorageStatus.Putaway },
                PID = pid
            };
            if (transactionType == OutboundType.Return)
                filter.Ownership = Ownership.Supplier;

            return (await repository.GetStorageDetailWithPartInfo(filter)).Any();
        }

        private void UpdateInventoryQuantities(Inventory inventory, decimal allocatedQty, int? allocatedPkg)
        {
            inventory.AllocatedQty = allocatedQty;
            inventory.AllocatedPkg = allocatedPkg;
        }

        private async Task UpdateInventoryAllocatedQty(Outbound outbound, OutboundDetail outboundDetail)
        {
            #region update Inventory Allocated Qty
            if (outbound.TransType == OutboundType.ManualEntry
                || outbound.TransType == OutboundType.WHSTransfer
                || outbound.TransType == OutboundType.ScannerManualEntry)
            {
                var inventoryEhp = await repository.GetInventoryAsync(outbound.CustomerCode, outboundDetail.SupplierID, outboundDetail.ProductCode, outbound.WHSCode, Ownership.EHP);
                inventoryEhp.AllocatedQty = 0;
                inventoryEhp.AllocatedPkg = 0;
                var inventorySupplier = await repository.GetInventoryAsync(outbound.CustomerCode, outboundDetail.SupplierID, outboundDetail.ProductCode, outbound.WHSCode, Ownership.Supplier);
                inventorySupplier.AllocatedQty = 0;
                inventorySupplier.AllocatedPkg = 0;

                await repository.SaveChangesAsync();

                var l_odsAllocatedPID = await repository.GetAllocatedStorageDetailSummaryList(new AllocatedStorageDetailSummaryQueryFilter()
                {
                    CustomerCode = outbound.CustomerCode,
                    SupplierID = outboundDetail.SupplierID,
                    ProductCode = outboundDetail.ProductCode,
                    WHSCode = outbound.WHSCode
                });

                foreach (var dr in l_odsAllocatedPID)
                {
                    UpdateInventoryQuantities(dr.Ownership == Ownership.Supplier ? inventorySupplier : inventoryEhp, dr.AllocatedQty, dr.AllocatedPkg);
                    await repository.SaveChangesAsync();
                }

                var outboundUnallocateQty = await repository.GetOutboundDetailUnallocatedQty(outbound.CustomerCode, outboundDetail.SupplierID, outboundDetail.ProductCode, outbound.WHSCode);
                if (outboundUnallocateQty.HasValue && outboundUnallocateQty.Value > 0)
                {
                    //Step 4 : Update Allocated Qty in Inventory 
                    var ehpRemainingQty = inventoryEhp.OnHandQty - inventoryEhp.AllocatedQty - inventoryEhp.QuarantineQty;

                    if (ehpRemainingQty >= outboundUnallocateQty.Value)
                    {
                        inventoryEhp.AllocatedQty = outboundUnallocateQty.Value;
                    }
                    else if (ehpRemainingQty < outboundUnallocateQty.Value)
                    {
                        inventoryEhp.AllocatedQty = ehpRemainingQty;
                        inventorySupplier.AllocatedQty = outboundUnallocateQty.Value - ehpRemainingQty;
                    }
                    await repository.SaveChangesAsync();
                }
            }
            #endregion
        }

        public async Task<Result<bool>> UnAllocateBatch(IList<UndoAllocationDto> unAllocateData)
        {
            if (unAllocateData.Count > 0)
            {
                var outboundJob = unAllocateData.First().JobNo;
                var outbound = await repository.GetOutboundAsync(outboundJob);
                
                if (outbound.TransType == OutboundType.EKanban)
                {
                    return new InvalidResult<bool>(new JsonResultError("AutoallocationNotAllowed").ToJson());
                }
            }

            foreach (var data in unAllocateData)
            {
                var unAllocateResult = await UnAllocate(data.JobNo, data.LineItem, data.SeqNo, data.PID);
                if (unAllocateResult.ResultType != ResultType.Ok) { return unAllocateResult; }
            }
            return new SuccessResult<bool>(true);
        }

        private async Task<Result<bool>> UnAllocate(string outJobNo, int lineItem, int? seqNo, string PID)
        {
            if(await iLogConnect.IsProcessingOutbound(outJobNo))
                return new InvalidResult<bool>(new JsonResultError("ILogIsProcessingThisOutbound").ToJson());

            return await WithTransactionScopeAndLock(async () =>
            {
                var pickingList = await repository.PickingLists()
                    .Where(pl => pl.JobNo == outJobNo && pl.LineItem == lineItem &&
                                (seqNo == null || pl.SeqNo == seqNo) &&
                                (String.IsNullOrEmpty(PID) || pl.PID == PID))
                    .OrderBy(pl => pl.SeqNo)
                    .ToListAsync();

                Result<bool> result = default;
                foreach (var pl in pickingList)
                {
                    result = await UnAllocatedPickingListItem(pl);
                    if (result.ResultType != ResultType.Ok) { return result; }
                }

                return new SuccessResult<bool>(true);
            });
        }

        private async Task<Result<bool>> UnAllocatedPickingListItem(PickingList pickingList)
        {
            bool isCPart = false;

            bool emptyPID = string.IsNullOrEmpty(pickingList.PID);
            string allocatedPIDNo = pickingList.PID;
            //Step 2 : Update Storage Detail
            var pickingAllocatedPID = await repository.GetPickingAllocatedPIDAsync(pickingList.JobNo, pickingList.LineItem, pickingList.SeqNo);
            if (pickingAllocatedPID != null)
            {
                if (string.IsNullOrEmpty(allocatedPIDNo)) allocatedPIDNo = pickingAllocatedPID.PID;
                await repository.DeletePickingAllocatedPIDAsync(pickingAllocatedPID);
                isCPart = true;
            }
            var pickingListAllocatedPID = await repository.GetPickingListAllocatedPIDAsync(pickingList.JobNo, pickingList.LineItem, pickingList.SeqNo);
            if (pickingListAllocatedPID != null)
            {
                if (string.IsNullOrEmpty(allocatedPIDNo)) allocatedPIDNo = pickingListAllocatedPID.PID;
                await repository.DeletePickingListAllocatedPIDAsync(pickingListAllocatedPID);
            }

            StorageDetail storageDetail = null;
            if (!string.IsNullOrEmpty(allocatedPIDNo))
            {
                storageDetail = await repository.GetStorageDetailAsync(allocatedPIDNo);
                if (storageDetail == null)
                    return new InvalidResult<bool>(new JsonResultError("CantFindPickingItemFromStorageMaster").ToJson());

                if (!emptyPID)
                {
                    if (storageDetail.OutJobNo != pickingList.JobNo)
                        return new InvalidResult<bool>(new JsonResultError
                        {
                            MessageKey = "PidNotAllocatedByOutJob__",
                            Arguments = new Dictionary<string, string>
                        {
                            {"pid",pickingList.PID },
                            {"jobNo",pickingList.JobNo }
                        }
                        }.ToJson());

                    if (storageDetail.Status >= StorageStatus.Picked)
                        return new InvalidResult<bool>(new JsonResultError
                        {
                            MessageKey = "PidInvalidStatus__",
                            Arguments = new Dictionary<string, string>
                        {
                            {"pid",pickingList.PID },
                            {"status",((StorageStatus)storageDetail.Status).ToString() }
                        }
                        }.ToJson());
                }
            }
            else if (!isCPart)
            {
                // find 1 line in storagedetail that meets the criteria (qty, productcode, inbound/control date)
                // order by PID, and free up that line item
                storageDetail = await repository.GetStorageDetailForFilter(new StorageQueryFilter
                {
                    ProductCode = pickingList.ProductCode,
                    Qty = pickingList.Qty,
                    Status = StorageStatus.Allocated,
                    OutJobNo = "",
                    LocationCode = pickingList.LocationCode
                });
            }

            if (storageDetail == null)
                return new InvalidResult<bool>(new JsonResultError("CantFindPickingItemFromStorageMaster").ToJson());

            storageDetail.OutJobNo = "";
            if (isCPart)
                storageDetail.AllocatedQty -= pickingList.Qty;
            else
                storageDetail.AllocatedQty = 0;

            if (storageDetail.AllocatedQty == 0)
                storageDetail.Status = StorageStatus.Putaway;

            //Step 3 : Update Outbound Detail
            var outboundDetail = await repository.GetOutboundDetailAsync(pickingList.JobNo, pickingList.LineItem);
            outboundDetail.PickedQty -= pickingList.Qty;
            outboundDetail.PickedPkg -= 1;
            if (outboundDetail.PickedQty < 0)
                return new InvalidResult<bool>(new JsonResultError("NegativePickingQty").ToJson());

            // AU: no need to do this step as we do not change the outboundDetail.Qty so we do not need to change Inventory for non-TESA 
            //await UpdateOutboundDetail(outboundDetail, Ownership.Supplier);

            //step 4 : Remove from Picking List
            await repository.DeletePickingListAsync(pickingList);
            return new SuccessResult<bool>(true);
        }

        private bool PickedMoreThanOrderQty(string jobNo, int lineItem, decimal orderQty, decimal additionalQty)
        {
            var totalQtyPicked = repository.PickingLists()
                .Where(p => p.JobNo == jobNo && p.LineItem == lineItem)
                .Sum(p => p.Qty);
            return orderQty < totalQtyPicked + additionalQty;
        }

        private class PickingListItemRow
        {
            public string PID { get; set; }
            public decimal Qty { get; set; }
            public string WHSCode { get; set; }
            public string LocationCode { get; set; }
            public string InboundJobNo { get; set; }
            public DateTime InboundDate { get; set; }
            public DateTime? ControlDate { get; set; }
            public string AllocatedPid { get; set; }
        }

        private readonly ITTLogixRepository repository;
        private readonly IOutboundService outboundService;
        private readonly IUtilityService utilityService;
        private readonly IILogConnect iLogConnect;
        private readonly AppSettings appSettings;
        private readonly IMapper mapper;
    }
}
