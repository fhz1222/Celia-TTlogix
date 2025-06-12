using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TT.Common;
using TT.Controllers.Authorization;
using TT.Controllers.Extensions;
using TT.Controllers.Utilities;
using TT.Core.QueryFilters;
using TT.Services.Interfaces;
using TT.Services.Models;
using TT.Services.Services.Utilities;

namespace TT.Controllers.Controllers
{
    [Route("api/loadings")]
    [ModuleAccessAuthorize(SystemModuleNames.LOADING)]
    [ApiController]
    public class LoadingsController : TTLogixControllerBase
    {
        public LoadingsController(ILoadingService loadingService)
            => this.loadingService = loadingService;

        /// <summary>
        /// Get the list of loadings
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getLoadings")]
        public async Task<ActionResult<LoadingListDto>> GetLoadings([FromQuery] LoadingListQueryFilter queryFilter)
        {
            queryFilter.UserWHSCode = User.GetWHSCode();
            return Ok(await loadingService.GetLoadingList(queryFilter));
        }

        /// <summary>
        /// Set truck arrival to current date
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("truckArrival")]
        public async Task<ActionResult> TruckArrival([RequiredAsJsonError][FromQuery] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await loadingService.SetTruckArrival(jobNo));
        }

        /// <summary>
        /// Set truck departure to current date
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("truckDeparture")]
        public async Task<ActionResult> TruckDeparture([RequiredAsJsonError][FromQuery] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await loadingService.SetTruckDeparture(jobNo));
        }

        /// <summary>
        /// gets the Loading full object 
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getLoading")]
        public async Task<ActionResult<LoadingDto>> GetLoading([RequiredAsJsonError] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }

            var loading = await loadingService.GetLoading(jobNo);
            return loading == null ? NotFound() : Ok(loading);
        }

        /// <summary>
        /// gets the Loading detail rows 
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getLoadingDetails")]
        public async Task<ActionResult<IEnumerable<LoadingDetailDto>>> GetLoadingDetails([RequiredAsJsonError] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }

            var loadingDetails = await loadingService.GetLoadingDetails(jobNo);
            return loadingDetails == null ? NotFound() : Ok(loadingDetails);
        }

        /// <summary>
        /// Add loading header
        /// </summary>
        /// <param name="loadingDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> PostLoading(LoadingAddDto loadingDto)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await loadingService.CreateLoading(loadingDto, User.Identity.Name));
        }

        /// <summary>
        /// Add loading header and details from the attached outbound job numbers
        /// </summary>
        /// <param name="loadingDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addLoadingFromOutbound")]
        public async Task<ActionResult> AddLoadingFromOutbound(AddLoadingFromOutboundDto loadingDto)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await loadingService.CreateLoadingFromOutbound(loadingDto, User.Identity.Name, User.GetWHSCode()));
        }

        /// <summary>
        /// Updates the loading object and some corresponding entities
        /// replaces the UpdateLoading and UpdateLoadingDetail calls altogether, called from LoadingDetail.SaveLoadingInformation 
        /// </summary>
        [ServiceFilter(typeof(CheckIfLockedFilter))]
        [HttpPatch("{jobNo}")]
        public async Task<ActionResult> PatchLoading([RequiredAsJsonError] string jobNo, [FromBody] LoadingDto loadingDto)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await loadingService.UpdateLoading(jobNo, loadingDto, User.Identity.Name));
        }

        /// <summary>
        /// Get job nos of stock on storage marked as bonded with no CommInvNo
        /// previously used by LoadingDetail btnComplete_Click m_oWebServiceCtrl.CheckBondedStockBeforeLoading
        /// if any jobNos are returned, they should be formatted in the following message:
        /// l_strMsg = $"No Commercial Invoice No. fill in for following Outbound job: {string.Join(", ", jobNos)}");
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getBondedStockJobNosWithoutCommInv")]
        public async Task<ActionResult<IEnumerable<string>>> GetBondedStockJobNosWithoutCommInv([RequiredAsJsonError] string jobNo)
            => Ok(await loadingService.GetBondedStockJobNosWithoutCommInv(jobNo));

        /// <summary>
        /// Confirm the loading
        /// api ref: ConfirmLoading
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpPost]
        [ServiceFilter(typeof(CheckIfLockedFilter))]
        [Route("confirmLoading")]
        public async Task<ActionResult> ConfirmLoading([RequiredAsJsonError][FromQuery] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await loadingService.ConfirmLoading(jobNo, User.Identity.Name));
        }

        /// <summary>
        /// Set loading as cancelled
        /// no api reference
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpPost]
        [ServiceFilter(typeof(CheckIfLockedFilter))]
        [Route("cancelLoading")]
        public async Task<ActionResult> CancelLoading([RequiredAsJsonError][FromQuery] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await loadingService.CancelLoading(jobNo, User.Identity.Name));
        }

        [HttpPost]
        [Route("deleteLoadingDetails")]
        public async Task<ActionResult> DeleteLoadingDetails(DeleteLoadingDetailDto data)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await loadingService.DeleteLoadingDetails(data, User.Identity.Name));
        }

        /// <summary>
        /// Gets the list of orders that can be selected to create loading detail 
        /// original method name: GetLoadingEntryListForEurope
        /// </summary>
        /// <param name="customerCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getLoadingEntryList")]
        public async Task<ActionResult<IEnumerable<LoadingEntryDto>>> GetLoadingEntryList([RequiredAsJsonError] string customerCode)
            => Ok(await loadingService.GetLoadingEntryList(customerCode, User.GetWHSCode()));

        /// <summary>
        /// Create loading detail rows based on selected order nos
        /// Original method name: BatchInsertLoadingDetail 
        /// </summary>
        /// <param name="jobNo">Loading jobNo</param>
        /// <param name="orderNos">Selected OrderNos</param>
        /// <returns></returns>
        [HttpPost]
        [Route("batchCreateLoadingDetails")]
        public async Task<ActionResult> BatchCreateLoadingDetail([RequiredAsJsonError][FromQuery] string jobNo, IEnumerable<string> orderNos)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await loadingService.CreateLoadingDetails(orderNos, jobNo, User.Identity.Name, User.GetWHSCode()));
        }

        [HttpPost]
        [ServiceFilter(typeof(CheckIfLockedFilter))]
        [Route("setAllowForDispatch")]
        public async Task<ActionResult> SetAllowForDispatch([RequiredAsJsonError][FromQuery] string jobNo, [RequiredAsJsonError][FromQuery] bool allow)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await loadingService.SetAllowForDispatch(jobNo, allow, User.Identity.Name));
        }

        /// <summary>
        /// Get LoadingReport
        /// </summary>
        /// <param name="jobNo">Loading jobNo</param>
        /// <returns>file stream</returns>
        [HttpGet]
        [Route("getLoadingReport")]
        public async Task<FileStreamResult> GetLoadingReport([RequiredAsJsonError] string jobNo)
            => new FileStreamResult(await loadingService.LoadingReport(User.GetWHSCode(), jobNo), "application/pdf")
            {
                FileDownloadName = jobNo + "_loading_report.pdf"
            };

        /// <summary>
        /// Get DeliveryDocketReport
        /// </summary>
        /// <param name="jobNo">Loading jobNo</param>
        /// <returns>file stream</returns>
        [HttpGet]
        [Route("getDeliveryDocketReport")]
        public async Task<ActionResult> GetDeliveryDocketReport([RequiredAsJsonError] string jobNo)
        {
            var reportResult = await loadingService.DeliveryDocketReport(User.GetWHSCode(), jobNo);
            return FromStreamResult(reportResult, jobNo + "_delivery_docket_report.pdf", "application/pdf");
        }

        /// <summary>
        /// Get LoadingPickingInstructionReport
        /// </summary>
        /// <param name="jobNo">Loading jobNo</param>
        /// <returns>file stream</returns>
        [HttpGet]
        [Route("getPickingInstructionReport")]
        public async Task<ActionResult> GetLoadingPickingInstructionReport([RequiredAsJsonError] string jobNo)
        {
            var reportResult = await loadingService.LoadingPickingInstructionReport(User.GetWHSCode(), jobNo, User.Identity.Name);
            return FromStreamResult(reportResult, jobNo + "_loading_picking_instruction_report.pdf", "application/pdf");
        }

        /// <summary>
        /// Get OutboundReport filered by loading jobNo
        /// </summary>
        /// <param name="jobNo">Loading jobNo</param>
        /// <returns>file stream</returns>
        [HttpGet]
        [Route("getOutboundReport")]
        public async Task<ActionResult> GetOutboundReport([RequiredAsJsonError] string jobNo)
        {
            var reportResult = await loadingService.OutboundReport(User.GetWHSCode(), jobNo);
            return FromStreamResult(reportResult, jobNo + "_outbound_report.pdf", "application/pdf");
        }

        /// <summary>
        /// Get DeliveryDocketCombinedReport
        /// </summary>
        /// <param name="jobNo">Loading jobNo</param>
        /// <returns>file stream</returns>
        [HttpGet]
        [Route("getDeliveryDocketCombinedReport")]
        public async Task<ActionResult> GetDeliveryDocketCombinedReport([RequiredAsJsonError] string jobNo)
        {
            var reportResult = await loadingService.DeliveryDocketCombinedReport(User.GetWHSCode(), jobNo);
            return FromStreamResult(reportResult, jobNo + "_delivery_docket_combined_report.pdf", "application/pdf");
        }

        private readonly ILoadingService loadingService;
    }
}
