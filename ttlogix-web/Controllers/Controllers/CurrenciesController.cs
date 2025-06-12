using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TT.Services.Services;

namespace TT.Controllers.Controllers
{
    [Authorize]
    [Route("api/currencies")]
    [ApiController]
    public class CurrenciesController : TTLogixControllerBase
    {
        public CurrenciesController() { }

        /// <summary>
        /// Get list of currencies
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get() => Ok(UtilityService.CURRENCIES);
    }
}
