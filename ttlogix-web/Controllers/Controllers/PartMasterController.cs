using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TT.Common;
using TT.Controllers.Authorization;
using TT.Controllers.Extensions;
using TT.Core.Entities;
using TT.Core.QueryFilters;
using TT.Services.Interfaces;
using TT.Services.Models;
using TT.Services.Services;
using TT.Services.Services.Utilities;

namespace TT.Controllers.Controllers
{
    [ModuleAccessAuthorize(
        SystemModuleNames.PARTMASTER + "," +
        SystemModuleNames.STOCKTRANSFER + "," +
        SystemModuleNames.OUTBOUND + "," +
        SystemModuleNames.INBOUND + "," +
        SystemModuleNames.INVENTORY + "," +
        SystemModuleNames.LOADING
        )]
    [Route("api/partMasters")]
    [ApiController]
    public class PartMasterController : TTLogixControllerBase
    {
        public PartMasterController(IInventoryService inventoryService)
            => this.inventoryService = inventoryService;

        /// <summary>
        /// Gets the list of part masters for the customer code and supplier
        /// used in InboundEntry product list dropdown
        /// </summary>
        /// <param name="customerCode"></param>
        /// <param name="supplierID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getPartMasterListBySupplier")]
        public async Task<ActionResult<PartMasterBySupplierDto>> GetPartMasterListBySupplier([RequiredAsJsonError] string customerCode, [RequiredAsJsonError] string supplierID)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return Ok(await inventoryService.GetPartMasterListBySupplier(customerCode, supplierID));
        }

        /// <summary>
        /// Gets the list of part masters for the customer code
        /// used in PartsMaster screen
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getPartMasterList")]
        public async Task<ActionResult<PartMasterListDto>> GetPartMasterList([FromQuery] PartMasterListQueryFilter queryFilter)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return Ok(await inventoryService.GetPartMasterList(queryFilter));
        }

        /// <summary>
        /// Gets the details for given part
        /// used in PartsMaster screen => part details
        /// </summary>
        /// <param name="customerCode"></param>
        /// <param name="productCode1"></param>
        /// <param name="supplierID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getPartMaster")]
        public async Task<ActionResult<PartMasterDto>> GetPartMaster([RequiredAsJsonError] string customerCode, [RequiredAsJsonError] string productCode1, [RequiredAsJsonError] string supplierID)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return Ok(await inventoryService.GetPartMaster(customerCode, productCode1, supplierID));
        }

        /// <summary>
        /// Gets the list of UOMs for dropdowns
        /// used in PartsMaster module
        /// original: CTT_RegistrationController => LoadUOMWithDecimalList
        /// </summary>
        /// <param name="customerCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getUOMListWithDecimal")]
        public async Task<ActionResult<PartMasterDto>> GetUOMListWithDecimal([RequiredAsJsonError] string customerCode)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return Ok(await inventoryService.GetUOMListWithDecimal(customerCode));
        }

        /// <summary>
        /// Updates the part master object
        /// </summary>
        [HttpPatch("updatePartMaster")]
        public async Task<ActionResult> PatchPartMaster([FromBody] PartMasterDto partMasterDto)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await inventoryService.UpdatePartMaster(partMasterDto));
        }

        /// <summary>
        /// Add part master
        /// </summary>
        /// <param name="partMasterDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createPartMaster")]
        public async Task<ActionResult> PostPartMaster(PartMasterDto partMasterDto)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await inventoryService.CreatePartMaster(partMasterDto, User.Identity.Name));
        }

        [HttpGet("iLogReadinessStatuses")]
        public ActionResult<IEnumerable<string>> GetILogReadinessStatuses() => Ok(UtilityService.ILOG_READINESS_STATUSES);

        [HttpGet("palletTypes")]
        public async Task<ActionResult<IEnumerable<PalletTypeDto>>> GetPalletTypes()
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return Ok(await inventoryService.GetPalletTypes());
        }

        [HttpGet("ELLISPalletTypes")]
        public async Task<ActionResult<IEnumerable<PalletTypeDto>>> GetELLISPalletTypes()
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return Ok(await inventoryService.GetELLISPalletTypes());
        }

        [HttpGet("unloadingPointChoice")]
        public async Task<ActionResult<UnloadingPointChoiceDto>> GetUnloadingPointChoice([RequiredAsJsonError] string customerCode, string supplierID = null)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return Ok(await inventoryService.GetUnloadingPointChoice(customerCode, supplierID));
        }

        private readonly IInventoryService inventoryService;
    }
}
