using Corelink.Application.Contracts;
using System.Text.Json;

namespace corelink_server.Middleware;

public sealed class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;

            var response = Answer<string>.Error(ex.Message);

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}