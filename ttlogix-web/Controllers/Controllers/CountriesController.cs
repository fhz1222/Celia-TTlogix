using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TT.Core.Interfaces;
using TT.Services.Models;

namespace TT.Controllers.Controllers
{
    [Authorize]
    [Route("api/countries")]
    [ApiController]
    public class CountriesController : TTLogixControllerBase
    {
        public CountriesController(ITTLogixRepository repository) => this.repository = repository;

        /// <summary>
        /// Get list of countries
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CountryListItemDto>>> Get()
            => Ok(await repository.GetCountries<CountryListItemDto>());

        private readonly ITTLogixRepository repository;
    }
}
