using Microsoft.AspNetCore.Authorization;

namespace TT.Controllers.Authorization
{
    public class ModuleAccessAuthorizeAttribute : AuthorizeAttribute
    {
        const string POLICY_PREFIX = "Module";

        public ModuleAccessAuthorizeAttribute(string prefix) => Prefix = prefix;

        public string Prefix
        {
            get => Policy.Substring(POLICY_PREFIX.Length);
            set => Policy = $"{POLICY_PREFIX}{value}";
        }
    }
}
