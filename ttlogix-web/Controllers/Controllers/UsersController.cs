using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TT.Common;
using TT.Controllers.Authorization;
using TT.Controllers.Extensions;
using TT.Core.QueryFilters;
using TT.Services.Interfaces;
using TT.Services.Models;
using TT.Services.Services.Utilities;

namespace TT.Controllers.Controllers
{
    [ModuleAccessAuthorize(SystemModuleNames.USER)]
    [Route("api/users")]
    [ApiController]
    public class UsersController : TTLogixControllerBase
    {
        public UsersController(IUserManagementService userService)
            => this.userService = userService;

        /// <summary>
        /// Get list of users
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<UserListDto>> GetUsersList([FromQuery] UserListQueryFilter queryFilter)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }

            queryFilter.WHSCode = User.GetWHSCode();
            return Ok(await userService.GetUsersList(queryFilter));
        }

        /// <summary>
        /// Get user details for code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getUser")]
        public async Task<ActionResult<UserDto>> GetUser([RequiredAsJsonError] string code)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }

            var user = await userService.GetUser(code);
            return user == null ? NotFound() : Ok(user);
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="code"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<ActionResult> PatchUser([FromQuery] string code, UserUpdateDto user)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            if (user.Code != code) { return NotFound(); }

            return FromResult(await userService.UpdateUser(user, User.Identity.Name));
        }

        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateUser(UserAddDto user)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await userService.CreateUser(user, User.Identity.Name));
        }

        /// <summary>
        /// Activate/Deactivate user
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("toggleStatus")]
        public async Task<ActionResult> ToggleStatus([RequiredAsJsonError][FromQuery] string code)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await userService.ToggleUserStatus(code, User.Identity.Name));
        }

        /// <summary>
        /// get user report pdf
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getUserReport")]
        public async Task<FileStreamResult> GetUserReport()
            => new FileStreamResult(await userService.UserReport(User.GetWHSCode()), "application/pdf")
            {
                FileDownloadName = "UserReport.pdf"
            };

        private readonly IUserManagementService userService;
    }
}
