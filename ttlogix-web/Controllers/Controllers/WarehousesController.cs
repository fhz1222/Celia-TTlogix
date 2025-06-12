using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TT.Core.Interfaces;
using TT.Services.Models;
using TT.Services.Services.Utilities;

namespace TT.Controllers.Controllers
{
    [Route("api/warehouses")]
    [ApiController]
    public class WarehousesController : TTLogixControllerBase
    {
        public WarehousesController(ITTLogixRepository repository, IMapper mapper)
            => (this.repository, this.mapper) = (repository, mapper);

        /// <summary>
        /// Get list of active warehouses
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WarehouseDto>>> Get()
            => Ok(await repository.Warehouses().Where(w => w.Status == 1).Select(w => mapper.Map<WarehouseDto>(w)).ToListAsync());

        /// <summary>
        /// Get list of locations for warehouse
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("locations")]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetLocations([RequiredAsJsonError] string whsCode)
            => Ok(await repository.Locations().Where(w => w.Status == 1 && w.WHSCode == whsCode).Select(w => mapper.Map<LocationDto>(w)).ToListAsync());


        private readonly ITTLogixRepository repository;
        private readonly IMapper mapper;
    }
}
