using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TT.Controllers.Extensions;
using TT.Core.Interfaces;
using TT.Services.Models;

namespace TT.Controllers.Controllers
{
    [Authorize]
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : TTLogixControllerBase
    {
        public CustomersController(ITTLogixRepository repository, IMapper mapper)
            => (this.repository, this.mapper) = (repository, mapper);

        /// <summary>
        /// Get list of customers for current user's WHSCode
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerBasicDto>>> Get()
        {
            var whsCode = User.GetWHSCode();
            var customers = await repository.Customers()
                .Where(c => c.WHSCode == whsCode)
                .OrderBy(c => c.Name)
                .Select(w => mapper.Map<CustomerBasicDto>(w))
                .ToListAsync();
            return Ok(customers);
        }

        private readonly ITTLogixRepository repository;
        private readonly IMapper mapper;
    }
}
