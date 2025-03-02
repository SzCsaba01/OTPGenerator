using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using System.Security.Authentication;
using OTPGenerator.Services.Business.Exceptions;

namespace User.API.Infrastructure.Middleware;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
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
        catch (Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            switch (exception)
            {
                case InvalidPasswordException e:
                    response.StatusCode = (int) e.Code;
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            _logger.LogError(exception, exception.Message);

            var result = JsonSerializer.Serialize(new { message = exception?.Message });
            await response.WriteAsync(result);
        }
    }
}
