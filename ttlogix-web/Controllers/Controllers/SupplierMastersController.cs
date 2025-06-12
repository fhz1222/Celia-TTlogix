using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TT.Common;
using TT.Controllers.Authorization;
using TT.Controllers.Extensions;
using TT.Core.Interfaces;
using TT.Services.Models;
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
    [Route("api/supplierMasters")]
    [ApiController]
    public class SupplierMastersController : TTLogixControllerBase
    {
        public SupplierMastersController(ITTLogixRepository repository, IMapper mapper)
            => (this.repository, this.mapper) = (repository, mapper);

        /// <summary>
        /// Get supplier master simplified object
        /// </summary>
        /// <param name="factoryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getList")]
        public async Task<ActionResult<IEnumerable<SupplierMasterBasicDto>>> List([RequiredAsJsonError] string factoryId)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }

            var suppliers = await repository.GetSupplierMasterListAsync(factoryId);
            return suppliers == null ? NotFound() : Ok(mapper.Map<IEnumerable<SupplierMasterBasicDto>>(suppliers.OrderBy(s => s.CompanyName)));
        }

        private readonly ITTLogixRepository repository;
        private readonly IMapper mapper;
    }
}
