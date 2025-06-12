using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace TT.Controllers.Authorization
{
    public class ModuleAccessPolicyProvider : IAuthorizationPolicyProvider
    {
        const string POLICY_PREFIX = "Module";

        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        public ModuleAccessPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(POLICY_PREFIX, StringComparison.OrdinalIgnoreCase))
            {
                var modulePrefix = policyName.Substring(POLICY_PREFIX.Length);
                var policy = new AuthorizationPolicyBuilder();
                policy.RequireAuthenticatedUser();
                policy.AddRequirements(new ModulePrefixRequirement(modulePrefix));
                return Task.FromResult(policy.Build());
            }
            return FallbackPolicyProvider.GetPolicyAsync(policyName);
        }
    }
}
