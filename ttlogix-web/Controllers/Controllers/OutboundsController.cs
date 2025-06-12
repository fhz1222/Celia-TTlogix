using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServiceResult;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TT.Common;
using TT.Controllers.Authorization;
using TT.Controllers.Extensions;
using TT.Controllers.Utilities;
using TT.Core.Enums;
using TT.Core.QueryFilters;
using TT.Services.Interfaces;
using TT.Services.Models;
using TT.Services.Services;
using TT.Services.Services.Utilities;

namespace TT.Controllers.Controllers
{
    [ModuleAccessAuthorize(SystemModuleNames.OUTBOUND)]
    [Route("api/outbounds")]
    [ApiController]
    public class OutboundsController : TTLogixControllerBase
    {
        public OutboundsController(IOutboundService outboundService, IOptions<AppSettings> appSettings)
            => this.outboundService = outboundService;

        /// <summary>
        /// gets the list of outbounds for the main grid; filtered and paged
        /// original endpoint: OutboundBizFacade.cs > GetOutboundList(DataTable p_dtFilter, DataTable p_dtOrderBy)
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns>OutboundListDto</returns>
        [HttpGet]
        [Route("getOutbounds")]
        public async Task<ActionResult<OutboundListDto>> GetOutbounds([FromQuery] OutboundListQueryFilter queryFilter)
        {
            queryFilter.UserWHSCode = User.GetWHSCode();
            return Ok(await outboundService.GetOutboundList(queryFilter));
        }

        /// <summary>
        /// gets the outbound full object
        /// original endpoint: GetOutbound(Request.TT_Outbound p_oOutbound)
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getOutbound")]
        public async Task<ActionResult<OutboundDto>> GetOutbound([RequiredAsJsonError] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }

            var outbound = await outboundService.GetOutbound(jobNo);
            return outbound == null ? NotFound() : Ok(outbound);
        }

        /// <summary>
        /// original endpoint: GetOutboundDetailList(DataTable p_dtFilter, DataTable p_dtOrderBy)
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getOutboundDetails")]
        public async Task<ActionResult<OutboundDetailDto>> GetOutboundDetailList([RequiredAsJsonError] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return Ok(await outboundService.GetOutboundDetailList(jobNo));
        }

        /// <summary>
        /// original endpoint: GetOutboundDetailWithReceivedQtyList(DataTable p_dtFilter, DataTable p_dtOrderBy)
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getOutboundDetailsWithReceivedQty")]
        public async Task<ActionResult<OutboundDetailDto>> GetOutboundDetailWithReceivedQtyList([RequiredAsJsonError] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return Ok(await outboundService.GetOutboundDetailWithReceivedQtyList(jobNo));
        }

        /// <summary>
        /// gets the pickable list with quantities for all product codes for part masters for outbound detail item
        /// used in "Pick Entry" method on OutboundDetailItem
        /// original endpoint:l_oOutboundController.GetOutboundPickableList and l_oOutboundController.GetOutboundPickableListManual
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getOutboundPickableList")]
        public async Task<ActionResult<IEnumerable<OutboundPickableListDto>>> GetOutboundPickableList([FromQuery] OutboundPickableListQueryFilter filter)
            => Ok(await outboundService.GetOutboundPickableList(filter));

