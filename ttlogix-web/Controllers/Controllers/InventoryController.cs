using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using TT.Common;
using TT.Controllers.Authorization;
using TT.Controllers.Extensions;
using TT.Services.Interfaces;
using TT.Services.Models;
using TT.Services.Services;
using TT.Services.Services.Utilities;

namespace TT.Controllers.Controllers
{
    [ModuleAccessAuthorize(SystemModuleNames.INVENTORY)]
    [Route("api/inventory")]
    [ApiController]
    public class InventoryController : TTLogixControllerBase
    {
        public InventoryController(IInventoryService inventoryService)
        {
            this.inventoryService = inventoryService;
        }

        /// <summary>
        /// Get customer inventory summary data
        /// original endpoint: OutboundDetailItem > GetCustomerInventoryControl() (l_oCustomerCtrl.DisplayInventoryControl, l_oRegistrationCtrl.GetInstance)
        /// </summary>
        /// <param name="customerCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getCustomerInventoryProductCodeMap")]
        public async Task<ActionResult<CustomerInventoryProductCodeMapDto>> GetCustomerInventoryProductCodeMap([RequiredAsJsonError] string customerCode)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return Ok(await inventoryService.GetCustomerInventoryProductCodeMap(customerCode));
        }

        /// <summary>
        /// Gets the correct labels depending on customer code for Inbound detail table
        /// Original method: InboundDetail: LoadInventoryControl
        /// </summary>
        /// <param name="customerCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getCustomerInventoryControlCodeMap")]
        public async Task<ActionResult<CustomerInventoryControlCodeMapDto>> GetCustomerInventoryControlCodeMap([RequiredAsJsonError] string customerCode)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return Ok(await inventoryService.GetCustomerInventoryControlCodeMap(customerCode));
        }

        private readonly IInventoryService inventoryService;
    }
}
