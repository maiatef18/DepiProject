using Mos3ef.Api.Exceptions;
using System.Net;
using System.Text.Json;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await HandleException(context, ex);
        }
    }

    private Task HandleException(HttpContext context, Exception ex)
    {
        HttpStatusCode statusCode;
        string message = ex.Message;
        object errors = null;

        switch (ex)
        {
            case ValidationException ve:
                statusCode = HttpStatusCode.BadRequest;
                errors = ve.Errors;
                break;

            case BadRequestException:
                statusCode = HttpStatusCode.BadRequest;
                break;

            case NotFoundException:
                statusCode = HttpStatusCode.NotFound;
                break;

            case UnauthorizedException:
                statusCode = HttpStatusCode.Unauthorized;
                break;

            default:
                statusCode = HttpStatusCode.InternalServerError;
                message = "An unexpected error occurred";
                break;
        }

        var response = new
        {
            status = (int)statusCode,
            message,
            errors
        };

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
