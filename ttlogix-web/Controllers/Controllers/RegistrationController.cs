using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TT.Controllers.Extensions;
using TT.Core.Interfaces;
using TT.Services.Interfaces;
using TT.Services.Models;
using TT.Services.Services.Utilities;
using TT.Services.Services;

namespace TT.Controllers.Controllers
{
    [Authorize]
    [Route("api/registration")]
    [ApiController]
    public class RegistrationController : TTLogixControllerBase
    {
        public RegistrationController(IRegistrationService registrationService) => this.registrationService = registrationService;

        /// <summary>
        /// Print Location labels on selected printer
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("printLocationLabels")]
        public async Task<ActionResult> PrintLocationLabels([RequiredAsJsonError] PrintLocationLabelsDto data)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await registrationService.PrintLocationLabels(data.Codes, data.Printer, data.Copies));
        }

        /// <summary>
        /// Get Location labels
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getLocationLabels")]
        public async Task<ActionResult<IEnumerable<QRCodeDto>>> GetLocationLabels([FromBody] GetLocationQRsDto data)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await registrationService.GetLocationLabels(data.Codes));
        }

        private readonly IRegistrationService registrationService;
    }
}
