using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Presentation.Utilities.Interfaces;

namespace Presentation.Utilities
{
    public class CheckIfLockedFilter : IAsyncActionFilter
    {
        private readonly ILocksService locksService;

        public CheckIfLockedFilter(ILocksService locksService)
            => this.locksService = locksService;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionArguments.ContainsKey("jobNo") && context.ActionArguments.ContainsKey("userCode"))
            {
                var jobNo = context.ActionArguments["jobNo"].ToString();
                var userCode = context.ActionArguments["userCode"].ToString();
                var currentLock = await locksService.GetAccessLock(jobNo);
                if (currentLock != null && currentLock.UserCode != userCode)
                {
                    context.Result = new BadRequestObjectResult($"Locked By: {currentLock.UserCode}");
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
