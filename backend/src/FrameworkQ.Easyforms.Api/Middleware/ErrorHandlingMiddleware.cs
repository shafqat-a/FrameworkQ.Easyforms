namespace FrameworkQ.Easyforms.Api.Middleware;

using System.Net;
using System.Text.Json;

/// <summary>
/// Global error handling middleware
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = context.TraceIdentifier;

        _logger.LogError(exception, "Unhandled exception occurred. CorrelationId: {CorrelationId}", correlationId);

        var response = new
        {
            error = new
            {
                code = GetErrorCode(exception),
                message = exception.Message,
                correlationId = correlationId,
                details = GetErrorDetails(exception)
            }
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = GetStatusCode(exception);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }

    private static string GetErrorCode(Exception exception)
    {
        return exception switch
        {
            ArgumentException => "INVALID_ARGUMENT",
            InvalidOperationException => "INVALID_OPERATION",
            UnauthorizedAccessException => "UNAUTHORIZED",
            FileNotFoundException => "NOT_FOUND",
            _ => "INTERNAL_ERROR"
        };
    }

    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            ArgumentException => (int)HttpStatusCode.BadRequest,
            InvalidOperationException => (int)HttpStatusCode.BadRequest,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            FileNotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError
        };
    }

    private static object? GetErrorDetails(Exception exception)
    {
        // Add specific error details for certain exception types
        if (exception is AggregateException aggregateException)
        {
            return aggregateException.InnerExceptions.Select(e => e.Message).ToList();
        }

        return null;
    }
}
