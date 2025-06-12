using System.Security.Claims;

namespace TT.Controllers.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetWHSCode(this ClaimsPrincipal user)
        {
            var userData = user.FindFirst((c) => c.Type == ClaimTypes.UserData);
            return userData?.Value;
        }
    }
}
