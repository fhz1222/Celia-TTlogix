using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
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
    [ModuleAccessAuthorize(SystemModuleNames.GROUP)]
    [Route("api/accessGroups")]
    [ApiController]
    public class AccessGroupsController : TTLogixControllerBase
    {
        public AccessGroupsController(IUserManagementService userService) => this.userService = userService;

        /// <summary>
        /// Get simple list of access groups
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccessGroupSimpleDto>>> GetAccessGroups([FromQuery] AccessGroupFilter filter = null)
            => Ok(await userService.GetAccessGroups(filter));

        /// <summary>
        /// Get access group
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getAccessGroup")]
        public async Task<ActionResult<AccessGroupDto>> GetAccessGroup([RequiredAsJsonError] string code)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            var accessGroup = await userService.GetAccessGroup(code);
            return accessGroup == null ? NotFound() : Ok(accessGroup);
        }

        /// <summary>
        /// Update access group
        /// </summary>
        /// <param name="code"></param>
        /// <param name="accessGroup"></param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<ActionResult> PatchAccessGroup([FromQuery] string code, AccessGroupDto accessGroup)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            if (accessGroup.Code != code) { return NotFound(); }
            return FromResult(await userService.UpdateAccessGroup(accessGroup, User.Identity.Name));
        }

        /// <summary>
        /// Create access group
        /// </summary>
        /// <param name="accessGroup"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateAccessGroup(AccessGroupAddDto accessGroup)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await userService.CreateAccessGroup(accessGroup, User.Identity.Name));
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
            return FromResult(await userService.ToggleAccessGroupStatus(code, User.Identity.Name));
        }

        /// <summary>
        /// Get privileges tree
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getPrivilegesTree")]

        public async Task<ActionResult<SystemModuleTreeDto>> GetPrivilegesTree([RequiredAsJsonError] string groupCode)
            => Ok(await userService.GetPrivilegesTree(groupCode));

        /// <summary>
        /// Update privileges tree
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("updatePrivilegesTree")]

        public async Task<ActionResult<bool>> UpdatePrivilegesTree([FromQuery] string groupCode, SystemModuleTreeDto tree)
        {
            if (ModelState.IsInvalid()) { return ValidationProblem(ModelState); }
            return FromResult(await userService.UpdatePrivilegesTree(groupCode, tree));
        }

        private readonly IUserManagementService userService;
    }
}
