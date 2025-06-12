using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TT.Controllers.Extensions;
using TT.Services.Interfaces;
using TT.Services.Models;

namespace TT.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        public AuthController(ICurrentUserService currentUserService) => this.currentUserService = currentUserService;

        /// <summary>
        /// Get current user and corresponding roles list
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<CurrentUserDto>> GetCurrentUserWithRoles()
        {
            var user = await currentUserService.CurrentUser();
            return user == null ? Unauthorized(new { message = "Invalid token" }) : Ok(user);
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(AuthRequestDto data)
        {
            if (ModelState.IsInvalid()) { return BadRequest(); }

            var user = await currentUserService.Authenticate(data.Username, data.Password, data.Warehouse);
            return user == null ? BadRequest(new { message = "Username or password is incorrect" }) : Ok(user);
        }

        private readonly ICurrentUserService currentUserService;
    }
}
