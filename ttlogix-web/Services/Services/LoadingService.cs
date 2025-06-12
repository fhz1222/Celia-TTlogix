using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using ServiceResult;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TT.Common;
using TT.Common.Extensions;
using TT.Core.Entities;
using TT.Core.Enums;
using TT.Core.Interfaces;
using TT.Core.QueryFilters;
using TT.Services.Interfaces;
using TT.Services.Models;
using TT.Services.Services.Utilities;
using TT.Services.Utilities;

namespace TT.Services.Services
{
    public class LoadingService : ServiceBase<LoadingService>, ILoadingService
    {
        public LoadingService(ITTLogixRepository repository,
            ILocker locker,
            ILogger<LoadingService> logger,
            IMapper mapper,
            IUtilityService utilityService,
            IOptions<AppSettings> appSettings,
            IOutboundService outboundService,
            IReportService reportService,
            ILoggerService loggerService) : base(locker, logger)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.utilityService = utilityService;
            this.outboundService = outboundService;
            this.reportService = reportService;
            this.loggerService = loggerService;
            this.appSettings = appSettings.Value;
        }

        public async Task<LoadingListDto> GetLoadingList(LoadingListQueryFilter filter)
        {
            var query = repository.GetLoadingList<LoadingListItemDto>(filter);

            var pagedQuery = query.Skip(filter.PageSize * (filter.PageNo - 1)).Take(filter.PageSize);
            var total = await query.CountAsync();
            var data = await pagedQuery.ToListAsync();

            return new LoadingListDto
            {
                Data = data,
                PageSize = filter.PageSize,
                PageNo = filter.PageNo,
                Total = total
            };
        }

        public async Task<IEnumerable<LoadingEntryDto>> GetLoadingEntryList(string customerCode, string warehouse)
        {
            var isSAP = appSettings.IsSAPFactory(customerCode);
            return await repository.GetLoadingEntryList<LoadingEntryDto>(customerCode, warehouse, isSAP, null);
        }

