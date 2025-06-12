using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TT.Controllers.Extensions;
using TT.Core.QueryFilters;
using TT.Services.Interfaces;
using TT.Services.Models;
using TT.Services.Services.Utilities;

namespace TT.Controllers.Controllers
{
    [Authorize]
    [Route("api/eStockTransfers")]
    [ApiController]
    public class EStockTransfersController : TTLogixControllerBase
    {
        public EStockTransfersController(IEStockTransferService eStockTransferService)
            => this.eStockTransferService = eStockTransferService;

        /// <summary>
        /// gets the list of eStockTransfers - for Hungary
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns>EKanbanListDto</returns>
        [HttpGet]
        [Route("getEStockTransferList")]
        public async Task<ActionResult<EStockTransferListDto>> GetEStockTransferList([FromQuery] EStockTransferListQueryFilter queryFilter)
        {
            queryFilter.UserWHSCode = User.GetWHSCode();
            return Ok(await eStockTransferService.GetEStockTransferList(queryFilter));
        }

        /// <summary>
        /// Get the list of EStockTransfer for checking
        /// for TESAH (Hungary)
        /// originally the endpoint for ImportEStockTransferConfirmation grid
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getEStockTransferPartsStatusByOwnership")]
        public async Task<ActionResult<IEnumerable<EStockTransferPartsStatusDto>>> GetEStockTransferPartsStatusByOwnership([RequiredAsJsonError] string orderNo)
            => Ok(await eStockTransferService.GetEStockTransferPartsStatusByOwnership(orderNo));

        /// <summary>
        /// check if there are any discrepancies in the eStock
        /// original endpoint: l_oInventoryCtrl.GetEStockTransferDiscrepancyList
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("hasAnyEStockTransferDiscrepancy")]
        public async Task<ActionResult<bool>> HasAnyEStockTransferDiscrepancy([RequiredAsJsonError] string jobNo)
            => Ok(await eStockTransferService.HasAnyEStockTransferDiscrepancy(jobNo));


        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("EStockTransferCheck")]
        public async Task<ActionResult<EStockTransferPartsStatusDto>> EStockTransferCheck(OrderNosDto data)
        {
            return Ok(await eStockTransferService.EStockTransferCheck(data.OrderNos));
        }

        private readonly IEStockTransferService eStockTransferService;
    }
}
