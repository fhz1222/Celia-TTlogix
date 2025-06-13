using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TT.Common;
using TT.Controllers.Authorization;
using TT.Controllers.Extensions;
using TT.Core.QueryFilters;
using TT.Services.Interfaces;
using TT.Services.Models;
using TT.Services.Services;
using TT.Services.Services.Utilities;

namespace TT.Controllers.Controllers
{
    [ModuleAccessAuthorize(
        SystemModuleNames.INVENTORY + "," +
        SystemModuleNames.STOCKTRANSFER + "," +
        SystemModuleNames.OUTBOUND + "," +
        SystemModuleNames.INBOUND)]
    [Route("api/ekanbans")]
    [ApiController]
    public class EKanbansController : TTLogixControllerBase
    {
        public EKanbansController(IEKanbanService eKanbanService) => this.eKanbanService = eKanbanService;

        /// <summary>
        /// gets the list of eKanbans for Europe; filtered and paged (main grid > btnEKanban)
        /// original endpoints: OutboundBizFacade.cs > GetEKanbanListForEurope(DataTable p_dtFilter, DataTable p_dtOrderBy), 
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns>EKanbanListDto</returns>
        [HttpGet]
        [Route("getEKanbanListForEurope")]
        public async Task<ActionResult<EKanbanListDto>> GetEKanbanListForEurope([FromQuery] EKanbanListQueryFilter queryFilter)
        {
            queryFilter.UserWHSCode = User.GetWHSCode();
            return Ok(await eKanbanService.GetEKanbanListForEurope(queryFilter));
        }

        /// <summary>
        /// Checks if there are any eKanban records for the JobNo
        /// original endpoint: GetEKanbanListByJobNo(DataTable p_dtFilter, DataTable p_dtOrderBy)
        /// changed from list to bool value
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("hasEKanban")]
        public async Task<ActionResult<bool>> HasEkanban([RequiredAsJsonError] string jobNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return Ok(await eKanbanService.HasEkanbanForJobNo(jobNo));
        }

        /// <summary>
        /// original endpoint: ImportEKanbanConfirmation: oStorageCtrl.GetEKanbanPartsStatusByOwnershipEHP(ref oFilter, ref m_dstDataSet);
        /// - does not go through web service, directly from TT-Logix
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getEKanbanPartsStatusByOwnershipEHP")]
        public async Task<ActionResult<IEnumerable<EKanbanPartsStatusDto>>> GetEKanbanPartsStatusByOwnershipEHP([RequiredAsJsonError] string orderNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return Ok(await eKanbanService.GetEKanbanPartsStatusByOwnershipEHP(orderNo));
        }

        /// <summary>
        /// original endpoint: ImportEKanbanConfirmation: oStorageCtrl.GetEKanbanPartsStatusForCPart(ref oFilter, ref m_dstDataSet);
        /// - does not go through web service, directly from TT-Logix
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getEKanbanPartsStatusForEKanbanCPart")]
        public async Task<ActionResult<IEnumerable<EKanbanPartsStatusDto>>> GetEKanbanPartsStatusForEKanbanCPart([RequiredAsJsonError] string orderNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return Ok(await eKanbanService.GetEKanbanPartsStatusForEKanbanCPart(orderNo));
        }

        /// <summary>
        /// original endpoint: ImportEKanbanConfirmation: oStorageCtrl.GetEKanbanPartsStatusByOwnership(ref oFilter, ref m_dstDataSet);
        /// - does not go through web service, directly from TT-Logix
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getEKanbanPartsStatusByOwnership")]
        public async Task<ActionResult<IEnumerable<EKanbanPartsStatusDto>>> GetEKanbanPartsStatusByOwnership([RequiredAsJsonError] string orderNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return Ok(await eKanbanService.GetEKanbanPartsStatusByOwnership(orderNo));
        }

        /// <summary>
        /// original endpoint: ImportEKanbanConfirmation: oStorageCtrl.GetEKanbanPartsStatusForCPart(ref oFilter, ref m_dstDataSet);
        /// - does not go through web service, directly from TT-Logix
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getEKanbanPartsStatusForCPart")]
        public async Task<ActionResult<IEnumerable<EKanbanPartsStatusDto>>> GetEKanbanPartsStatusForCPart([RequiredAsJsonError] string orderNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return Ok(await eKanbanService.GetEKanbanPartsStatusForCPart(orderNo));
        }

        /// <summary>
        /// original endpoint: ImportEKanbanConfirmation: oStorageCtrl.GetEKanbanPartsStatusForCPartWithoutExt(ref oFilter, ref m_dstDataSet);
        /// - does not go through web service, directly from TT-Logix
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getEKanbanPartsStatusForCPartWithoutExt")]
        public async Task<ActionResult<IEnumerable<EKanbanPartsStatusDto>>> GetEKanbanPartsStatusForCPartWithoutExt([RequiredAsJsonError] string orderNo)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return Ok(await eKanbanService.GetEKanbanPartsStatusForCPartWithoutExt(orderNo));
        }

        /// <summary>
        /// get text file with formatted eKanban data
        /// no endpoint, data received from DataDownloadController.GetEKanbanData(m_strJobNo,out m_arySendData))
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getEKanbanDataToDownload")]
        public async Task<IActionResult> GetEKanbanDataToDownload([RequiredAsJsonError] string orderNumber)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }

            var data = await eKanbanService.GetEKanbanDataToDownload(orderNumber);
            return ToTextFileStreamResult(data, "EKanbanDataToDownload.txt");
        }

        /// <summary>
        /// check if we can  perform import eKanban operation.
        /// original endpoint: CheckEKanbanFulfillable
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("checkEKanbanFulfillable")]
        public async Task<ActionResult<bool>> CheckEKanbanFulfillable(OrderNosDto data)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await eKanbanService.CheckEKanbanFulfillable(data.OrderNos));
        }

        /// <summary>
        /// check if we can  perform import eKanban operation.
        /// original endpoint: CheckEKanbanFulfillable
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("EKanbanCheck")]
        public async Task<ActionResult<EKanbanPartsStatusDto>> EKanbanCheck(OrderNosDto data)
        {
            return Ok(await eKanbanService.EKanbanCheck(data.OrderNos));
        }

        private readonly IEKanbanService eKanbanService;

        [HttpPost]
        [Route("cancelEKanbans")]
        public async Task<ActionResult> CancelEKanbans(OrderNosDto data)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await eKanbanService.CancelEKanbans(data.OrderNos));
        }
    }
}
