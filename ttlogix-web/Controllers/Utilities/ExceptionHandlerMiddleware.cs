using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace TT.Controllers.Utilities;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionHandlerMiddleware> logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        => (this.next, this.logger) = (next, logger);

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");

            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.BadRequest;

            var operationId = System.Diagnostics.Activity.Current?.RootId;
            var error = ex switch
            { 
                SqlException sqlEx when sqlEx.Number == 1205 => "{\"MessageKey\":\"Deadlock__\",\"Arguments\":{\"id\":\"" + operationId + "\"}}",
                _ => "{\"MessageKey\":\"ExceptionMessage__\",\"Arguments\":{\"id\":\"" + operationId + "\"}}"
            };
            var responseBody = new List<string>() { error };
            await response.WriteAsync(JsonSerializer.Serialize(responseBody));
        }
    }
}
