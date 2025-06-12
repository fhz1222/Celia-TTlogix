using Microsoft.AspNetCore.Mvc;
using System;
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
    [ModuleAccessAuthorize(SystemModuleNames.INBOUND)]
    [Route("api/inbounds")]
    [ApiController]
    public class InboundsController : TTLogixControllerBase
    {
        public InboundsController(IInboundService inboundService) => this.inboundService = inboundService;

        /// <summary>
        /// Get the list of inbounds
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getInbounds")]
        public async Task<ActionResult<InboundListDto>> GetInbounds([FromQuery] InboundListQueryFilter queryFilter)
        {
            queryFilter.UserWHSCode = User.GetWHSCode();
            return Ok(await inboundService.GetInboundList(queryFilter));
        }

        /// <summary>
        /// Get inbound for jobNo
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getInbound")]
        public async Task<ActionResult<InboundDto>> GetInbound([RequiredAsJsonError] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }

            var inbound = await inboundService.GetInbound(jobNo);
            return inbound == null ? NotFound() : Ok(inbound);
        }

        /// <summary>
        /// Get inbound detail rows for job no, include the price values
        /// original endpoint: GetInboundDetailListWithPrice
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getInboundDetails")]
        public async Task<ActionResult<IEnumerable<InboundDetailDto>>> GetInboundDetails([RequiredAsJsonError] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return Ok(await inboundService.GetInboundDetailList(jobNo));
        }

        /// <summary>
        /// Create manual inbound
        /// </summary>
        /// <param name="inboundDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createInboundManual")]
        public async Task<ActionResult<string>> CreateInboundManual(InboundManualDto inboundDto)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await inboundService.CreateInboundManual(inboundDto, User.Identity.Name));
        }

        /// <summary>
        /// Updates inbound header
        /// </summary>
        /// <param name="jobNo"></param>
        /// <param name="inboundDto"></param>
        /// <returns></returns>
        [HttpPatch("{jobNo}")]
        [ServiceFilter(typeof(CheckIfLockedFilter))]
        public async Task<ActionResult> PatchInbound([RequiredAsJsonError] string jobNo, [FromBody] InboundDto inboundDto)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await inboundService.UpdateInbound(jobNo, inboundDto, User.Identity.Name));
        }

        /// <summary>
        /// Add inbound detail row (new Pkg Entry)
        /// original endpoint and logic in AddPkgEntry
        /// </summary>
        /// <param name="inboundDetail"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createInboundDetail")]
        public async Task<ActionResult> CreateInboundDetail([FromBody] InboundDetailEntryAddDto inboundDetail)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await inboundService.CreateInboundDetail(inboundDetail, User.GetWHSCode(), User.Identity.Name));
        }

        /// <summary>
        /// Update inbound detail row (new Pkg Entry)
        /// original endpoint and logic in AddPkgEntry
        /// </summary>
        /// <param name="inboundDetail"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("modifyInboundDetail")]
        public async Task<ActionResult> ModifyInboundDetail([FromBody] InboundDetailEntryModifyDto inboundDetail)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await inboundService.UpdateInboundDetail(inboundDetail, User.GetWHSCode(), User.Identity.Name));
        }

        /// <summary>
        /// get list of ASNs to import
        /// original endpoint: m_oWebServiceCtrl.GetASNList
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getASNsToImport")]
        public async Task<ActionResult<ASNListDto>> GetASNsToImport([FromQuery] ASNListQueryFilter queryFilter)
        {
            queryFilter.WHSCode = User.GetWHSCode();
            return Ok(await inboundService.GetASNListToImport(queryFilter));
        }

        /// <summary>
        /// Gets the list of asns
        /// </summary>
        /// <param name="asnNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getASNDetails")]
        public async Task<ActionResult<IEnumerable<ASNDetailSimpleDto>>> GetASNDetails([RequiredAsJsonError] string asnNo)
            => Ok(await inboundService.GetASNDetails(asnNo));

        /// <summary>
        /// Import selected ASN
        /// Note: import by Container No logic made no sense so it is replaced with just 1:1 import of selected ASN
        /// </summary>
        /// <param name="asnNo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("importASN")]
        public async Task<ActionResult<string>> ImportASN([RequiredAsJsonError][FromQuery] string asnNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await inboundService.ImportASN(asnNo, User.GetWHSCode(), User.Identity.Name));
        }

        /// <summary>
        /// Import selected File
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("importFile")]
        public async Task<ActionResult<string[]>> ImportFile([FromForm] InboundUploadFileDto req)
        {
            if (req.File == null)
            {
                return BadRequest(new { message = "inbound.error.select_file" });
            }
            if (ModelState.IsInvalid())
            {
                return ValidationProblem(ModelState);
            }

            var tmp = Path.GetTempFileName();
            var stream = new FileStream(tmp, FileMode.Open);
            try
            {
                req.File.CopyTo(stream);
                stream.Seek(0, SeekOrigin.Begin);

                return FromResult(await inboundService.ImportFile(stream, User.GetWHSCode(), req.CustomerCode, req.SupplierID, User.Identity.Name));
            }
            catch (Exception e3)
            {
                return BadRequest(new { message = e3.Message });
            }
            finally
            {
                stream.Close();
                System.IO.File.Delete(tmp);
            }
        }

        /// <summary>
        /// Cancel inbound
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpPost]
        [ServiceFilter(typeof(CheckIfLockedFilter))]
        [Route("cancelInbound")]
        public async Task<ActionResult<bool>> CancelInbound([RequiredAsJsonError] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await inboundService.CancelInbound(jobNo, User.Identity.Name));
        }

        /// <summary>
        /// Download IDT CSV file
        /// original: InboundDetail => btnDownloadIDT_Click
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getIDTAsCSV")]
        public async Task<IActionResult> GetIDTAsCSV([RequiredAsJsonError] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return ToCSVFileStreamResult(await inboundService.GetIDT(jobNo), $"IDT_{jobNo}.csv");
        }

        /// <summary>
        /// Increase the no of packages for inbound line item
        /// original endpoint: IncreasePkgQty
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("increasePkgQty")]
        public async Task<ActionResult> IncreasePkgQty(AddPkgQtyDto data)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await inboundService.IncreasePkgQty(data.JobNo, data.LineItem, data.Qty, User.Identity.Name));
        }

        /// <summary>
        /// Remove storage PID(s) for the specified inboud record JobNo and LineItem
        /// remove selected PIDs or all PIDs for the selected line (RemoveAll=true)
        /// original endpoint: RemovePID
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("removePIDs")]
        public async Task<ActionResult> RemovePIDs(RemovePIDsDto data)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await inboundService.RemovePIDs(data, User.Identity.Name));
        }

        /// <summary>
        /// Get LocationReport
        /// </summary>
        /// <param name="jobNo">Inbound jobNo</param>
        /// <returns>file stream</returns>
        [HttpGet]
        [Route("getLocationReport")]
        public async Task<ActionResult> GetLocationReport([RequiredAsJsonError] string jobNo)
            => new FileStreamResult(await inboundService.LocationReport(User.GetWHSCode(), jobNo), "application/pdf")
            {
                FileDownloadName = jobNo + "_inbound_location_report.pdf"
            };

        /// <summary>
        /// Get InboundReport
        /// </summary>
        /// <param name="jobNo">Inbound jobNo</param>
        /// <returns>file stream</returns>
        [HttpGet]
        [Route("getInboundReport")]
        public async Task<ActionResult> GetInboundReport([RequiredAsJsonError] string jobNo)
            => new FileStreamResult(await inboundService.InboundReport(User.GetWHSCode(), jobNo), "application/pdf")
            {
                FileDownloadName = jobNo + "_inbound_report.pdf"
            };

        /// <summary>
        /// Get WarehouseInNoteReport
        /// </summary>
        /// <param name="jobNo">Inbound jobNo</param>
        /// <returns>file stream</returns>
        [HttpGet]
        [Route("getWarehouseInNoteReport")]
        public async Task<ActionResult> GetWarehouseInNoteReport([RequiredAsJsonError] string jobNo)
            => new FileStreamResult(await inboundService.WarehouseInNoteReport(User.GetWHSCode(), jobNo), "application/pdf")
            {
                FileDownloadName = jobNo + "_inbound_report.pdf"
            };

        /// <summary>
        /// Get DiscrepancyReport
        /// </summary>
        /// <param name="jobNo">Inbound jobNo</param>
        /// <returns>file stream</returns>
        [HttpGet]
        [Route("getDiscrepancyReport")]
        public async Task<ActionResult> GetDiscrepancyReport([RequiredAsJsonError] string jobNo)
        {
            var reportResult = await inboundService.DiscrepancyReport(User.GetWHSCode(), jobNo);
            return FromStreamResult(reportResult, jobNo + "_inbound_discrepancy_report.pdf", "application/pdf");
        }

        [HttpGet]
        [Route("getOutstandingInboundsXlsReport")]
        public async Task<ActionResult> GetOutstandingInboundsXlsReport()
        {
            var reportStream = await inboundService.GetOutstandingInboundsXlsReport(User.GetWHSCode());
            return new FileStreamResult(reportStream, "application/octet-stream")
            {
                FileDownloadName = "outstanding_inbound_report.xlsx"
            };
        }

        private readonly IInboundService inboundService;
    }
}
