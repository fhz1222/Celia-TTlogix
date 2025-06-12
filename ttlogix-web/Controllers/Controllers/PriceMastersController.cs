using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TT.Controllers.Extensions;
using TT.Services.Interfaces;
using TT.Services.Models;

namespace TT.Controllers.Controllers
{
    [Authorize]
    [Route("api/priceMasters")]
    [ApiController]
    public class PriceMastersController : TTLogixControllerBase
    {

        public PriceMastersController(IPriceMasterService priceMasterService)
            => this.priceMasterService = priceMasterService;

        /// <summary>
        /// updates price master and storage detail selling price
        /// original endpoint: Outbound detail > Update Stock Value (btnUpdatePriceMaster_Click)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updatePriceMasterOutbound")]
        public async Task<ActionResult> UpdatePriceMasterOutbound(PriceMasterOutboundUpdateDto data)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await priceMasterService.UpdatePriceMasterOutbound(data.PickingListForPriceMasterDtos, data.CustomerCode, data.JobNo, User.Identity.Name));
        }

        /// <summary>
        /// updates price master
        /// original endpoint: Inbound detail > Update Price Master (btnUpdatePrice_Click)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updatePriceMasterInbound")]
        public async Task<ActionResult> UpdatePriceMasterInbound(PriceMasterInboundUpdateDto data)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await priceMasterService.UpdatePriceMasterInbound(data.InboundDetailForPriceMasterDtos, data.CustomerCode, data.SupplierID, data.JobNo, User.Identity.Name, data.Currency));
        }

        private readonly IPriceMasterService priceMasterService;
    }
}
