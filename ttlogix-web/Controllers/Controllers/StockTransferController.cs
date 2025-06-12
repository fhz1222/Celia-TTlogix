using Microsoft.AspNetCore.Mvc;
using ServiceResult;
using System.Collections.Generic;
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
    /// <summary>
    /// All endpoints related to Stock Transfer operations
    /// </summary>
    [ModuleAccessAuthorize(SystemModuleNames.STOCKTRANSFER)]
    [Route("api/stockTransfers")]
    [ApiController]
    public class StockTransferController : TTLogixControllerBase
    {
        public StockTransferController(IStockTransferService stockTransferService)
            => this.stockTransferService = stockTransferService;

        /// <summary>
        /// Get stock transfer records displayed on the ST main list
        /// </summary>
        /// <param name="queryFilter">filtering and paging parameters</param>
        /// <returns>Stock transfer list DTO containing paging information and data records</returns>
        [HttpGet]
        [Route("getStockTransferList")]
        public async Task<ActionResult<StockTransferListDto>> GetStockTransferList([FromQuery] StockTransferListQueryFilter queryFilter)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }

            queryFilter.UserWHSCode = User.GetWHSCode();
            return Ok(await stockTransferService.GetStockTransferList(queryFilter));
        }

        /// <summary>
        /// Get stock transfer data record for particular JobNo
        /// </summary>
        /// <param name="jobNo">ST JobNo</param>
        /// <returns>Stock transfer DTO</returns>
        /// <response code="400">JobNo is required</response>
        [HttpGet]
        [Route("getStockTransfer")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<StockTransferDto>> GetStockTransfer([RequiredAsJsonError] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }

            var stockTransfer = await stockTransferService.GetStockTransfer(jobNo);
            return stockTransfer == null ? NotFound() : Ok(stockTransfer);
        }

        /// <summary>
        /// Get stock transfer details list extended with UOM and storage data
        /// </summary>
        /// <param name="jobNo">ST JobNo</param>
        /// <returns>Stock transfer detail DTO list</returns>
        /// <response code="400">JobNo is required</response>
        [HttpGet]
        [Route("getStockTransferDetails")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IEnumerable<StockTransferDetailDto>>> GetStockTransferDetailList([RequiredAsJsonError] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return Ok(await stockTransferService.GetStockTransferDetailList(jobNo));
        }

        /// <summary>
        /// Get stock transfer summary list
        /// </summary>
        /// <param name="jobNo">ST JobNo</param>
        /// <returns>Stock Transfer summary DTO list</returns>
        /// <response code="400">JobNo is required</response>
        [HttpGet]
        [Route("getStockTransferSummary")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IEnumerable<StockTransferSummaryDto>>> GetStockTransferSummaryList([RequiredAsJsonError] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return Ok(await stockTransferService.GetStockTransferSummaryList(jobNo));
        }

        /// <summary>
        /// Create Stock Transfer entry 
        /// </summary>
        /// <param name="customerCode">Customer code</param>
        /// <returns>Newly created Stock transfer DTO</returns>
        /// <response code="400">customerCode is required</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<StockTransferDto>> CreateStockTransfer([RequiredAsJsonError][FromQuery] string customerCode)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await stockTransferService.CreateStockTransfer(customerCode, User.Identity.Name, User.GetWHSCode()));
        }

        /// <summary>
        /// Add Stock Transfer detail records for selected PIDs
        /// </summary>
        /// <param name="dto">Data object containing relevant JobNo and a list of PIDs to add</param>
        /// <returns>True</returns>
        /// <response code="400">Invalid input model data</response>
        /// <response code="404">Relevant data record was not found</response>
        [HttpPost]
        [Route("addStockTransferDetailByPID")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<bool>> AddStockTransferDetailByPID([RequiredAsJsonError] StockTransferDetailByPIDDto dto)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await stockTransferService.AddStockTransferDetailByPID(dto, User.Identity.Name));
        }

        /// <summary>
        /// Delete Stock Transfer detail records for selected PIDs
        /// </summary>
        /// <param name="dto">Data object containing relevant JobNo and a list of PIDs to delete</param>
        /// <returns>True</returns>
        /// <response code="400">Invalid input model data</response>
        /// <response code="404">Relevant data record was not found</response>
        [HttpPost]
        [Route("deleteStockTransferDetailByPID")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<bool>> DeleteStockTransferDetailByPID([RequiredAsJsonError] StockTransferDetailByPIDDto dto)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await stockTransferService.DeleteStockTransferDetailByPID(dto));
        }

        /// <summary>
        /// Delete Stock Transfer detail record by line number
        /// </summary>
        /// <param name="jobNo">ST JobNo</param>
        /// <param name="lineItem">Line item to delete</param>
        /// <returns>True</returns>
        /// <response code="400">Invalid input model data or jobNo parameter is empty</response>
        /// <response code="404">Relevant data record was not found</response>
        [ServiceFilter(typeof(CheckIfLockedFilter))]
        [HttpPost]
        [Route("deleteStockTransferDetail")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<bool>> DeleteStockTransferDetail([RequiredAsJsonError] string jobNo, [RequiredAsJsonError] int lineItem)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await stockTransferService.DeleteStockTransferDetail(jobNo, lineItem));
        }

        /// <summary>
        /// Update Stock Transfer entry
        /// </summary>
        /// <param name="jobNo">ST JobNo</param>
        /// <param name="stockTransferDto">ST dto to update</param>
        /// <returns>ST DTO updated</returns>
        /// <response code="400">Invalid input model data or jobNo parameter is empty</response>
        /// <response code="404">Relevant data record was not found</response>
        [ServiceFilter(typeof(CheckIfLockedFilter))]
        [HttpPatch]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<StockTransferDto>> UpdateStockTransfer([FromQuery][RequiredAsJsonError] string jobNo, StockTransferDto stockTransferDto)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await stockTransferService.UpdateStockTransfer(jobNo, stockTransferDto));
        }

        /// <summary>
        /// Import eKanban data to Stock Transfer
        /// </summary>
        /// <param name="orderNo">Order No to import</param>
        /// <returns>comma-separated list of JobNos created</returns>
        /// <response code="400">JobNo is required</response>
        /// <response code="404">Relevant data record was not found</response>
        [HttpPost]
        [Route("importEKanban")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<string>> ImportEKanbanEUCPart([RequiredAsJsonError] string orderNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await stockTransferService.ImportEKanbanEUCPart(orderNo, User.GetWHSCode(), User.Identity.Name));
        }

        /// <summary>
        /// Import multiple EKanban order numbers to Stock Transfer
        /// </summary>
        /// <param name="data">List of order nos to import</param>
        /// <returns>comma-separated list of JobNos created</returns>
        /// <response code="400">Invalid input model data</response>
        /// <response code="404">Relevant data record was not found</response>
        [HttpPost]
        [Route("importEKanbanEUCPartMulti")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<string>> ImportEKanbanEUCPartMulti(OrderNosDto data)
        {
            var allowedMethods = stockTransferService.GetAllowedSTFImportMethods();
            if (!allowedMethods.AllowEKanbanImport) 
                return FromResult(new InvalidResult<string>(new JsonResultError("OperationNotPermitted").ToJson()));
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await stockTransferService.ImportEKanbanEUCPartMulti(data.OrderNos, User.GetWHSCode(), User.Identity.Name));
        }

        /// <summary>
        /// Imports eStockTransfer data as Stock Transfer
        /// </summary>
        /// <param name="orderNo">Order No to import</param>
        /// <returns>JobNo created</returns>
        /// <response code="400">OrderNo is required</response>
        /// <response code="404">Relevant data record was not found</response>
        [HttpPost]
        [Route("importEStockTransfer")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<string>> ImportEStockTransfer([RequiredAsJsonError] string orderNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await stockTransferService.ImportEStockTransfer(orderNo, User.GetWHSCode(), User.Identity.Name));
        }

        /// <summary>
        /// Import multiple eStockTransfer order numbers to Stock Transfer
        /// </summary>
        /// <param name="data">List of order nos to import</param>
        /// <returns>comma-separated list of JobNos created</returns>
        /// <response code="400">Invalid input model data</response>
        /// <response code="404">Relevant data record was not found</response>
        [HttpPost]
        [Route("importEStockTransferMulti")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<string>> ImportEStockTransferMulti([RequiredAsJsonError] OrderNosDto data)
        {
            var allowedMethods = stockTransferService.GetAllowedSTFImportMethods();
            if (!allowedMethods.AllowEStockTransferImport)
                return FromResult(new InvalidResult<string>(new JsonResultError("OperationNotPermitted").ToJson()));
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await stockTransferService.ImportEStockTransferMulti(data.OrderNos, User.GetWHSCode(), User.Identity.Name));
        }

        /// <summary>
        /// Cancel stock transfer
        /// </summary>
        /// <param name="jobNo">ST JobNo</param>
        /// <returns>True</returns>
        /// <response code="400">JobNo is required</response>
        /// <response code="404">Relevant data record was not found</response>
        [ServiceFilter(typeof(CheckIfLockedFilter))]
        [HttpPost]
        [Route("cancel")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<bool>> Cancel([RequiredAsJsonError] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await stockTransferService.Cancel(jobNo, User.Identity.Name));
        }

        /// <summary>
        /// Complete stock transfer
        /// </summary>
        /// <param name="jobNo">ST JobNo</param>
        /// <returns>True</returns>
        /// <response code="400">JobNo is required</response>
        /// <response code="404">Relevant data record was not found</response>
        [ServiceFilter(typeof(CheckIfLockedFilter))]
        [HttpPost]
        [Route("complete")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<bool>> Complete([RequiredAsJsonError] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await stockTransferService.Complete(jobNo, User.Identity.Name));
        }

        /// <summary>
        /// Split stock transfer detail rows by grouping them by inbound date
        /// </summary>
        /// <param name="jobNo">ST JobNo</param>
        /// <returns>True</returns>
        /// <response code="400">JobNo is required</response>
        /// <response code="404">Relevant data record was not found</response>
        [ServiceFilter(typeof(CheckIfLockedFilter))]
        [HttpPost]
        [Route("splitByInboundDate")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<bool>> SplitByInboundDate([RequiredAsJsonError][FromQuery] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await stockTransferService.SplitByInboundDate(jobNo, User.Identity.Name));
        }

        /// <summary>
        /// Get Stock Transfer Report as pdf
        /// <param name="jobNo">ST JobNo</param>
        /// <returns>file stream</returns>
        /// <response code="400">JobNo is required</response>
        [HttpGet]
        [Route("getStockTransferReport")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> GetStockTransferReport([RequiredAsJsonError] string jobNo)
            => new FileStreamResult(await stockTransferService.StockTransferReport(User.GetWHSCode(), jobNo), "application/pdf")
            {
                FileDownloadName = await stockTransferService.StockTransferReportFileName(jobNo)
            };

        /// <summary>
        /// Get Stock Transfer EDT list as csv
        /// </summary>
        /// <param name="jobNo">ST JobNo</param>
        /// <returns>file stream</returns>
        /// <response code="400">JobNo is required</response>
        [HttpGet]
        [Route("downloadEDTToCSV")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DownloadEDTToCSV([RequiredAsJsonError] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromStreamResult(await stockTransferService.DownloadEDTToCSV(jobNo), $"{jobNo}.txt", "text/plain");
        }

        /// <summary>
        /// Returns allowed import methods for Stock Transfers
        /// </summary>
        /// <returns>AllowedImportMethods dto</returns>
        [HttpGet]
        [Route("allowedSTFImportMethods")]
        [ProducesResponseType(200)]
        public ActionResult<AllowedSTFImportMethodsDto> GetAllowedSTFImportMethods()
        {
            return stockTransferService.GetAllowedSTFImportMethods();
        }

        private readonly IStockTransferService stockTransferService;
    }
}