        /// <summary>
        /// Cancel Outbound record
        /// original endpoint: CancelOutbound(string p_strJobNo, string p_strUserCode)
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpPost]
        [ServiceFilter(typeof(CheckIfLockedFilter))]
        [Route("cancelOutbound")]
        public async Task<ActionResult> CancelOutbound([RequiredAsJsonError] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await outboundService.CancelOutbound(jobNo, User.Identity.Name));
        }

        /// <summary>
        /// changes the outbound status
        /// original endpoint: UpdateOutboundStatus(Request.TT_Outbound p_oOutbound, byte p_byteOwnership, double p_dblMasterPIDQty)
        /// #2803 - this has been amended so it only changes the statuses when called; 
        /// should not really be called itself, but individually on each of the endpoints as required
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updateOutboundStatus")]
        public async Task<ActionResult<bool>> UpdateOutboundStatus([RequiredAsJsonError][FromQuery] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await outboundService.UpdateOutboundStatus(jobNo));
        }

        /// <summary>
        /// creates outbound and correcsponding EKanbanHeader
        /// original endpoint: CreateOutboundManual(Request.TT_Outbound p_oOutbound, string p_strManualType)
        /// </summary>
        /// <param name="outboundDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createOutboundManual")]
        public async Task<ActionResult<string>> CreateOutboundManual(OutboundManualDto outboundDto)
        {
            var allowedMethods = outboundService.GetAllowedOutboundCreationMethods();
            if (!allowedMethods.AllowManual && outboundDto.TransType != OutboundType.WHSTransfer)
                return FromResult(new InvalidResult<string>(new JsonResultError("OperationNotPermitted").ToJson()));
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await outboundService.CreateOutboundManual(outboundDto, User.Identity.Name));
        }

        /// <summary>
        /// patches the outbound record
        /// original endpoint: UpdateOutbound(Request.TT_Outbound p_oOutbound)
        /// </summary>
        /// <param name="jobNo"></param>
        /// <param name="outboundDto"></param>
        /// <returns></returns>
        [ServiceFilter(typeof(CheckIfLockedFilter))]
        [HttpPatch("{jobNo}")]
        public async Task<ActionResult> PatchOutbound([RequiredAsJsonError] string jobNo, [FromBody] OutboundDto outboundDto)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await outboundService.UpdateOutbound(jobNo, outboundDto));
        }

        /// <summary>
        /// Delete outbound detail line and change the inventory quantities 
        /// original endpoint: DeleteOutboundDetailManual(Request.TT_OutboundDetail p_oOutboundDetail), DeleteOutboundDetail(Request.TT_OutboundDetail p_oOutboundDetail), 
        /// also it does the check for the existing picking lists.
        /// note DeleteOutboundDetail and DeleteOutboundDetailManual do the same job.
        /// </summary>
        /// <param name="jobNo"></param>
        /// <param name="lineItem"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("outboundDetail")]
        [ServiceFilter(typeof(CheckIfLockedFilter))]
        public async Task<ActionResult> DeleteOutboundDetail([RequiredAsJsonError][FromQuery] string jobNo, [RequiredAsJsonError][FromQuery] int lineItem)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await outboundService.DeleteOutboundDetail(jobNo, lineItem));
        }

        /// <summary>
        /// imports eKanban data
        /// original endpoint:ImportEKanbanEUCPart(string strOrderNo, string strFactoryID, ref string strJobNo,string p_strWHSCode, string p_strUserCode)
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="factoryId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("importEKanbanEUCPart")]
        public async Task<ActionResult<string>> ImportEKanbanEUCPart([RequiredAsJsonError] string orderNo, string factoryId)
        {

            var allowedMethods = outboundService.GetAllowedOutboundCreationMethods();
            if (!allowedMethods.AllowEKanbanImport)
                return FromResult(new InvalidResult<string>(new JsonResultError("OperationNotPermitted").ToJson()));
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await outboundService.ImportEKanbanEUCPart(orderNo, factoryId, User.GetWHSCode(), User.Identity.Name));
        }

        /// <summary>
        /// Cargo Out operation for EKanban = TESA
        /// original endpoint: GetListResult CompleteOutboundEurope(ref string[] p_strJobNo, string p_strUserCode)
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpPost]
        [ServiceFilter(typeof(CheckIfLockedFilter))]
        [Route("completeOutboundEurope")]
        public async Task<ActionResult<bool>> CompleteOutboundEurope([RequiredAsJsonError] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await outboundService.CompleteOutboundEurope(new string[] { jobNo }, User.Identity.Name));
        }

        /// <summary>
        /// Complete done from OutboundDetails
        /// a simple change of Outbound status to Completed.
        /// original endpoint: OutboundDetails btnComplete_Click CompleteDiscrepancyOutbound
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpPost]
        [ServiceFilter(typeof(CheckIfLockedFilter))]
        [Route("completeDiscrepancyOutbound")]
        public async Task<ActionResult<bool>> CompleteDiscrepancyOutbound([RequiredAsJsonError] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await outboundService.CompleteDiscrepancyOutbound(jobNo, User.Identity.Name));
        }

        /// <summary>
        /// Cargo Out operation for EKAnban  RefNo = TTK
        /// original endpoint: GetListResult CompleteOutboundReturn(ref string[] p_strJobNo, string p_strUserCode)
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpPost]
        [ServiceFilter(typeof(CheckIfLockedFilter))]
        [Route("completeOutboundReturn")]
        public async Task<ActionResult<bool>> CompleteOutboundReturn([RequiredAsJsonError] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await outboundService.CompleteOutboundReturn(new string[] { jobNo }, User.Identity.Name));
        }

        /// <summary>
        /// Cargo Out operation for jobType = manual
        /// original endpoint: GetListResult CompleteOutboundManual(ref string[] p_strJobNo, string p_strUserCode)
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpPost]
        [ServiceFilter(typeof(CheckIfLockedFilter))]
        [Route("completeOutboundManual")]
        public async Task<ActionResult<bool>> CompleteOutboundManual([RequiredAsJsonError] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await outboundService.CompleteOutboundManual(new string[] { jobNo }, User.Identity.Name));
        }

        /// <summary>
        /// Cargo Out operation for eKanban, not TESA and not TTK
        /// original endpoint: CargoInTransit(ref string[] p_strJobNo, string p_strUserCode)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("cargoInTransit")]
        public async Task<ActionResult<bool>> CargoInTransit(JobNosDto data)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await outboundService.CargoInTransit(data.JobNos, User.Identity.Name));
        }

        /// <summary>
        /// cancels allocation for selected parts (the same method for TESA and non-TESA)
        /// original endpoint: CancelAllocationCPart(ref DataSet p_DataSet, Request.TT_OutboundDetail p_oOutboundDetail) and
        /// CancelAllocation(ref DataSet p_DataSet, Request.TT_OutboundDetail p_oOutboundDetail)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("cancelAllocation")]
        public async Task<ActionResult<bool>> CancelAllocation(CancelAllocationDto data)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await outboundService.CancelAllocation(data));
        }

        /// <summary>
        /// create ountbound detail entry for outbound
        /// original endpoint: OutboundDetailItem > PickEntry inline method (no endpoint)
        /// AddNewOutboundDetail, AddNewOutboundDetailManual (both in the end used the same method)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addNewOutboundDetail")]
        public async Task<ActionResult<int>> AddNewOutboundDetail(OutboundDetailAddDto data)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await outboundService.AddNewOutboundDetail(data, User.Identity.Name));
        }

        /// <summary>
        /// undo pick entry for outbound
        /// original endpoint: OutboundDetailItem > l_oPickingListController.UndoPicking (no endpoint, btn_Unpick)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("undoPicking")]
        public async Task<ActionResult<bool>> UndoPicking(UndoPickEntryDto data)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await outboundService.UndoPicking(data.OutJobNo, data.PIDs));
        }

        /// <summary>
        /// split the outbound rows by creating a new outbound for selected items
        /// original endpoint: SplitOutbound(string p_strJobNo, System.Data.DataSet p_dstPicking, bool p_bOwnershipSplit, double p_dblMasterPIDQty, string p_strUserCode, string p_strSupplierName) 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("splitOutbound")]
        public async Task<ActionResult<string>> SplitOutbound(SplitOutboundDto data)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await outboundService.SplitOutbound(data, User.Identity.Name));
        }

        /// <summary>
        /// split the outbound rows by grouping them by inbound job no.
        /// Replaces the calculations made both in the form and on the API call SplitOutboundWithSupplierName
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("splitOutboundByInboundJobNo")]
        public async Task<ActionResult<string>> SplitOutboundByInboundJobNo([RequiredAsJsonError][FromQuery] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await outboundService.SplitOutboundByDateOrInJobNo(jobNo, false, User.Identity.Name));
        }

        /// <summary>
        /// split the outbound rows by grouping them by inbound date.
        /// Replaces the calculations made both in the form and on the API call SplitOutboundWithSupplierName
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("splitOutboundByDate")]
        public async Task<ActionResult<string>> SplitOutboundByInboundDate([RequiredAsJsonError][FromQuery] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await outboundService.SplitOutboundByDateOrInJobNo(jobNo, true, User.Identity.Name));
        }

        /// <summary>
        /// split the outbound by grouping picking list items by ownership.
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("splitOutboundByOwnership")]
        public async Task<ActionResult<string>> SplitOutboundByOwnership([RequiredAsJsonError][FromQuery] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await outboundService.SplitOutboundByOwnership(jobNo, User.Identity.Name));
        }

        /// <summary>
        /// Release Bonded
        /// original endpoint: ReleaseBondedStock(ref string[] p_strJobNo, string p_strUserCode)
        /// also combines the UpdateOutbound(..) call that follows
        /// </summary>
        /// <param name="jobNo"></param>
        /// <param name="outboundDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("releaseBondedStock")]
        public async Task<ActionResult<bool>> ReleaseBondedStock([RequiredAsJsonError][FromQuery] string jobNo, [FromBody] OutboundDto outboundDto)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            if (jobNo != outboundDto.JobNo) { return Forbid(); }

            return FromResult(await outboundService.ReleaseBondedStock(outboundDto, User.Identity.Name));
        }

        /// <summary>
        /// Complete warehouse transfer
        /// original endpoint: CompleteWHSTransfer
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("completeWHSTransfer")]
        public async Task<ActionResult<bool>> CompleteWHSTransfer([RequiredAsJsonError] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await outboundService.CompleteWHSTransfer(new string[] { jobNo }, User.Identity.Name));
        }

        /// <summary>
        /// Get Outbound QRCode Image
        /// original endpoint: combines 2 methods: GetOutboundQRCode and AddNewPickingQRCode
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getOutboundQRCodeImage")]
        public async Task<ActionResult<string>> GetOutboundQRCodeImage([RequiredAsJsonError] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }

            var result = await outboundService.GetOutboundQRCodeImage(jobNo, User.Identity.Name);
            return result.ResultType == ResultType.Ok ? Convert.ToBase64String(result.Data) : FromResult(result);
        }

        [HttpGet]
        [Route("getOrderSummary")]
        public async Task<ActionResult<OutboundDetailDto>> GetOrderSummary([RequiredAsJsonError] string jobNo)
        {
            return Ok(await outboundService.GetOrderSummary(jobNo));
        }


        [HttpGet]
        [Route("getOutboundReport")]
        public async Task<ActionResult> GetOutboundReport([RequiredAsJsonError] string jobNo)
        {
            var reportResult = await outboundService.OutboundReport(User.GetWHSCode(), User.Identity.Name, jobNo);
            return FromStreamResult(reportResult, jobNo + "_outbound_report.pdf", "application/pdf");
        }

        [HttpGet]
        [Route("getPickingListReport")]
        public async Task<FileStreamResult> GetPickingListReport([RequiredAsJsonError] string jobNo)
            => new FileStreamResult(await outboundService.PickingListReport(User.GetWHSCode(), User.Identity.Name, jobNo), "application/pdf")
            {
                FileDownloadName = jobNo + "_picking_list.pdf"
            };

        [HttpGet]
        [Route("getPickingInstructionReport")]
        public async Task<FileStreamResult> GetPickingInstructionReport([RequiredAsJsonError] string jobNo)
            => new FileStreamResult(await outboundService.PickingInstructionReport(User.GetWHSCode(), User.Identity.Name, jobNo), "application/pdf")
            {
                FileDownloadName = jobNo + "_picking_list.pdf"
            };

        [HttpGet]
        [Route("getDeliveryDocketReport")]
        public async Task<FileStreamResult> GetDeliveryDocketReport([RequiredAsJsonError] string jobNo)
            => new FileStreamResult(await outboundService.DeliveryDocketReport(User.GetWHSCode(), User.Identity.Name, jobNo), "application/pdf")
            {
                FileDownloadName = jobNo + "_delivery_docket.pdf"
            };

        [HttpGet]
        [Route("getDeliveryDocketWithPIDReport")]
        public async Task<FileStreamResult> GetDeliveryDocketWithPIDReport([RequiredAsJsonError] string jobNo)
            => new FileStreamResult(await outboundService.DeliveryDocketWithPIDReport(User.GetWHSCode(), User.Identity.Name, jobNo), "application/pdf")
            {
                FileDownloadName = jobNo + "_delivery_docket_with_pid.pdf"
            };

        [HttpGet]
        [Route("getPackingListReport")]
        public async Task<FileStreamResult> GetPackingListReport([RequiredAsJsonError] string jobNo)
            => new FileStreamResult(await outboundService.PackingListReport(User.GetWHSCode(), User.Identity.Name, jobNo), "application/pdf")
            {
                FileDownloadName = jobNo + "_packing_list.pdf"
            };

        /// <summary>
        /// Returns a csv file with EDT
        /// Original endpoint: DownloadEDTToCSV, split into separate controllers for outbound and stock transfer
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("downloadEDTToCSV")]
        public async Task<IActionResult> DownloadEDTToCSV([RequiredAsJsonError] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromStreamResult(await outboundService.DownloadEDTToCSV(jobNo, User.Identity.Name), $"{jobNo}.txt", "text/plain");
        }

        /// <summary>
        /// Dispatches a TESAG warehouse transfer outbound
        /// </summary>
        /// <param name="jobNo">Warehouse transfer job number</param>
        /// <returns></returns>
        [HttpPost("dispatchWarehouseTransfer")]
        public async Task<ActionResult<bool>> DispatchWarehouseTransfer([RequiredAsJsonError][FromQuery] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await outboundService.DispatchWarehouseTransfer(jobNo, User.Identity.Name));
        }
        /// Returns allowed import methods for Stock Transfers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("allowedOutboundCreationMethods")]
        public ActionResult<AllowedOutboundCreationMethodsDto> GetAllowedOutboundCreationMethods()
        {
            return outboundService.GetAllowedOutboundCreationMethods();
        }

        private readonly IOutboundService outboundService;
    }
}
