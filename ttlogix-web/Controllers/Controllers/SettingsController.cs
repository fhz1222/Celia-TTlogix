using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TT.Common;
using TT.Services.Models;

namespace TT.Controllers.Controllers
{
    [Authorize]
    [Route("api/settings")]
    [ApiController]
    public class SettingsController : TTLogixControllerBase
    {
        public SettingsController(IOptions<AppSettings> appSettings, IMapper mapper)
            => (this.appSettings, this.mapper) = (appSettings.Value, mapper);

        /// <summary>
        /// Get list of settings
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<AppSettingsDto> Get()
            => Ok(mapper.Map<AppSettingsDto>(appSettings));

        private readonly AppSettings appSettings;
        private readonly IMapper mapper;
    }
}
