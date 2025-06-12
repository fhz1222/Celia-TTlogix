using Application.Exceptions;
using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace Presentation.Utilities;

public static class ErrorHandler
{
    public static void UseCustomErrors(this IApplicationBuilder app, IHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            app.Use(WriteDevelopmentResponse);
        }
        else
        {
            app.Use(WriteProductionResponse);
        }
    }

    private static Task WriteDevelopmentResponse(HttpContext httpContext, Func<Task> next)
        => WriteResponse(httpContext, includeDetails: true);

    private static Task WriteProductionResponse(HttpContext httpContext, Func<Task> next)
        => WriteResponse(httpContext, includeDetails: false);

    private static async Task WriteResponse(HttpContext httpContext, bool includeDetails)
    {
        var exceptionDetails = httpContext.Features.Get<IExceptionHandlerFeature>();
        var ex = exceptionDetails?.Error;
        if (ex == null) { return; }

        // ProblemDetails has it's own content type
        httpContext.Response.ContentType = "application/problem+json";

        var title = ex.Message;
        var details = includeDetails ? ex.ToString() : null;
        var code = ex is TtlogixApiException || ex is DomainException ? ex.GetType().Name : null;
        var problem = new ProblemDetails
        {
            Type = code,
            Status = httpContext.Response.StatusCode,
            Title = title,
            Detail = details
        };

        // For tracing the request
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        if (traceId != null)
        {
            problem.Extensions["traceId"] = traceId;
        }

        //Serialize the problem details object to the Response as JSON (using System.Text.Json)
        var stream = httpContext.Response.Body;
        await JsonSerializer.SerializeAsync(stream, problem);
    }
}
