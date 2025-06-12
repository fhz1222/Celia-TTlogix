using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TT.Controllers.Authorization
{
    public class ModuleAccessAuthorizationHandler : AuthorizationHandler<ModulePrefixRequirement>
    {
        public ModuleAccessAuthorizationHandler() { }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ModulePrefixRequirement requirement)
        {
            var roles = context.User.FindAll(c => c.Type == ClaimTypes.Role).Select(r => r.Value.ToUpper()).ToList();
            if (roles != null)
            {
                foreach (var requiredRole in requirement.Prefixes.Split(','))
                {
                    if (roles.Any(r => r.StartsWith(requiredRole.ToUpper())))
                    {
                        context.Succeed(requirement);
                        break;
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
