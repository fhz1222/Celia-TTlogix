using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NPOI.SS.UserModel;
using ServiceResult;
using System;
using System.Collections.Generic;
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
    public class InboundService : ServiceBase<InboundService>, IInboundService
    {
        public InboundService(ITTLogixRepository repository,
            ILocker locker,
            ILogger<InboundService> logger,
            IMapper mapper,
            IUtilityService utilityService,
            IOptions<AppSettings> appSettings,
            IReportService reportService,
            IBillingService billingService,
            IXlsService xlsService,
            IILogConnect iLogConnect)
            : base(locker, logger)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.utilityService = utilityService;
            this.reportService = reportService;
            this.billingService = billingService;
            this.xlsService = xlsService;
            this.appSettings = appSettings.Value;
            this.repository = repository;
            this.iLogConnect = iLogConnect;
        }

        public async Task<InboundListDto> GetInboundList(InboundListQueryFilter filter)
        {
            var query = repository.GetInboundList<InboundListItemDto>(filter);

            var pagedQuery = query.Skip(filter.PageSize * (filter.PageNo - 1)).Take(filter.PageSize);
            var total = await query.CountAsync();
            var data = await pagedQuery.ToListAsync();

            return new InboundListDto
            {
                Data = data,
                PageSize = filter.PageSize,
                PageNo = filter.PageNo,
                Total = total
            };
        }

        public async Task<InboundDto> GetInbound(string jobNo)
        {
            return await repository.GetInboundExtendedAsync<InboundDto>(jobNo);
        }

        public async Task<IEnumerable<InboundDetailDto>> GetInboundDetailList(string jobNo)
        {
            return await repository.GetInboundDetailListWithPrice<InboundDetailDto>(jobNo);
        }

        public async Task<Result<string>> CreateInboundManual(InboundManualDto inboundDto, string userCode)
        {
            return await WithTransactionScope(async () =>
            {

                var entity = mapper.Map<Inbound>(inboundDto);
                entity.CreatedBy = userCode;
                var jobNoResult = await utilityService.GenerateJobNo(JobType.Inbound);
                if (jobNoResult.ResultType == ResultType.Invalid)
                {
                    return jobNoResult;
                }
                entity.JobNo = jobNoResult.Data;
                await repository.AddInboundAsync(entity);
                return new SuccessResult<string>(entity.JobNo);
            });
        }

        public async Task<ASNListDto> GetASNListToImport(ASNListQueryFilter filter)
        {
            var query = repository.GetASNListToImport<ASNListItemDto>(filter);

            var pagedQuery = query.Skip(filter.PageSize * (filter.PageNo - 1)).Take(filter.PageSize);
            var total = await query.CountAsync();
            var data = await pagedQuery.ToListAsync();

            return new ASNListDto
            {
                Data = data,
                PageSize = filter.PageSize,
                PageNo = filter.PageNo,
                Total = total
            };
        }

        public async Task<IEnumerable<ASNDetailSimpleDto>> GetASNDetails(string asnNo)
        {
            return await repository.ASNDetails().Where(d => d.ASNNo == asnNo).OrderBy(d => d.LineItem)
                .Select(i => new ASNDetailSimpleDto
                {
                    LineItem = i.LineItem,
                    NoOfOuter = i.NoOfOuter,
                    QtyPerOuter = i.QtyPerOuter,
                    ProductCode = i.ProductCode
                }).ToListAsync();
        }

        public async Task<Result<string[]>> ImportFile(Stream file, string whsCode, string customerCode, string supplierID, string userCode)
        {
            return await WithTransactionScopeAndLock<string[]>(async () =>
            {
                var jobs = new List<string>();
                repository.ChangeTrackingOff();

                var sheet = WorkbookFactory.Create(file).GetSheetAt(0);

                var jobNoResult = await utilityService.GenerateJobNo(JobType.Inbound);
                if (jobNoResult.ResultType != ResultType.Ok)
                {
                    return new InvalidResult<string[]>(jobNoResult.Errors[0]);
                }
                #region create inbound
                var inbound = new Inbound
                {
                    JobNo = jobNoResult.Data,
                    CustomerCode = customerCode,
                    SupplierID = supplierID,
                    WHSCode = whsCode,
                    RefNo = "",
                    IRNo = "",
                    Remark = "",
                    TransType = InboundType.ManualEntry,
                    CreatedBy = userCode,
                    ETA = DateTime.Now
                };
                await repository.AddInboundAsync(inbound);
                #endregion

                // get the supplier master only once per inbound
                //step 2 : Load Part Master & Supplier Master AU: part master is not really used 
                var supplierMaster = await repository.GetSupplierMasterAsync(inbound.CustomerCode, inbound.SupplierID);
                if (supplierMaster == null)
                {
                    return new InvalidResult<string[]>(new JsonResultError("RecordNotFound").ToJson());
                }



                #region Step 5: Create Inbound Detail


                var inboundDetails = new List<InboundDetail>();
                var storageDetails = new List<StorageDetail>();
                var externalPids = new List<ExternalPID>();
                jobs.Add(inbound.JobNo);

                DataFormatter formatter = new DataFormatter();
                foreach (IRow row in sheet)
                {
                    if (row.RowNum == 0)
                    {
                        continue;
                    }
                    if (row.GetCell(0) == null || formatter.FormatCellValue(row.GetCell(0)).Trim() == "")
                    {
                        continue;
                    }
                    string ProductCode = formatter.FormatCellValue(row.GetCell(0));

                    int qty = 0;
                    try
                    {
                        qty = (int)row.GetCell(1).NumericCellValue;
                    }
                    catch (InvalidOperationException)
                    {
                        return new InvalidResult<string[]>(new JsonResultError("InboundFileQtyNotNumeric__", "row", (row.RowNum + 1).ToString()).ToJson());
                    }
                    
                    string ETD = string.Empty;
                    string ControlCode2 = string.Empty;
                    string ControlCode3 = string.Empty;
                    string ControlCode4 = string.Empty;
                    string ControlCode5 = string.Empty;
                    string ControlCode6 = string.Empty;
                    string ExternalPID = "";
                    if (row.GetCell(2) != null)
                    {
                        ExternalPID = formatter.FormatCellValue(row.GetCell(2)).Trim();
                    }
                    if (row.GetCell(3) != null)
                    {
                        ETD = formatter.FormatCellValue(row.GetCell(3));
                    }
                    if (row.GetCell(4) != null)
                    {
                        ControlCode2 = formatter.FormatCellValue(row.GetCell(4));
                    }
                    if (row.GetCell(5) != null)
                    {
                        ControlCode3 = formatter.FormatCellValue(row.GetCell(5));
                    }
                    if (row.GetCell(6) != null)
                    {
                        ControlCode4 = formatter.FormatCellValue(row.GetCell(6));
                    }
                    if (row.GetCell(7) != null)
                    {
                        ControlCode5 = formatter.FormatCellValue(row.GetCell(7));
                    }
                    if (row.GetCell(8) != null)
                    {
                        ControlCode6 = formatter.FormatCellValue(row.GetCell(8));
                    }

                    //string ;
                    var partMaster = await repository.GetPartMasterAsync(inbound.CustomerCode, inbound.SupplierID, ProductCode);
                    if (partMaster == null)
                    {
                        return new InvalidResult<string[]>(new JsonResultError("Part not found: " + ProductCode).ToJson());
                    }

                    var inboundDetail = new InboundDetail()
                    {
                        JobNo = inbound.JobNo,
                        ProductCode = ProductCode,

                        PackageType = partMaster.PackageType,
                        NoOfLabel = 1,
                        NoOfPackage = 1,
                        Length = partMaster.LengthTT,
                        Width = partMaster.WidthTT,
                        Height = partMaster.HeightTT,
                        NetWeight = partMaster.NetWeightTT,
                        GrossWeight = partMaster.GrossWeightTT,
                        ControlCode1 = ETD,
                        ControlCode2 = ControlCode2,
                        ControlCode3 = ControlCode3,
                        ControlCode4 = ControlCode4,
                        ControlCode5 = ControlCode5,
                        ControlCode6 = ControlCode6,
                        LineItem = row.RowNum,
                        CreatedBy = userCode,
                        PkgLineItem = 0,
                        ImportedQty = qty,
                        Qty = qty
                    };



                    var addResult = await AddNewInboundDetail(inbound, inboundDetail, supplierMaster, false);
                    if (addResult.ResultType != ResultType.Ok)
                        return new InvalidResult<string[]>(addResult.Errors[0]);

                    storageDetails.AddRange(addResult.Data);
                    inboundDetails.Add(inboundDetail);
                    if (ExternalPID != "")
                    {
                        var ext = new ExternalPID();
                        ext.ExternalID = ExternalPID;
                        ext.ExternalSystem = 8;
                        ext.InJobNo = inbound.JobNo;
                        ext.InLineItem = row.RowNum;
                        ext.PID = addResult.Data.First().PID;
                        externalPids.Add(ext);
                    }
                }


                await repository.BatchAddInboundDetailAsync(inboundDetails);
                await repository.BatchAddStorageDetailAsync(storageDetails);
                if (externalPids.Count > 0)
                {
                    await repository.BatchAddExternalPIDAsync(externalPids, false);
                }

                #endregion



                await repository.SaveChangesAsync();

                return new SuccessResult<string[]>(jobs.ToArray());
            });
        }

        public async Task<Result<string>> ImportASN(string asnNo, string whsCode, string userCode)
        {
            return await WithTransactionScopeAndLock<string>(async () =>
            {
                repository.ChangeTrackingOff();
                #region Step 1: Determine the CustomerID and Supply Paradigm
                var asnHeader = await repository.GetASNHeaderAsync(asnNo);
                if (asnHeader == null)
                {
                    return new InvalidResult<string>(new JsonResultError("RecordNotFound").ToJson());
                }

                if ((asnHeader.Status == "REC") || (asnHeader.Status == "DTL"))
                {
                    return new InvalidResult<string>(new JsonResultError()
                    {
                        MessageKey = "UnableToImportASNWithStatus__",
                        Arguments = new Dictionary<string, string>(){
                            { "asnNo", asnNo },
                            { "status", asnHeader.Status }
                        }
                    }.ToJson());
                }
                var bVMI = asnHeader.IsVMI == 1;
                // CustomerID will be the FactoryID
                var l_strCustomerID = asnHeader.FactoryID;
                #endregion

                #region Step 2: Load the list of ASN Detail
                var asnDetailRows = await repository.GetASNDetailWithSPQList(asnNo);
                if (!asnDetailRows.Any())
                {
                    return new InvalidResult<string>(new JsonResultError("ASNNoDoesNotContainASNDetail__", "asnNo", asnNo).ToJson());
                }
                #endregion

                #region Step 4: Create Inbound Header
                var jobNoResult = await utilityService.GenerateJobNo(JobType.Inbound);
                if (jobNoResult.ResultType != ResultType.Ok)
                    return new InvalidResult<string>(jobNoResult.Errors[0]);

                var inbound = new Inbound
                {
                    JobNo = jobNoResult.Data,
                    CustomerCode = asnHeader.FactoryID,
                    SupplierID = asnHeader.SupplierID,
                    WHSCode = whsCode,
                    RefNo = asnNo,
                    IRNo = asnNo,
                    Remark = "",
                    TransType = InboundType.ASN,
                    CreatedBy = userCode,
                    ETA = DateTime.Now
                };
                await repository.AddInboundAsync(inbound);
                #endregion

                // get the supplier master only once per inbound
                //step 2 : Load Part Master & Supplier Master AU: part master is not really used 
                var supplierMaster = await repository.GetSupplierMasterAsync(inbound.CustomerCode, inbound.SupplierID);
                if (supplierMaster == null)
                {
                    return new InvalidResult<string>(new JsonResultError("RecordNotFound").ToJson());
                }

                #region Step 5: Create Inbound Detail & update InJobNo in ASNDetail

                int lineItemIdx = 1;

                var inboundDetails = new List<InboundDetail>();
                var storageDetails = new List<StorageDetail>();

                foreach (var asnDetailRow in asnDetailRows)
                {
                    var asnDetail = asnDetailRow.ASNDetail;
                    if ((asnDetail.Status == "REC") || (asnDetail.Status == "DTL") || (asnDetail.Status == "IMP"))
                    {
                        return new InvalidResult<string>(new JsonResultError()
                        {
                            MessageKey = "UnableToImportASNWithStatus__",
                            Arguments = new Dictionary<string, string>
                            {
                                {"asnNo", asnNo},
                                {"status", asnDetail.Status }
                            }
                        }.ToJson());
                    }

                    var totalQty = asnDetail.NoOfOuter * asnDetail.QtyPerOuter;
                    var noOfPackage = asnDetail.NoOfOuter;
                    var qtyPerOuter = asnDetail.QtyPerOuter;

                    if (String.IsNullOrWhiteSpace(asnDetailRow.ProductCode))
                        return new InvalidResult<string>(new JsonResultError("ProductCodeNotRegisteredInPartsMaster__", "productCode", asnDetailRow.ASNDetail.ProductCode).ToJson());

                    if (asnDetailRow.SPQ < 1)
                        return new InvalidResult<string>(new JsonResultError("ProductCodeHasInvalidSPQ__", "productCode", asnDetailRow.ProductCode).ToJson());

                    if (totalQty < 1)
                        return new InvalidResult<string>(new JsonResultError("ProductCodeHasInvalidQty__", "productCode", asnDetailRow.ProductCode).ToJson());

                    var l_dblSPQ = asnDetailRow.SPQ;

                    var inboundDetail = CreateInboundDetailFromASNDetail(asnDetailRow, inbound.JobNo, userCode);

                    if (totalQty >= asnDetail.QtyPerOuter)
                    {
                        var qty = totalQty - (totalQty % qtyPerOuter);
                        noOfPackage = qty / qtyPerOuter;

                        inboundDetail.LineItem = lineItemIdx;

                        inboundDetail.ImportedQty = qty;
                        inboundDetail.Qty = qty;
                        inboundDetail.NoOfPackage = noOfPackage;
                        inboundDetail.NoOfLabel = noOfPackage;

                        //Step 4 : Insert into Inbound Detail
                        var addResult = await AddNewInboundDetail(inbound, inboundDetail, supplierMaster, bVMI);
                        if (addResult.ResultType != ResultType.Ok)
                            return new InvalidResult<string>(addResult.Errors[0]);

                        storageDetails.AddRange(addResult.Data);
                        inboundDetails.Add(inboundDetail);
                    }

                    #region Add remaining Partial Box if it exists
                    if ((totalQty % qtyPerOuter) != 0)
                    {
                        lineItemIdx++;
                        var remainingQty = totalQty % qtyPerOuter;

                        var additionalInboundDetail = CreateInboundDetailFromASNDetail(asnDetailRow, inbound.JobNo, userCode);
                        additionalInboundDetail.LineItem = lineItemIdx;
                        additionalInboundDetail.ImportedQty = remainingQty;
                        additionalInboundDetail.Qty = remainingQty;
                        additionalInboundDetail.NoOfPackage = 1;
                        additionalInboundDetail.NoOfLabel = 1;

                        var addResult = await AddNewInboundDetail(inbound, additionalInboundDetail, supplierMaster, bVMI);
                        if (addResult.ResultType != ResultType.Ok)
                            return new InvalidResult<string>(addResult.Errors[0]);

                        storageDetails.AddRange(addResult.Data);
                        inboundDetails.Add(additionalInboundDetail);
                    }
                    #endregion

                    //Update InjobNo into ASNDetail
                    asnDetail.InJobNo = jobNoResult.Data;
                    asnDetail.Status = "IMP";

                    lineItemIdx++;
                }

                await repository.BatchAddInboundDetailAsync(inboundDetails);
                await repository.BatchAddStorageDetailAsync(storageDetails);

                #endregion

                #region Step 6: Update Status of ASNNo in <ASNHeader>
                if (asnHeader.Status != "IMP")
                {
                    asnHeader.Status = "IMP";
                }
                #endregion

                await repository.SaveChangesAsync();

                return new SuccessResult<string>(jobNoResult.Data);
            });
        }

        public async Task<Result<InboundDto>> UpdateInbound(string jobNo, InboundDto inboundDto, string userCode)
        {
            return await WithTransactionScope<InboundDto>(async () =>
            {
                var entity = await repository.GetInboundAsync(jobNo);
                if (entity == null)
                {
                    return new NotFoundResult<InboundDto>(new JsonResultError("RecordNotFound").ToJson());
                }
                if (entity.Status == InboundStatus.Cancelled)
                {
                    return new InvalidResult<InboundDto>(new JsonResultError("CannotSaveRecordIsCancelled").ToJson());
                }
                mapper.Map(inboundDto, entity);
                entity.RevisedBy = userCode;
                entity.RevisedDate = DateTime.Now;
                await repository.SaveChangesAsync();

                var dto = await repository.GetInboundExtendedAsync<InboundDto>(entity.JobNo);
                return new SuccessResult<InboundDto>(dto);
            });
        }

        public async Task<Result<bool>> CreateInboundDetail(InboundDetailEntryAddDto inboundDetailDto, string whsCode, string userCode)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                // step 0 Cycle Count check:
                var cycleCheck = await HasAnyOutstandingCycleCountJobCycleCountItem(new string[] { inboundDetailDto.ProductCode }, whsCode);
                if (cycleCheck.ResultType != ResultType.Ok)
                {
                    return cycleCheck;
                }

                var inbound = await repository.GetInboundAsync(inboundDetailDto.JobNo);
                if (inbound == null)
                {
                    return new NotFoundResult<bool>(new JsonResultError("RecordNotFound").ToJson());
                }

                //step 2 : Load Part Master & Supplier Master AU: part master is not really used 
                var supplierMaster = await repository.GetSupplierMasterAsync(inbound.CustomerCode, inbound.SupplierID);
                if (supplierMaster == null)
                {
                    return new InvalidResult<bool>(new JsonResultError("RecordNotFound").ToJson());
                }

                var entity = new InboundDetail();
                mapper.Map(inboundDetailDto, entity);

                var validateEntryResult = ValidateInboundDetailQuantites(entity, inboundDetailDto.QtyPerPkg, inboundDetailDto.PartMasterContainerFactor);
                if (validateEntryResult.ResultType != ResultType.Ok)
                    return validateEntryResult;

                entity.CreatedBy = userCode;
                entity.ControlDate = DateTime.Now;
                entity.LineItem = await GetNextLineItem(entity.JobNo);

                var addResult = await AddNewInboundDetail(inbound, entity, supplierMaster, supplierMaster.IsVMI);
                if (addResult.ResultType != ResultType.Ok)
                    return new InvalidResult<bool>(addResult.Errors[0]);

                await repository.AddInboundDetailAsync(entity);
                await repository.BatchAddStorageDetailAsync(addResult.Data);

                await repository.SaveChangesAsync();

                return new SuccessResult<bool>(true);
            });
        }

        public async Task<Result<InboundDetailDto>> UpdateInboundDetail(InboundDetailEntryModifyDto inboundDetailDto, string whsCode, string userCode)
        {
            return await WithTransactionScope<InboundDetailDto>(async () =>
            {
                // step 0 Cycle Count check:
                var cycleCheck = await HasAnyOutstandingCycleCountJobCycleCountItem(new string[] { inboundDetailDto.ProductCode }, whsCode);
                if (cycleCheck.ResultType != ResultType.Ok)
                {
                    return new InvalidResult<InboundDetailDto>(cycleCheck.Errors[0]);
                }

                //Step 3 :Update Inbound Detail
                var inboundDetail = await repository.GetInboundDetailAsync(inboundDetailDto.JobNo, inboundDetailDto.LineItem);
                mapper.Map(inboundDetailDto, inboundDetail, opts =>
                {
                    opts.AfterMap((src, dest) =>
                    {
                        if (appSettings.OwnerCode == "TTKL")
                        {
                            dest.ControlCode3 = src.ControlCode3;
                        }
                        dest.RevisedDate = DateTime.Now;
                        dest.RevisedBy = userCode;
                    });
                });

                var validateEntryResult = ValidateInboundDetailQuantites(inboundDetail, inboundDetailDto.QtyPerPkg, inboundDetailDto.PartMasterContainerFactor);
                if (validateEntryResult.ResultType != ResultType.Ok)
                    return new InvalidResult<InboundDetailDto>(validateEntryResult.Errors[0]);

                //step 1 : Load Inbound
                var inbound = await repository.GetInboundAsync(inboundDetailDto.JobNo);
                if (inbound == null)
                {
                    return new NotFoundResult<InboundDetailDto>(new JsonResultError("RecordNotFound").ToJson());
                }
                //step 2 : Load Part Master & Supplier Master AU PartMaster is not used anywhere so no need to get it
                var supplierMaster = await repository.GetSupplierMasterAsync(inbound.CustomerCode, inbound.SupplierID);
                if (supplierMaster == null)
                {
                    return new InvalidResult<InboundDetailDto>(new JsonResultError("RecordNotFound").ToJson());
                }

                //step 4 : Update Storagedetail 
                var storageDetails = await repository.StorageDetails()
                    .Where(s => s.InJobNo == inboundDetail.JobNo && s.LineItem == inboundDetail.LineItem).ToListAsync();
                foreach (var sd in storageDetails)
                {
                    sd.Status = StorageStatus.Cancelled;
                    sd.Qty = 0;
                }
                //step 5 :  Insert Into Storage Detail : AU: read the comment on the method to see why there are differences
                var storageDetailResult = await AddStorageDetailForNewInboundDetail(inbound, supplierMaster, inboundDetail, supplierMaster.IsVMI, false);
                if (storageDetailResult.ResultType != ResultType.Ok)
                    return new InvalidResult<InboundDetailDto>(storageDetailResult.Errors[0]);

                await repository.BatchAddStorageDetailAsync(storageDetailResult.Data);
                await repository.SaveChangesAsync();

                return new SuccessResult<InboundDetailDto>((await repository.GetInboundDetailListWithPrice<InboundDetailDto>(inbound.JobNo, inboundDetail.LineItem)).FirstOrDefault());
            });
        }

        public async Task<Result<bool>> CancelInbound(string jobNo, string userCode)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                // Load Inbound instance
                var inbound = await repository.GetInboundAsync(jobNo);
                if (inbound == null)
                {
                    return new InvalidResult<bool>(new JsonResultError("RecordNotFound").ToJson());
                }
                if (inbound.Status != InboundStatus.NewJob)
                {
                    return new InvalidResult<bool>(new JsonResultError("InboundJobCannotBeCancelled__", "jobNo", jobNo).ToJson());
                }

                //No more delete of storagedetail, set status to 99 , qty = 0 (6may05)
                //Batch delete storagedetail 
                var storageDetails = await repository.StorageDetails().Where(s => s.InJobNo == jobNo).ToListAsync();
                foreach (var sd in storageDetails)
                {
                    sd.Status = StorageStatus.Cancelled;
                    sd.Qty = 0;
                }

                // Update Inbound status and cancellation date / user
                inbound.Status = InboundStatus.Cancelled;
                inbound.CancelledBy = userCode;
                inbound.CancelledDate = DateTime.Now;

                // Update ASNheader status and injobno
                if (inbound.TransType == InboundType.ASN)
                {
                    // Load ASNheader
                    var asnHeader = await repository.GetASNHeaderAsync(inbound.IRNo);
                    if (asnHeader == null)
                    {
                        return new InvalidResult<bool>(new JsonResultError("RecordNotFound").ToJson());
                    }

                    var asnDetails = await repository.ASNDetails()
                        .Where(a => a.InJobNo == jobNo && a.ASNNo == inbound.IRNo).ToListAsync();

                    foreach (var asnDetail in asnDetails)
                    {
                        asnDetail.InJobNo = "";
                        asnDetail.Status = asnDetail.PreImportStatus;
                    }

                    //Let it remain as IMP eventhough no lineitems imported
                    //User request to update the status to "NEW" when cancel inbound 15 May 2015 Nick
                    var hasAsnDetailsForOtherJobs = await repository.ASNDetails()
                        .Where(a => a.InJobNo != "" && a.InJobNo != jobNo && a.ASNNo == inbound.IRNo)
                        .AnyAsync();

                    if (!hasAsnDetailsForOtherJobs)
                    {
                        asnHeader.Status = "NEW";
                    }
                }
                await repository.SaveChangesAsync();
                return new SuccessResult<bool>(true);
            });
        }

        public async Task<Result<bool>> IncreasePkgQty(string jobNo, int lineItem, decimal qty, string userCode)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                //step 1 : Load Inbound
                var inbound = await repository.GetInboundAsync(jobNo);
                if (inbound == null)
                {
                    return new InvalidResult<bool>(new JsonResultError("RecordNotFound").ToJson());
                }

                //step 2 : Load Part Master AU not used
                //Step 2 : update  Inbound Detail
                var inboundDetail = await repository.GetInboundDetailAsync(jobNo, lineItem);
                if (inboundDetail == null)
                {
                    return new InvalidResult<bool>(new JsonResultError("RecordNotFound").ToJson());
                }

                // validation
                if (qty % (inboundDetail.Qty / inboundDetail.NoOfPackage) != 0)
                {
                    return new InvalidResult<bool>(new JsonResultError("ProvideValidEntryQtyDivisableByPackage").ToJson());
                }

                var originalNoOfPkg = inboundDetail.NoOfPackage;
                var qtyPerPkg = inboundDetail.Qty / inboundDetail.NoOfPackage;

                inboundDetail.Qty += qty;
                inboundDetail.NoOfPackage = (int)(inboundDetail.Qty / qtyPerPkg);
                inboundDetail.NoOfLabel = inboundDetail.NoOfPackage;
                inboundDetail.RevisedBy = userCode;
                inboundDetail.RevisedDate = DateTime.Now;

                await repository.SaveChangesAsync();

                //step 5 :  Insert Into Storage Detail
                var existingStorageDetail = repository.StorageDetails().Where(s => s.InJobNo == jobNo && s.LineItem == lineItem).First();
                var seqNo = utilityService.GetAutoNum(AutoNumTable.StorageDetail, jobNo, lineItem);

                var noOfNewPkg = inboundDetail.NoOfPackage - originalNoOfPkg;

                for (var currentSeqNo = seqNo; currentSeqNo < (noOfNewPkg + seqNo); currentSeqNo++)
                {
                    //Step 5.1 : Get the PID
                    //Step 5.2 : Insert into TT_PIDCode;
                    var pidNumber = await utilityService.GetNextPIDNumber();
                    if (String.IsNullOrEmpty(pidNumber))
                        return new InvalidResult<bool>(new JsonResultError("FailToRetrievePID__", "jobNo", inboundDetail.JobNo).ToJson());

                    var storageDetail = new StorageDetail
                    {
                        PID = pidNumber,
                        InJobNo = inboundDetail.JobNo,
                        LineItem = inboundDetail.LineItem,
                        SeqNo = currentSeqNo,
                        ParentID = "",
                        CustomerCode = inbound.CustomerCode,
                        ProductCode = inboundDetail.ProductCode,
                        InboundDate = inbound.ETA,
                        ControlDate = inboundDetail.ControlDate,
                        OriginalQty = inboundDetail.Qty / inboundDetail.NoOfLabel,
                        Qty = inboundDetail.Qty / inboundDetail.NoOfLabel,
                        QtyPerPkg = inboundDetail.Qty / inboundDetail.NoOfPackage,
                        NoOfLabel = inboundDetail.NoOfLabel,
                        Length = inboundDetail.Length,
                        Width = inboundDetail.Width,
                        Height = inboundDetail.Height,
                        NetWeight = inboundDetail.NetWeight,
                        GrossWeight = inboundDetail.GrossWeight,
                        ControlCode1 = inboundDetail.ControlCode1,
                        ControlCode2 = inboundDetail.ControlCode2,
                        ControlCode3 = inboundDetail.ControlCode3,
                        ControlCode4 = inboundDetail.ControlCode4,
                        ControlCode5 = inboundDetail.ControlCode5,
                        ControlCode6 = inboundDetail.ControlCode6,
                        WHSCode = inbound.WHSCode,
                        Status = StorageStatus.Incoming,
                        SupplierID = inbound.SupplierID,
                        IsVMI = existingStorageDetail.IsVMI,
                        BondedStatus = existingStorageDetail.BondedStatus,
                        Ownership = existingStorageDetail.Ownership,
                        BuyingPrice = existingStorageDetail.BuyingPrice ?? 0,
                        SellingPrice = existingStorageDetail.SellingPrice ?? 0
                    };
                    await repository.AddStorageDetailAsync(storageDetail);
                }
                return new SuccessResult<bool>(true);
            });
        }

        public async Task<Result<bool>> RemovePIDs(RemovePIDsDto data, string userCode)
        {
            return await WithTransactionScope(async () =>
            {
                //step 1 : Update Storagedetail
                var allStorageDetails = repository.StorageDetails()
                        .Where(sd => sd.InJobNo == data.JobNo && sd.LineItem == data.LineItem
                                 && sd.Status != StorageStatus.Cancelled && sd.Status != StorageStatus.ZeroOut)
                        .ToList();

                var dataToCancel = data.RemoveAll ? allStorageDetails : allStorageDetails.Where(sd => data.PIDs.Contains(sd.PID));
                foreach (var sd in dataToCancel)
                {
                    sd.Status = StorageStatus.Cancelled;
                    sd.Qty = 0;
                }

                //step 2 : Get summary from Storage Detail
                //l_oStorageController.GetStorageDetailSummary(ref l_oFilter, ref l_dstStorageDetail, ref l_oInnerCon); replaced with:
                var storageDetailQuantities = allStorageDetails
                    .Where(s => s.Status != StorageStatus.Cancelled && s.Qty > 0)
                    .GroupBy(g => new { g.InJobNo, g.LineItem, g.ProductCode, g.QtyPerPkg })
                    .Select(i => new
                    {
                        i.Key.ProductCode,
                        i.Key.QtyPerPkg,
                        i.Key.LineItem,
                        TotalQty = i.Sum(s => s.Qty),
                        NoOfPkg = i.Select(s => s.LineItem).Count()
                    }).ToList();

                var inboundDetail = await repository.GetInboundDetailAsync(data.JobNo, data.LineItem);
                if (inboundDetail == null)
                {
                    return new NotFoundResult<bool>(new JsonResultError("RecordNotFound").ToJson());
                }
                bool completeInbound = false;
                if (storageDetailQuantities.Count == 1)
                {
                    //step 1 : Update Storagedetail (No of label)
                    foreach (var sd in allStorageDetails.Where(s => s.Status != StorageStatus.Cancelled))
                    {
                        sd.NoOfLabel = storageDetailQuantities.First().NoOfPkg;
                    }

                    if (storageDetailQuantities.First().TotalQty != 0)
                    {
                        inboundDetail.NoOfLabel = storageDetailQuantities.First().NoOfPkg;
                        inboundDetail.Qty = storageDetailQuantities.First().TotalQty;
                        inboundDetail.NoOfPackage = storageDetailQuantities.First().NoOfPkg;
                        inboundDetail.RevisedBy = userCode;
                        inboundDetail.RevisedDate = DateTime.Now;
                        completeInbound = true;
                    }
                    else
                    {
                        return new InvalidResult<bool>(new JsonResultError("InvalidInboundDetailQty").ToJson());
                    }
                }
                else if (!storageDetailQuantities.Any())
                {
                    await repository.DeleteInboundDetailAsync(inboundDetail);
                    // check remaining lines in storage
                    bool anyStorageDetailsExist = repository.StorageDetails()
                        .Where(sd => sd.InJobNo == data.JobNo && sd.Status != StorageStatus.Cancelled && sd.Status != StorageStatus.ZeroOut)
                        .Any();

                    if (anyStorageDetailsExist)
                    {
                        completeInbound = true;
                    }
                    else
                    {
                        var inbound = await repository.GetInboundAsync(data.JobNo);
                        inbound.Status = InboundStatus.NewJob;
                    }
                }
                else
                {
                    return new InvalidResult<bool>(new JsonResultError("InvalidStorageDetailSummary").ToJson());
                }
                await repository.SaveChangesAsync();

                if (completeInbound)
                {
                    var completeResult = await CompleteInbound(data.JobNo, userCode);
                    if (completeResult.ResultType != ResultType.Ok)
                        return completeResult;
                }
                return new SuccessResult<bool>(true);
            });
        }

        public async Task<IEnumerable<InboundIDTListItemDto>> GetIDT(string jobNo)
        {
            return (await repository.GetInboundIDTList<InboundIDTListItemDto>(jobNo));
        }

        #region Reports

        public async Task<Stream> LocationReport(string whsCode, string jobNo)
        {
            var fileName = "InboundLocationReport.rpt";
            var formula = $"{{TT_Inbound.JobNo}} = '{jobNo}' AND {{TT_StorageDetail.WHSCode}} = '{whsCode}'";
            formula += $" AND {{TT_StorageDetail.Status}} <> {(int)StorageStatus.Cancelled}";
            formula += $" AND {{TT_StorageDetail.Status}} <> {(int)StorageStatus.ZeroOut}";
            var title = $"Inbound Location Report - {jobNo}";
            return await Task.Run(() => reportService.GenerateReport(fileName, whsCode, title, formula));
        }

        public async Task<Stream> InboundReport(string whsCode, string jobNo)
        {
            var fileName = "InboundReport.rpt";
            var title = $"Inbound Report - {jobNo}";
            var formula = $"{{TT_Inbound.JobNo}} = '{jobNo}' AND {{TT_Inbound.WHSCode}} = '{whsCode}'";
            return await Task.Run(() => reportService.GenerateReport(fileName, whsCode, title, formula));
        }

        public async Task<Stream> WarehouseInNoteReport(string whsCode, string jobNo)
        {
            var fileName = "WarehouseInNote.rpt";
            var title = $"Warehouse IN Note - {jobNo}";
            var formula = $"{{TT_Inbound.JobNo}} = '{jobNo}'";
            return await Task.Run(() => reportService.GenerateReport(fileName, whsCode, title, formula));
        }

        public async Task<Result<Stream>> DiscrepancyReport(string whsCode, string jobNo)
        {
            var inbound = await repository.GetInboundAsync(jobNo);
            if (inbound == null)
                return new NotFoundResult<Stream>(new JsonResultError("RecordNotFound").ToJson());
            if (inbound.TransType == InboundType.ASN)
            {
                var fileName = "InboundDiscrepancyReport.rpt";
                var title = $"Inbound Discrepancy Report - {jobNo}";
                var formula = $"{{TT_Inbound.JobNo}} = '{jobNo}'";
                var reportStream = await Task.Run(() => reportService.GenerateReport(fileName, whsCode, title, formula));
                return new SuccessResult<Stream>(reportStream);
            }
            else
                return new InvalidResult<Stream>(new JsonResultError("DiscrepancyReport_PrintErrorNotASN").ToJson());
        }

        public async Task<Stream> GetOutstandingInboundsXlsReport(string whsCode)
        {
            var data = await repository.GetOutstandingInboundForReport(whsCode);
            return xlsService.WriteOutstandingInboundsToXls(data);
        }
        #endregion


        private async Task<Result<bool>> CompleteInbound(string jobNo, string userCode)
        {
            #region step 2.4 : Check whether all items have been putaway
            var allStorageDetails = await repository.StorageDetails()
                .Where(sd => sd.InJobNo == jobNo && sd.Status != StorageStatus.Cancelled && sd.Status != StorageStatus.ZeroOut)
                .ToListAsync();

            var storageDetailsWithoutLocation = allStorageDetails.Where(s => s.LocationCode == "").ToList();

            #endregion

            var inbound = await repository.GetInboundAsync(jobNo);

            #region step 2.5 : Finalizing the upload for a job

            if (storageDetailsWithoutLocation.Any())
            {
                #region step 2.4.1 : Update Inbound if Partial Putaway  
                if (allStorageDetails.Count != storageDetailsWithoutLocation.Count && inbound.Status != InboundStatus.PartialPutaway)
                {
                    inbound.Status = InboundStatus.PartialPutaway;
                    await repository.SaveChangesAsync();
                }
                #endregion
            }
            else
            {
                #region step 2.4.2 : Close Transaction if completed
                var supplierMaster = await repository.GetSupplierMasterAsync(inbound.CustomerCode, inbound.SupplierID);

                #region step 4.2.b.2 : Get Inbound Detail and Update Inventory
                var inboundQtiesGroupedByProductCode = await repository.GetInboundDetailGroupByProductCode(jobNo);

                if (inboundQtiesGroupedByProductCode.Any())
                {
                    var ownership = Ownership.Supplier;
                    if (supplierMaster.SAPVendorType == (int)SAPVendorType.OwnStock || supplierMaster.SupplierID == UtilityService.STR_EHP_SUPPLIER_ID)
                        ownership = Ownership.EHP;

                    foreach (var inboundGroupRow in inboundQtiesGroupedByProductCode)
                    {
                        #region step a : Update Inventory
                        var inventory = await repository.GetInventoryAsync(inbound.CustomerCode, inbound.SupplierID,
                            inboundGroupRow.ProductCode, inbound.WHSCode, ownership);
                        if (inventory == null)
                        {
                            return new InvalidResult<bool>(new JsonResultError("RecordNotFound").ToJson());
                        }
                        inventory.OnHandQty += inboundGroupRow.TotalQty;
                        inventory.OnHandPkg += inboundGroupRow.TotalPkg;
                        await repository.SaveChangesAsync();
                        #endregion

                        #region step b : Insert into InvTransaction, step c : Insert into InvTransactionPerWHS, step d : Insert into InvTransactionPerSupplier
                        await CompleteInboundAddTransactions(inbound, inboundGroupRow, ownership);
                        #endregion
                    }
                }
                #endregion

                #region step 4.2.b.3 : Update StorageDetail Status
                await CompleteInboundUpdateIncomingStorageStatus(jobNo);
                #endregion

                #region Step 4.2.b.4 : (Customised Function) Update Quarantine Qty if putaway to Quarantine location

                var updateQuarantResult = await CompleteInboundUpdateQuarantined(jobNo, userCode);
                if (updateQuarantResult.ResultType != ResultType.Ok)
                    return updateQuarantResult;
                #endregion

                #region step 4.2.b.5 : Insert to billing log

                if (inbound.TransType == InboundType.Return && supplierMaster.IsVMI && inboundQtiesGroupedByProductCode.Any())
                {
                    foreach (var inboundQty in inboundQtiesGroupedByProductCode)
                    {
                        await billingService.WriteToBillingLog(
                            inbound.JobNo,
                            inbound.CustomerCode,
                            inbound.SupplierID,
                            inboundQty.ProductCode,
                            inbound.RefNo,
                            inboundQty.TotalQty * -1);
                    }
                }
                #endregion
                #region step 4.2.b.6 : Update Inbound's Status
                inbound.Status = InboundStatus.Completed;
                inbound.PutawayBy = userCode;
                inbound.PutawayDate = DateTime.Now;
                #endregion

                #region step 4.2.b.7 : Update ASNHeader's Status
                if (inbound.TransType == InboundType.ASN)
                {
                    await UpdateASNStatus(inbound.IRNo, inbound.JobNo);
                }
                #endregion

                await repository.SaveChangesAsync();
                #endregion

                iLogConnect.InboundCompleted(jobNo);
            }
            #endregion


            return new SuccessResult<bool>(true);
        }

        private async Task CompleteInboundAddTransactions(Inbound inbound, InboundQtyByProductCodeQueryResult inboundGroupRow, Ownership ownership)
        {
            #region step b : Insert into InvTransaction
            var lastTrans = await repository.GetInventoryLastTransactionBalance(inbound.CustomerCode, inboundGroupRow.ProductCode);
            var invTransaction = new InvTransaction
            {
                JobNo = inbound.JobNo,
                ProductCode = inboundGroupRow.ProductCode,
                CustomerCode = inbound.CustomerCode,
                JobDate = inbound.CreatedDate,
                Act = (int)InventoryTransactionType.Inbound,
                Qty = Convert.ToDouble(inboundGroupRow.TotalQty),
                Pkg = inboundGroupRow.TotalPkg,
                BalanceQty = Convert.ToDouble(inboundGroupRow.TotalQty) + (lastTrans?.BalanceQty ?? 0),
                BalancePkg = inboundGroupRow.TotalPkg + (lastTrans?.BalancePkg ?? 0)
            };
            await repository.AddInvTransactionAsync(invTransaction);

            #endregion

            #region step c : Insert into InvTransactionPerWHS
            var lastWHSTrans = await repository.GetInventoryLastTransactionPerWHSBalance(inbound.CustomerCode, inboundGroupRow.ProductCode, inbound.WHSCode);
            var invTransactionPerWHS = new InvTransactionPerWHS
            {
                JobNo = inbound.JobNo,
                ProductCode = inboundGroupRow.ProductCode,
                WHSCode = inbound.WHSCode,
                CustomerCode = inbound.CustomerCode,
                JobDate = inbound.CreatedDate,
                Act = (int)InventoryTransactionType.Inbound,
                Qty = Convert.ToDouble(inboundGroupRow.TotalQty),
                Pkg = inboundGroupRow.TotalPkg,
                BalanceQty = Convert.ToDouble(inboundGroupRow.TotalQty) + (lastWHSTrans?.BalanceQty ?? 0),
                BalancePkg = inboundGroupRow.TotalPkg + (lastWHSTrans?.BalancePkg ?? 0)
            };
            await repository.AddInvTransactionPerWHSAsync(invTransactionPerWHS);
            #endregion

            #region step d : Insert into InvTransactionPerSupplier

            var lastSupplierTrans = await repository.GetInventoryLastTransactionPerSupplierBalance(inbound.CustomerCode, inboundGroupRow.ProductCode, inbound.SupplierID, ownership);
            var invTransactionPerSupplier = new InvTransactionPerSupplier
            {
                JobNo = inbound.JobNo,
                ProductCode = inboundGroupRow.ProductCode,
                SupplierID = inbound.SupplierID,
                CustomerCode = inbound.CustomerCode,
                JobDate = inbound.CreatedDate,
                Act = (int)InventoryTransactionType.Inbound,
                Qty = inboundGroupRow.TotalQty,
                Ownership = ownership,
                BalanceQty = inboundGroupRow.TotalQty + Convert.ToDecimal(lastSupplierTrans ?? 0)
            };
            await repository.AddInvTransactionPerSupplierAsync(invTransactionPerSupplier);
            #endregion

        }

        private async Task CompleteInboundUpdateIncomingStorageStatus(string jobNo)
        {
            #region step 4.2.b.3 : Update StorageDetail Status
            var storageForPutawayStatus = repository.StorageDetails()
                .Where(s => s.InJobNo == jobNo && s.Status == StorageStatus.Incoming)
                .ToList();
            if (storageForPutawayStatus.Any())
            {
                foreach (var sd in storageForPutawayStatus)
                {
                    sd.Status = StorageStatus.Putaway;
                }
                await repository.SaveChangesAsync();
            }
            #endregion
        }

        private async Task<Result<bool>> CompleteInboundUpdateQuarantined(string jobNo, string userCode)
        {
            var quarantinedStorage = await repository.GetStorageDetailForLocationType(LocationType.Quarantine, jobNo);

            if (quarantinedStorage.Any())
            {
                #region Step 4.2.b.4.1 : Create Quarantine Job No
                var jobNoResult = await utilityService.GenerateJobNo(JobType.Quarantine);
                if (jobNoResult.ResultType != ResultType.Ok)
                {
                    return new InvalidResult<bool>(new JsonResultError("FailToCreateQuarantineJobNo").ToJson());
                }
                var quarantineJobNo = jobNoResult.Data;
                #endregion

                #region Step 4.2.b.4.2 : Insert into Quarantine Log
                foreach (var quarantinedSD in quarantinedStorage)
                {
                    #region Step 4.2.b.4.2.1 : Get Storage Detail Instance
                    var sd = await repository.GetStorageDetailAsync(quarantinedSD.PID);
                    #endregion

                    #region Step 4.2.b.4.2.2 : Get Inventory Instance
                    var inventory = await repository.GetInventoryAsync(quarantinedSD.CustomerCode, quarantinedSD.SupplierID, quarantinedSD.ProductCode, quarantinedSD.WHSCode, quarantinedSD.Ownership);
                    if (inventory == null)
                    {
                        return new InvalidResult<bool>(new JsonResultError("FailToRetrieveInventoryInstanceForProductCode__", "productCode", quarantinedSD.ProductCode).ToJson());
                    }
                    #endregion

                    #region Step 4.2.b.4.2.3 : Update Quarantine Pkg and Qty to Invetory
                    inventory.QuarantineQty += sd.Qty;
                    inventory.QuarantinePkg += Convert.ToInt32(Math.Ceiling(sd.Qty / sd.QtyPerPkg));
                    #endregion

                    #region Step 4.2.b.4.2.4 : Update <Status> Storage Detail
                    sd.Status = StorageStatus.Quarantine;
                    await repository.SaveChangesAsync();
                    #endregion

                    #region Step 4.2.b.4.2.5 : Get Next Line Item
                    var quarantineLineItem = utilityService.GetAutoNum(AutoNumTable.QuarantineLog, quarantineJobNo);
                    #endregion

                    #region Step 4.2.b.4.2.6 : Insert into Log
                    var quarantineLog = new QuarantineLog
                    {
                        JobNo = quarantineJobNo,
                        LineItem = quarantineLineItem,
                        PID = sd.PID,
                        Act = (int)QuarantineType.Onhold,
                        CreatedBy = userCode,
                        CreatedDate = DateTime.Now
                    };

                    await repository.AddQuarantineLogAsync(quarantineLog);

                    #endregion
                }
                #endregion

                iLogConnect.QuarantineJobCreated(quarantineJobNo);
            }
            return new SuccessResult<bool>(true);
        }

        private async Task UpdateASNStatus(string asnNo, string jobNo)
        {
            #region step 4.2.b.7 : Update ASNHeader's Status
            var asnDetails = repository.ASNDetails().Where(a => a.InJobNo == jobNo && a.ASNNo == asnNo).ToList();

            foreach (var asnDetail in asnDetails)
            {
                asnDetail.Status = "REC";
            }
            //Query for l_oInboundCtrl.GetASNDetailOutstandingList(ref l_oFilter, ref l_dtsASNDetail, ref l_oInnerCon);
            var hasOutstandingASNs = repository.ASNDetails()
                .Where(a => a.InJobNo != jobNo && a.ASNNo == asnNo && a.Status != "REC").Any();

            if (!hasOutstandingASNs)
            {
                var asnHeader = await repository.GetASNHeaderAsync(asnNo);
                asnHeader.Status = "REC";
            }
            #endregion
        }

        private Result<bool> ValidateInboundDetailQuantites(InboundDetail dto, decimal qtyPerPackage, int partMasterContainerFactor)
        {
            if (qtyPerPackage == 0 || (dto.Qty % qtyPerPackage) != 0)
                return new InvalidResult<bool>(new JsonResultError("ProvideValidEntryQtyDivisableToQty").ToJson());

            if (dto.NoOfPackage == 0 || ((dto.Qty * partMasterContainerFactor) % dto.NoOfPackage) != 0)
                return new InvalidResult<bool>(new JsonResultError("ProvideValidEntryQtyDivisableByPackage").ToJson());

            if ((dto.Qty * partMasterContainerFactor) % dto.NoOfLabel != 0)
                return new InvalidResult<bool>(new JsonResultError("ProvideValidEntryQtyDivisableByLabel").ToJson());
            return new SuccessResult<bool>(true);
        }

        private InboundDetail CreateInboundDetailFromASNDetail(ASNDetailWithSPQQueryResult asnDetailRow, string jobNo, string userCode)
        {
            return new InboundDetail()
            {
                JobNo = jobNo,
                ProductCode = asnDetailRow.ProductCode,

                PackageType = asnDetailRow.PackageType,
                Length = asnDetailRow.Length,
                Width = asnDetailRow.Width,
                Height = asnDetailRow.Height,
                NetWeight = asnDetailRow.NetWeight,
                GrossWeight = asnDetailRow.GrossWeight,

                ControlCode1 = asnDetailRow.ASNDetail.BatchNo ?? string.Empty,
                ControlCode2 = asnDetailRow.ASNDetail.ManufacturedDate.HasValue ? String.Format("{0:dd MMM yyyy}", asnDetailRow.ASNDetail.ManufacturedDate) : string.Empty,
                //ControlCode3 = "",
                //ControlCode4 = "",
                //ControlCode5 = "",
                //ControlCode6 = "",
                ControlDate = DateTime.Now,
                CreatedBy = userCode,
                ASNNo = asnDetailRow.ASNDetail.ASNNo,
                ASNLineItem = asnDetailRow.ASNDetail.LineItem
            };
        }

        private async Task<Result<IEnumerable<StorageDetail>>> AddNewInboundDetail(Inbound inbound, InboundDetail inboundDetail, SupplierMaster supplierMaster, bool isVMI)
        {
            var storageDetailResult = await AddStorageDetailForNewInboundDetail(inbound, supplierMaster, inboundDetail, isVMI);
            if (storageDetailResult.ResultType != ResultType.Ok)
                return storageDetailResult;

            return new SuccessResult<IEnumerable<StorageDetail>>(storageDetailResult.Data);
        }

        private async Task<int> GetNextLineItem(string inJobNo)
        {
            var lastLine = await repository.InboundDetails().Where(i => i.JobNo == inJobNo)
                .OrderByDescending(i => i.LineItem).Select(i => i.LineItem).FirstOrDefaultAsync();
            return lastLine == 0 ? 1 : (lastLine + 1);
        }

        /// <summary>
        /// this method is compilation of code in 2 methods:
        /// - AddNewInboundDetail
        /// - ModifyInboundDetail
        /// The only difference that was there was slight change in PID generation (by auto method, or consecutive pids by adding suffix-this should have no difference
        /// and the part concerning Hungary check (only in AddNewInboundDetail)
        /// </summary>
        /// <param name="inbound"></param>
        /// <param name="inboundDetail"></param>
        /// <param name="isVMI"></param>
        /// <param name="checkForHungary"></param>
        /// <returns></returns>
        private async Task<Result<IEnumerable<StorageDetail>>> AddStorageDetailForNewInboundDetail(Inbound inbound, SupplierMaster supplierMaster, InboundDetail inboundDetail, bool isVMI, bool checkForHungary = true)
        {
            //Step 3 : Get Line Item - AU: this is already done when creating the inboundDetail object; must be done before if we need to calculate it

            //step 5 :  Insert Into Storage Detail
            //Step 5.1 : Get the PID
            var pidNumber = await utilityService.GetNextPIDNumber(false);
            if (String.IsNullOrEmpty(pidNumber))
                return new InvalidResult<IEnumerable<StorageDetail>>(new JsonResultError("FailToRetrievePID__", "jobNo", inboundDetail.JobNo).ToJson());

            var storageDetails = new List<StorageDetail>();
            var pidNos = new List<string>();

            for (var count = 1; count <= inboundDetail.NoOfLabel; count++)
            {
                if (count > 1)
                    pidNumber = await utilityService.GetNextPIDNumber(pidNumber, false);

                pidNos.Add(pidNumber);

                var storageDetail = new StorageDetail
                {
                    PID = pidNumber,
                    InJobNo = inboundDetail.JobNo,
                    LineItem = inboundDetail.LineItem,
                    SeqNo = count,
                    ParentID = "",
                    CustomerCode = inbound.CustomerCode,
                    ProductCode = inboundDetail.ProductCode,
                    InboundDate = inbound.ETA,
                    ControlDate = inboundDetail.ControlDate,
                    OriginalQty = inboundDetail.Qty / inboundDetail.NoOfLabel,
                    Qty = inboundDetail.Qty / inboundDetail.NoOfLabel,
                    NoOfLabel = inboundDetail.NoOfLabel,
                    Length = inboundDetail.Length,
                    Width = inboundDetail.Width,
                    Height = inboundDetail.Height,
                    NetWeight = inboundDetail.NetWeight,
                    GrossWeight = inboundDetail.GrossWeight,
                    ControlCode1 = inboundDetail.ControlCode1,
                    ControlCode2 = inboundDetail.ControlCode2,
                    ControlCode3 = inboundDetail.ControlCode3,
                    ControlCode4 = inboundDetail.ControlCode4,
                    ControlCode5 = inboundDetail.ControlCode5,
                    ControlCode6 = inboundDetail.ControlCode6,
                    QtyPerPkg = inboundDetail.Qty / inboundDetail.NoOfPackage,
                    WHSCode = inbound.WHSCode,
                    Status = (int)StorageStatus.Incoming,
                    SupplierID = inbound.SupplierID,
                    IsVMI = (byte)(isVMI ? 1 : 0),
                    BondedStatus = supplierMaster.IsBonded,
                    BuyingPrice = 0,
                    SellingPrice = 0
                };

                if (appSettings.OwnerCode.StartsWith(UtilityService.TESA_CODE))
                {
                    if (supplierMaster.SAPVendorType == (int)SAPVendorType.OwnStock)
                    {
                        storageDetail.Ownership = Ownership.EHP;
                        storageDetail.BondedStatus = (int)BondedStatus.NonBonded;
                    }
                    else if (inbound.TransType == InboundType.Excess)
                    {
                        storageDetail.Ownership = Ownership.EHP;
                        storageDetail.BondedStatus = (int)BondedStatus.NonBonded;
                    }
                    else
                    {
                        #region Check if it is Hungary Special - SubContract Inbound as EHP Stock for Storage Location T002
                        var subContractInbound = false;
                        if (checkForHungary && inbound.CustomerCode.Substring(0, 2) == "HU")
                        {
                            subContractInbound = (from epo in repository.EPOs()
                                                  join asnDetail in repository.ASNDetails() on new { epo.PONo, epo.POLineItem }
                                                       equals new { asnDetail.PONo, POLineItem = asnDetail.POLineNo }
                                                  where asnDetail.ASNNo == inboundDetail.ASNNo
                                                       && asnDetail.LineItem == inboundDetail.LineItem
                                                       && epo.SAPLocationID == "T002"
                                                  select epo.SAPLocationID).Any();
                        }
                        #endregion

                        if (!subContractInbound)
                        {
                            storageDetail.Ownership = Ownership.Supplier;
                        }
                        else
                        {
                            storageDetail.Ownership = Ownership.EHP;
                            storageDetail.BondedStatus = (int)BondedStatus.NonBonded;
                            storageDetail.IsVMI = 0;
                        }
                    }
                }
                else
                {
                    if (supplierMaster.SupplierID == UtilityService.STR_EHP_SUPPLIER_ID)
                        storageDetail.Ownership = Ownership.EHP;
                    else if (inbound.TransType == InboundType.Excess)
                        storageDetail.Ownership = Ownership.EHP;
                    else
                        storageDetail.Ownership = Ownership.Supplier;
                }
                storageDetails.Add(storageDetail);
                //await repository.AddStorageDetailAsync(storageDetail, false);
            }

            await repository.BatchAddPIDCodeAsync(pidNos.Select(p => new PIDCode { PIDNo = p }).ToList());
            return new SuccessResult<IEnumerable<StorageDetail>>(storageDetails);
        }

        /// <summary>
        /// original code: InboundEntry.CheckCycleCountLock()
        /// </summary>
        /// <param name="productCodes"></param>
        /// <param name="whsCode"></param>
        /// <returns></returns>
        private async Task<Result<bool>> HasAnyOutstandingCycleCountJobCycleCountItem(IEnumerable<string> productCodes, string whsCode)
        {
            #region Step 2 : Get Outstanding Cycle Count Detail
            var jobNos = await repository.GetCycleCountJobNos(productCodes, whsCode);
            if (jobNos.Any())
            {
                return new InvalidResult<bool>(new JsonResultError("UnableToAddOutstandingCycleCount__", "jobNos", string.Join(", ", jobNos)).ToJson());
            }
            return new SuccessResult<bool>(true);
            #endregion
        }

        private readonly ITTLogixRepository repository;
        private readonly IILogConnect iLogConnect;
        private readonly IMapper mapper;
        private readonly IUtilityService utilityService;
        private readonly IReportService reportService;
        private readonly IBillingService billingService;
        private readonly IXlsService xlsService;
        private readonly AppSettings appSettings;
    }
}
