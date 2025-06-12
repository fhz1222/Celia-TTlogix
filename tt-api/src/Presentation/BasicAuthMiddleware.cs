using System.Net;
using System.Net.Http.Headers;
using System.Text;

public class BasicAuthMiddleware
{
    private readonly RequestDelegate next;
    private readonly IConfiguration config;

    public BasicAuthMiddleware(RequestDelegate next, IConfiguration config)
        => (this.next, this.config) = (next, config);

    public async Task Invoke(HttpContext context)
    {
        try
        {
            var auth = context.Request.Headers["Authorization"];
            var authHeader = AuthenticationHeaderValue.Parse(auth);
            var credentialBytes = Convert.FromBase64String(authHeader?.Parameter ?? string.Empty);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
            var clientId = credentials[0];
            var secret = credentials[1];

            if (IsNotAuthenticated(clientId, secret))
            {
                throw new Exception("Access denied");
            }
        }
        catch
        {
            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Unable to authenticate");
            return;
        }

        await next(context);
    }

    private bool IsNotAuthenticated(string clientId, string secret)
        => !(clientId == config["ClientId"] && secret == config["ClientSecret"]);
}