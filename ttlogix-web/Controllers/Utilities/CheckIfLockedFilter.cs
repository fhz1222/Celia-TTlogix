using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using TT.Services.Interfaces;
using TT.Services.Services.Utilities;

namespace TT.Controllers.Utilities
{
    public class CheckIfLockedFilter : IAsyncActionFilter
    {
        private readonly ILocksService locksService;

        public CheckIfLockedFilter(ILocksService locksService)
            => this.locksService = locksService;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionArguments.ContainsKey("jobNo"))
            {
                var jobNo = context.ActionArguments["jobNo"].ToString();
                var currentLock = await locksService.GetAccessLock(jobNo);
                if (currentLock != null && currentLock.UserCode != context.HttpContext.User.Identity.Name)
                {
                    context.Result = new BadRequestObjectResult(new string[] { new JsonResultError("LockedBy__", "userCode", currentLock.UserCode).ToJson() });
                }
                else
                {
                    await next();
                }
            }
            else
            {
                context.Result = new BadRequestObjectResult("Bad parameter");
            }
        }
    }
}
