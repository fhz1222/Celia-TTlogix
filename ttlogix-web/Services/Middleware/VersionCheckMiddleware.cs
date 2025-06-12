using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using TT.Common;

namespace TT.Services.Middleware
{
    public class VersionCheckMiddleware
    {
        private readonly RequestDelegate next;
        private readonly string version;
        private const string APP_VERSION_HEADER = "AppVersion";

        public VersionCheckMiddleware(RequestDelegate next)
        {
            this.next = next;
            this.version = Assembly.GetEntryAssembly()?.GetName().Version.ToString();
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                var frotendVersion = context.Request.Headers[APP_VERSION_HEADER].FirstOrDefault();
                if (frotendVersion != this.version)
                {

                    context.Response.Clear();
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync("UnsupportedAppVersion");
                    return;
                }
            }
            await next(context);
        }
    }
}
