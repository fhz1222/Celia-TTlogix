using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TT.Services.Interfaces;
using TT.Services.Models;
using TT.Services.Services.Utilities;

namespace TT.Controllers
{
    [ApiController]
    [Route("api/locks")]
    public class AccessLockController : ControllerBase
    {
        ILocksService locksService;
        ICurrentUserService currentUserService;

        public AccessLockController(ILocksService locksService, ICurrentUserService currentUserService)
        {
            this.locksService = locksService;
            this.currentUserService = currentUserService;
        }


        /// <summary>
        /// Cancel inbound
        /// </summary>
        /// <param name="jobNo"></param>
        /// <param name="moduleName"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("lock")]
        public async Task<ActionResult<bool>> TryLock([RequiredAsJsonError] string jobNo, [RequiredAsJsonError] string moduleName, [RequiredAsJsonError] string clientId)
        {
            var user = await currentUserService.CurrentUser();
            if (user == null) return false;
            return await locksService.TryCreateOrUpdateAccessLock(jobNo, clientId, moduleName, user.Code);
        }
        /// <summary>
        /// Cancel inbound
        /// </summary>
        /// <param name="jobNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("lock")]
        public async Task<ActionResult<AccessLockDto>> GetLock([FromQuery] string jobNo)
        {
            return await locksService.GetAccessLock(jobNo);
        }

        /// <summary>
        /// Cancel inbound
        /// </summary>
        /// <param name="jobNo"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("unlock")]
        public async Task<ActionResult<bool>> TryUnlock([RequiredAsJsonError] string jobNo, [RequiredAsJsonError] string clientId)
        {
            return await locksService.TryDeleteAccessLock(jobNo, clientId);
        }



    }
}
