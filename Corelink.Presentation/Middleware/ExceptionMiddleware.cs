using Corelink.Application.Contracts;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace corelink_server.Middleware;

public sealed class ExceptionMiddleware(
    RequestDelegate next,
    ILogger<ExceptionMiddleware> logger,
    IHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (KeyNotFoundException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 404;

            var message = environment.IsDevelopment() ? ex.Message : "Not found";
            var response = Answer<string>.NotFound(message);

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unhandled exception while processing {Method} {Path}",
                context.Request.Method,
                context.Request.Path.Value);

            if (context.Response.HasStarted)
            {
                logger.LogWarning("The response has already started, rethrowing the exception.");
                throw;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;

            var message = environment.IsDevelopment() ? ex.Message : "Internal server error";
            var response = Answer<string>.Error(message);

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}