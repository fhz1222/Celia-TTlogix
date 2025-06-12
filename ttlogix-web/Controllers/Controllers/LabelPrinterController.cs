using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TT.Core.Interfaces;
using TT.Services.Models;

namespace TT.Controllers.Controllers
{
    [Authorize]
    [Route("api/labelPrinters")]
    [ApiController]
    public class LabelPrinterController : TTLogixControllerBase
    {
        public LabelPrinterController(ITTLogixRepository repository) => this.repository = repository;

        /// <summary>
        /// Get list of printers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LabelPrinterDto>>> Get()
            => Ok(await repository.LabelPrinters()
                .Select(l => new LabelPrinterDto { IP = l.IP, Name = l.Name })
                .ToListAsync());

        private readonly ITTLogixRepository repository;
    }
}
