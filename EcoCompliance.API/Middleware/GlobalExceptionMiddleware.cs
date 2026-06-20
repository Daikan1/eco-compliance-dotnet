using System.Net;
using System.Text.Json;
using EcoCompliance.API.Exceptions;

namespace EcoCompliance.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (status, error) = ex switch
        {
            NotFoundException       => (HttpStatusCode.NotFound,           "Not Found"),
            AppValidationException  => (HttpStatusCode.BadRequest,         "Bad Request"),
            _                      => (HttpStatusCode.InternalServerError, "Internal Server Error")
        };

        var body = JsonSerializer.Serialize(new
        {
            timestamp = DateTime.UtcNow,
            status = (int)status,
            error,
            message = ex.Message
        });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;
        return context.Response.WriteAsync(body);
    }
}