        public async Task<Result<bool>> SetTruckArrival(string jobNo)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                var loading = await repository.GetLoadingAsync(jobNo);
                if (loading == null)
                    return new InvalidResult<bool>(new JsonResultError("UnableToRetrieveLoadingInstance").ToJson());
                loading.TruckArrivalDate = DateTime.Now;
                await repository.SaveChangesAsync();
                return new SuccessResult<bool>(true);
            });
        }

        public async Task<Result<bool>> SetTruckDeparture(string jobNo)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                var loading = await repository.GetLoadingAsync(jobNo);
                if (loading == null)
                    return new InvalidResult<bool>(new JsonResultError("UnableToRetrieveLoadingInstance").ToJson());
                loading.TruckDepartureDate = DateTime.Now;
                await repository.SaveChangesAsync();
                return new SuccessResult<bool>(true);
            });
        }

        public async Task<LoadingDto> GetLoading(string jobNo)
        {
            var entity = await repository.GetLoadingAsync(jobNo);
            if (entity != null)
            {
                return await MapLoadingDto(entity);
            }
            return null;
        }

        public async Task<IEnumerable<LoadingDetailDto>> GetLoadingDetails(string jobNo)
        {
            return await repository.GetLoadingDetailList<LoadingDetailDto>(jobNo);
        }

        public async Task<Result<LoadingDto>> CreateLoading(LoadingAddDto loadingDto, string userCode)
        {
            return await WithTransactionScope<LoadingDto>(async () =>
            {
                var loadingObjResult = await CreateLoadingObject(loadingDto, userCode);
                if (loadingObjResult.ResultType != ResultType.Ok)
                {
                    return new InvalidResult<LoadingDto>(loadingObjResult.Errors[0]);
                }
                var dto = await MapLoadingDto(loadingObjResult.Data);
                return new SuccessResult<LoadingDto>(dto);
            });
        }

        public async Task<Result<LoadingDto>> CreateLoadingFromOutbound(AddLoadingFromOutboundDto loadingDto, string userCode, string warehouse)
        {
            return await WithTransactionScope<LoadingDto>(async () =>
            {
                // validate outbounds
                var outbounds = await repository.Outbounds().Where(o => loadingDto.OutJobNos.Contains(o.JobNo)).ToListAsync();
                if (outbounds.Any(o => o.TransType == OutboundType.Return))
                    return new InvalidResult<LoadingDto>(new JsonResultError("CannotGenerateLoadingDetailsFromSelectedOutbound_RETURN").ToJson());

                if (outbounds.Any(o => o.Status == OutboundStatus.Cancelled))
                    return new InvalidResult<LoadingDto>(new JsonResultError("CannotGenerateLoadingDetailsFromSelectedOutbound_CANCELLED").ToJson());

                var isSAP = appSettings.IsSAPFactory(loadingDto.Loading.CustomerCode);
                var entries = await repository.GetLoadingEntryListFromOutbound<LoadingEntryDto>(loadingDto.Loading.CustomerCode, warehouse, isSAP, loadingDto.OutJobNos);
                if (!entries.Any() || entries.Count() != outbounds.Count)
                    return new InvalidResult<LoadingDto>(new JsonResultError("CannotGenerateLoadingDetailsFromSelectedOutbound").ToJson());

                var loadingObjResult = await CreateLoadingObject(loadingDto.Loading, userCode);
                if (loadingObjResult.ResultType != ResultType.Ok)
                {
                    return new InvalidResult<LoadingDto>(loadingObjResult.Errors[0]);
                }
                var entity = loadingObjResult.Data;

                // add detail rows from outbound details
                var addEntriesResult = await CreateLoadingRows(entries, entity, userCode);
                if (addEntriesResult.ResultType != ResultType.Ok)
                    return new InvalidResult<LoadingDto>(addEntriesResult.Errors[0]);

                var dto = await MapLoadingDto(entity);
                return new SuccessResult<LoadingDto>(dto);
            });
        }

        public async Task<Result<LoadingDto>> UpdateLoading(string jobNo, LoadingDto loadingDto, string userCode)
        {
            return await WithTransactionScope<LoadingDto>(async () =>
            {
                var entity = await repository.GetLoadingAsync(jobNo);
                if (entity == null)
                {
                    return new NotFoundResult<LoadingDto>(new JsonResultError("RecordNotFound").ToJson());
                }
                var wasAllowedForDispatch = entity.AllowedForDispatch;
                mapper.Map(loadingDto, entity);

                if (wasAllowedForDispatch != entity.AllowedForDispatch)
                {
                    entity.AllowedForDispatchModifiedBy = userCode;
                    entity.AllowedForDispatchModifiedDate = DateTime.Now;
                }

                var loadingDetails = await repository.LoadingDetails().Where(l => l.JobNo == jobNo).ToListAsync();
                foreach (var ld in loadingDetails)
                {
                    //Step 2 : Insert Into Loading Detail
                    ld.ETD = entity.ETD;
                }

                var outJobNos = loadingDetails.Where(l => !String.IsNullOrWhiteSpace(l.OutJobNo)).Select(l => l.OutJobNo);

                if (appSettings.IsNotSAPFactory(entity.CustomerCode))
                {
                    await UpdateOutboundsWithETDDate(outJobNos, entity.ETD);
                }
                await repository.SaveChangesAsync();

                var dto = await MapLoadingDto(entity);
                return new SuccessResult<LoadingDto>(dto);
            });
        }

        public async Task<IEnumerable<string>> GetBondedStockJobNosWithoutCommInv(string jobNo)
        {
            return await repository.GetBondedStockJobNosWithoutCommInv(jobNo);
        }

        public async Task<Result<bool>> ConfirmLoading(string jobNo, string userCode)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                loggerService.LogInformation($"ConfirmLoading (jobNo {jobNo} - started");

                var loading = await repository.GetLoadingAsync(jobNo);
                if (loading?.AllowedForDispatch != true)
                {
                    return new InvalidResult<bool>(new JsonResultError("LoadingNotAllowedForDispatch").ToJson());
                }

                //Step 2 : Check for outbound job status
                var outboundListAll = await repository.GetLoadingOutboundQuery(jobNo).Select(i => new { i.JobNo, i.Status, i.TransType, i.RefNo, i.CustomerCode }).ToListAsync();

                var outboundListNotPicked = outboundListAll.Where(o => new OutboundStatus[]
                    {
                    OutboundStatus.NewJob,
                    OutboundStatus.PartialDownload,
                    OutboundStatus.Downloaded,
                    OutboundStatus.PartialPicked,
                    OutboundStatus.Cancelled
                }.Contains(o.Status));
                if (outboundListNotPicked.Any())
                {
                    return new InvalidResult<bool>(new JsonResultError("FollowingOutboundJobsNotPicked__", "jobNos", string.Join(",", outboundListNotPicked.Select(o => o.JobNo))).ToJson());
                }

                //Step 3 : Cargo Out
                var outboundListPicked = outboundListAll.Where(o => o.Status == OutboundStatus.Picked);
                loggerService.LogInformation($"ConfirmLoading (jobNo {jobNo} - outbounds to process: {outboundListPicked.Count()}");

                Result<bool> result = null;
                foreach (var pickedOutbound in outboundListPicked.GroupBy(g => g.TransType))
                {
                    switch (pickedOutbound.Key)
                    {
                        case OutboundType.EKanban:
                            if (appSettings.OwnerCode.StartsWith(UtilityService.TESA_CODE))
                            {
                                result = await outboundService.CompleteOutboundEurope(pickedOutbound.Select(o => o.JobNo), userCode, false);
                                if (result.ResultType == ResultType.Ok)
                                {
                                    var pickedTTKs = pickedOutbound
                                        .Where(outbound => appSettings.IsSAPFactory(outbound.CustomerCode) && !outbound.RefNo.StartsWith("TTK"))
                                        .Select(p => p.RefNo).ToList();

                                    if (pickedTTKs.Any())
                                    {
                                        await repository.EKanbanHeaderBatchUpdateStatus(EKanbanStatus.DataSent, false, pickedTTKs);
                                    }
                                }
                            }
                            else
                            {
                                foreach (var outbound in pickedOutbound)
                                {
                                    if (!outbound.RefNo.Equals("") && outbound.RefNo.IndexOf("TTK") == 0)
                                    {
                                        result = await outboundService.CompleteOutboundReturn(new[] { outbound.JobNo }, userCode, false);
                                    }
                                    else
                                    {
                                        result = await outboundService.CargoInTransit(new[] { outbound.JobNo }, userCode, false);
                                    }
                                }
                            }
                            break;
                        case OutboundType.Return:
                            result = await outboundService.CompleteOutboundReturn(pickedOutbound.Select(o => o.JobNo), userCode, false);
                            break;
                        case OutboundType.ManualEntry:
                        case OutboundType.ScannerManualEntry:
                            result = await outboundService.CompleteOutboundManual(pickedOutbound.Select(o => o.JobNo), userCode, false);
                            break;
                        case OutboundType.CrossDock:
                        case OutboundType.WHSTransfer:
                        default:
                            result = new InvalidResult<bool>(new JsonResultError("InvalidOutboundJobType").ToJson());
                            break;
                    }
                    if (result?.ResultType != ResultType.Ok)
                    {
                        return result ?? new InvalidResult<bool>(new JsonResultError("FailToCargoOut").ToJson());
                    }
                }

                //Step 4 : Check all outbound completed
                var outboundListAllAfterUpdate = await repository.GetLoadingOutboundQuery(jobNo).Select(i => new { i.JobNo, i.Status, i.RefNo, i.CustomerCode }).ToListAsync();
                var outboundListAnyOpen = outboundListAllAfterUpdate.Where(o => o.Status != OutboundStatus.Completed);
                if (outboundListAnyOpen.Any())
                {
                    return new InvalidResult<bool>(new JsonResultError("FollowingOutboundJobsNotCargoOut__", "jobNos", string.Join(",", outboundListAnyOpen.Select(o => o.JobNo))).ToJson());
                }

                var completedTTKs = outboundListAllAfterUpdate
                    .Where(outbound => appSettings.IsSAPFactory(outbound.CustomerCode) && !outbound.RefNo.StartsWith("TTK"))
                    .Select(p => p.RefNo).ToList();
                if (completedTTKs.Any())
                {
                    await repository.EKanbanHeaderBatchUpdateStatus(EKanbanStatus.DataSent, false, completedTTKs, EKanbanStatus.InTransit);
                }

                //step 5 : Complete Loading
                loading.Status = LoadingStatus.Confirmed;
                loading.ConfirmedBy = userCode;
                loading.ConfirmedDate = DateTime.Now;

                await repository.SaveChangesAsync();
                loggerService.LogInformation($"ConfirmLoading (jobNo {jobNo} - ended");

                return new SuccessResult<bool>(true);
            });
        }

        public async Task<Result<LoadingDto>> CancelLoading(string jobNo, string userCode)
        {
            return await WithTransactionScope<LoadingDto>(async () =>
            {
                var loading = await repository.GetLoadingAsync(jobNo);
                if (loading == null)
                {
                    return new NotFoundResult<LoadingDto>(new JsonResultError("RecordNotFound").ToJson());
                }
                if (loading.Status != LoadingStatus.NewJob && loading.Status != LoadingStatus.Processing)
                {
                    return new InvalidResult<LoadingDto>(new JsonResultError()
                    {
                        MessageKey = "LoadingJobCannotBeCancelled__",
                        Arguments = new Dictionary<string, string>()
                        {
                            {"jobNo", jobNo },
                            {"loadingStatus", loading.Status.ToString() }
                        }
                    }.ToJson());
                }

                if (repository.LoadingDetails().Any(l => l.JobNo == jobNo))
                {
                    return new InvalidResult<LoadingDto>(new JsonResultError("LoadingJobWithDetailsCannotBeCancelled__", "jobNo", jobNo).ToJson());
                }

                loading.Status = LoadingStatus.Cancelled;
                loading.CancelledBy = userCode;
                loading.CancelledDate = DateTime.Now;
                await repository.SaveChangesAsync();

                var dto = await MapLoadingDto(loading);
                return new SuccessResult<LoadingDto>(dto);
            });
        }

        public async Task<Result<bool>> DeleteLoadingDetails(DeleteLoadingDetailDto data, string userCode)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                //step 1 : Get Loading
                var loading = await repository.GetLoadingAsync(data.JobNo);

                if (loading.Status == LoadingStatus.Completed || loading.Status == LoadingStatus.Confirmed)
                {
                    return new InvalidResult<bool>(new JsonResultError("CannotAddOrRemoveOutboundsToLoadingJobInStatusCompleted").ToJson());
                }

                if (loading == null)
                {
                    return new NotFoundResult<bool>(new JsonResultError("RecordNotFound").ToJson());
                }
                
                foreach (var orderNo in data.OrderNos)
                {
                    var entity = await repository.GetLoadingDetailAsync(data.JobNo, orderNo);
                    if (entity == null)
                        return new NotFoundResult<bool>(new JsonResultError()
                        {
                            MessageKey = "LoadingDetailNotFoundForJobNoOrderNo__",
                            Arguments = new Dictionary<string, string>()
                                {
                                    {"jobNo", data.JobNo },
                                    {"orderNo", orderNo }
                                }
                        }.ToJson());

                    await repository.DeleteLoadingDetailAsync(entity);
                }

                if (appSettings.IsSAPFactory(loading.CustomerCode))
                {
                    await ChangeEKanbanStatusToDataSent(data.OrderNos);
                    await repository.SaveChangesAsync();
                }

                loading.Status = await RefreshLoadingStatus(data.JobNo);
                loading.RevisedBy = userCode;
                loading.RevisedDate = DateTime.Now;
                await repository.SaveChangesAsync();

                return new SuccessResult<bool>(true);
                
            });
        }

        public async Task<Result<bool>> CreateLoadingDetails(IEnumerable<string> orderNos, string jobNo, string userCode, string warehouse)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                // validate
                if (orderNos == null || orderNos.Any(i => String.IsNullOrWhiteSpace(i)))
                    return new InvalidResult<bool>(new JsonResultError("OrderNosCannotBeEmpty").ToJson());

                //step 1 : Get Loading
                var loading = await repository.GetLoadingAsync(jobNo);
                if (loading.Status == LoadingStatus.Completed || loading.Status == LoadingStatus.Confirmed)
                {
                    return new InvalidResult<bool>(new JsonResultError("CannotAddOrRemoveOutboundsToLoadingJobInStatusCompleted").ToJson());
                }

                var isSAP = appSettings.IsSAPFactory(loading.CustomerCode);
                var entries = await repository.GetLoadingEntryList<LoadingEntryDto>(loading.CustomerCode, warehouse, isSAP, orderNos);
                return await CreateLoadingRows(entries, loading, userCode);
                
            });
        }

        public async Task<Result<bool>> SetAllowForDispatch(string jobNo, bool allow, string userCode)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                //step 1 : Get Loading
                var loading = await repository.GetLoadingAsync(jobNo);
                if (loading == null)
                    return new NotFoundResult<bool>(new JsonResultError("RecordNotFound").ToJson());

                if (loading.AllowedForDispatch != allow)
                {
                    loading.AllowedForDispatch = allow;
                    loading.AllowedForDispatchModifiedBy = userCode;
                    loading.AllowedForDispatchModifiedDate = DateTime.Now;
                    await repository.SaveChangesAsync();
                }
                return new SuccessResult<bool>(loading.AllowedForDispatch);
            });
        }

        private async Task<Result<bool>> CreateLoadingRows(IEnumerable<LoadingEntryDto> entries, Loading loading, string userCode)
        {
            foreach (var entry in entries)
            {
                //Step 2 : Insert Into Loading Detail
                var loadingDetail = new LoadingDetail();
                mapper.Map(entry, loadingDetail, opts =>
                {
                    opts.AfterMap((src, dest) =>
                    {
                        dest.JobNo = loading.JobNo;
                        dest.AddedBy = userCode;
                        dest.ETD = loading.ETD;
                    });
                });

                await repository.AddLoadingDetailAsync(loadingDetail);
            }

            if (appSettings.IsSAPFactory(loading.CustomerCode))
            {
                await ChangeEKanbanStatusToDataSent(entries.Select(e => e.OrderNo));
            }
            else
            {
                await UpdateOutboundsWithETDDate(entries.Select(e => e.OutboundJobNo), loading.ETD);
            }
            await repository.SaveChangesAsync();

            loading.Status = await RefreshLoadingStatus(loading.JobNo);
            loading.RevisedBy = userCode;
            loading.RevisedDate = DateTime.Now;

            if (loading.AllowedForDispatch)
            {
                loading.AllowedForDispatch = false;
                loading.AllowedForDispatchModifiedBy = userCode;
                loading.AllowedForDispatchModifiedDate = DateTime.Now;
            }

            await repository.SaveChangesAsync();

            return new SuccessResult<bool>(true);
        }

        public async Task<Stream> LoadingReport(string whsCode, string jobNo)
        {
            var fileName = "LoadingReport.rpt";
            var title = "Loading Report ";
            var formula = "{TT_Loading.JobNo} = '" + jobNo + "'";
            return await Task.Run(() => reportService.GenerateReport(fileName, whsCode, title, formula));
        }

        public async Task<Result<Stream>> DeliveryDocketReport(string whsCode, string jobNo)
        {
            var loading = await repository.GetLoadingAsync(jobNo);
            if (loading == null)
                return new NotFoundResult<Stream>(new JsonResultError("RecordNotFound").ToJson());

            if (loading.Status == LoadingStatus.Confirmed || loading.Status == LoadingStatus.Completed)
            {
                var fileName = "LoadingDeliveryDocket_HU.rpt";
                var title = "Delivery Docket ";
                var formula = "{TT_Loading.JobNo} = '" + jobNo + "'";
                var shouldShowLocationName = appSettings.OwnerCode.In(OwnerCode.PL, OwnerCode.GE, OwnerCode.IT);
                var parameters = new JObject() { { "showLocationName", shouldShowLocationName } };
                var reportStream = await Task.Run(() => reportService.GenerateReport(fileName, whsCode, title, formula, null, parameters));
                return new SuccessResult<Stream>(reportStream);
            }
            return new InvalidResult<Stream>(new JsonResultError("DeliveryDocketPrintError_Confirmed").ToJson());
        }

        public async Task<Result<Stream>> LoadingPickingInstructionReport(string whsCode, string jobNo, string userCode)
        {
            if (appSettings.OwnerCode.StartsWith(UtilityService.TESA_CODE))
            {
                // create QR codes for all outbounds linked to loadingDetail rows OutJobNo
                var loadingRows = await repository.LoadingDetails().Where(l => l.JobNo == jobNo && !String.IsNullOrWhiteSpace(l.OutJobNo)).ToListAsync();
                foreach (var ld in loadingRows)
                {
                    var res = await outboundService.GetOutboundQRCodeImage(ld.OutJobNo, userCode);
                    if (res.ResultType == ResultType.Invalid)
                        return new InvalidResult<Stream>(new JsonResultError()
                        {
                            MessageKey = "CannotCreateQRCodeForOutbound__",
                            Arguments = new Dictionary<string, string>()
                            {
                                {"outJobNo", ld.OutJobNo},
                                {"error", res.Errors[0]}
                            }
                        }.ToJson());
                }

                var fileName = "LoadingPickingInstructionReportRFEuro.rpt";
                var title = "Picking Instruction ";
                var formula = "{TT_Loading.JobNo} = '" + jobNo + "'";
                var reportStream = await Task.Run(() => reportService.GenerateReport(fileName, whsCode, title, formula));
                foreach (var ld in loadingRows)
                {
                    await repository.AddReportPrintingLogAsync(new ReportPrintingLog
                    {
                        PrintedBy = userCode,
                        JobNo = ld.OutJobNo,
                        ReportName = ReportNames.PickingInstructionReportRFEuro
                    });
                }

                return new SuccessResult<Stream>(reportStream);
            }
            return new InvalidResult<Stream>(new JsonResultError("CannotPrintReportOwnerNotTESA").ToJson());
        }

        public async Task<Result<Stream>> OutboundReport(string whsCode, string jobNo)
        {
            if (appSettings.OwnerCode.StartsWith(UtilityService.TESA_CODE))
            {
                var fileName = "LoadingOutboundReportEuro.rpt";
                var title = "Loading Report";
                var formula = "{TT_Loading.JobNo} = '" + jobNo + "'";
                var reportStream = await Task.Run(() => reportService.GenerateReport(fileName, whsCode, title, formula));
                return new SuccessResult<Stream>(reportStream);
            }
            return new InvalidResult<Stream>(new JsonResultError("CannotPrintReportOwnerNotTESA").ToJson());
        }

        public async Task<Result<Stream>> DeliveryDocketCombinedReport(string whsCode, string jobNo)
        {
            var loading = await repository.GetLoadingAsync(jobNo);
            if (loading == null)
                return new NotFoundResult<Stream>(new JsonResultError("RecordNotFound").ToJson());
            if (loading.Status == LoadingStatus.Confirmed || loading.Status == LoadingStatus.Completed)
            {
                var fileName = "LoadingDeliveryDocket.rpt";
                var title = "Delivery Docket ";
                var formula = "{TT_Loading.JobNo} = '" + jobNo + "'";
                var shouldShowLocationName = appSettings.OwnerCode.In(OwnerCode.PL, OwnerCode.GE, OwnerCode.IT);
                var parameters = new JObject() { { "showLocationName", shouldShowLocationName } };
                var reportStream = await Task.Run(() => reportService.GenerateReport(fileName, whsCode, title, formula, null, parameters));
                return new SuccessResult<Stream>(reportStream);
            }
            return new InvalidResult<Stream>(new JsonResultError("DeliveryDocketPrintError_Confirmed").ToJson());
        }

        private async Task<LoadingDto> MapLoadingDto(Loading entity)
        {
            var customerName = (await repository.GetCustomerAsync(entity.CustomerCode, entity.WHSCode)).Name;
            var currency = await repository.GetCurrencyForLoading(entity.JobNo);
            var calculatedNoOfPallet = await repository.GetNoOfPalletsForLoading(entity.JobNo);
            var statuses = await repository.GetOutboundStatusesForLoading(entity.JobNo);

            return mapper.Map<LoadingDto>(entity,
                opts =>
                {
                    opts.AfterMap((src, dest) =>
                    {
                        dest.CustomerName = customerName;
                        dest.Currency = currency;
                        dest.MixedCurrencies = currency == "";
                        dest.CalculatedNoOfPallet = calculatedNoOfPallet;
                        dest.MinOutboundStatus = statuses.Any() ? statuses.Min() : OutboundStatus.NewJob;
                        dest.MaxOutboundStatus = statuses.Any() ? statuses.Max() : OutboundStatus.NewJob;
                    });
                });
        }

        private async Task UpdateOutboundsWithETDDate(IEnumerable<string> outJobNos, DateTime? etd)
        {
            if (outJobNos.Any() && etd.HasValue)
            {
                var outbounds = await repository.Outbounds().Where(o => outJobNos.Contains(o.JobNo)).ToListAsync();
                foreach (var outbound in outbounds)
                {
                    outbound.ETD = etd.Value;
                }
            }

        }

        private async Task ChangeEKanbanStatusToDataSent(IEnumerable<string> orderNos)
        {
            var ekanbans = await repository.EKanbanHeaders().Where(e => orderNos.Contains(e.OrderNo)
                && e.Status == (int)EKanbanStatus.DataSent).ToListAsync();

            foreach (var ek in ekanbans)
            {
                ek.Status = (int)EKanbanStatus.InTransit;
            }
        }

        private async Task<LoadingStatus> RefreshLoadingStatus(string jobNo)
        {
            var status = LoadingStatus.NewJob;

            var outboundList = await repository.GetDistinctLoadingOutboundList(jobNo);
            foreach (var s in outboundList)
            {
                if (s == OutboundStatus.Picked ||
                    s == OutboundStatus.Packed ||
                    s == OutboundStatus.OutStanding ||
                    s == OutboundStatus.InTransit ||
                    s == OutboundStatus.Completed)
                {
                    status = LoadingStatus.Picked;
                }
                else if (s == OutboundStatus.NewJob ||
                    s == OutboundStatus.PartialDownload ||
                    s == OutboundStatus.Downloaded ||
                    s == OutboundStatus.PartialPicked)
                {
                    if (status != LoadingStatus.NewJob)
                        status = LoadingStatus.Processing;
                }
            }
            return status;
        }

        private async Task<Result<Loading>> CreateLoadingObject(LoadingAddDto loading, string userCode)
        {
            var entity = new Loading();
            mapper.Map(loading, entity);

            var jobNoResult = await utilityService.GenerateJobNo(JobType.Loading);
            if (jobNoResult.ResultType == ResultType.Invalid)
            {
                return new InvalidResult<Loading>(jobNoResult.Errors[0]);
            }
            entity.JobNo = jobNoResult.Data;
            entity.Status = LoadingStatus.NewJob;
            entity.CreatedBy = userCode;
            await repository.AddLoadingAsync(entity);

            return new SuccessResult<Loading>(entity);
        }

        private readonly ITTLogixRepository repository;
        private readonly IMapper mapper;
        private readonly IUtilityService utilityService;
        private readonly IOutboundService outboundService;
        private readonly IReportService reportService;
        private readonly ILoggerService loggerService;
        private readonly AppSettings appSettings;
    }
}
