using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace TT.Controllers
{
    [ApiController]
    [Route("api/version")]
    public class VersionController : ControllerBase
    {

        /// <summary>
        /// Get current user and corresponding roles list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> GetCurrentVersion()
        {
            return Ok(Assembly.GetEntryAssembly().GetName().Version.ToString());
        }

    }
}
