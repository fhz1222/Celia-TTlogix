using Microsoft.AspNetCore.Authorization;

namespace TT.Controllers.Authorization
{
    public class ModulePrefixRequirement : IAuthorizationRequirement
    {
        public string Prefixes { get; private set; }

        public ModulePrefixRequirement(string prefixes) { Prefixes = prefixes; }
    }
}
