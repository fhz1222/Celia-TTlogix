using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TT.Core.Interfaces;
using TT.Services.Models;

namespace TT.Controllers.Controllers
{
    [Authorize]
    [Route("api/packageTypes")]
    [ApiController]
    public class PackageTypesController : TTLogixControllerBase
    {
        public PackageTypesController(ITTLogixRepository repository) => this.repository = repository;

        /// <summary>
        /// Get list of package types
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PackageTypeListItemDto>>> Get()
            => Ok(await repository.GetPackageTypes<PackageTypeListItemDto>());

        private readonly ITTLogixRepository repository;
    }
}
