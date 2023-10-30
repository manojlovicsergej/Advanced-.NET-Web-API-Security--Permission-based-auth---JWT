using System.Net;
using System.Text.Json;
using Application.Exceptions;
using Common.Responses;

namespace WebApi.Middlewares;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception e)
        {
            var response = httpContext.Response;
            response.ContentType = "application/json";

            Error error = new();

            switch (e)
            {
                case CustomValidationException vex:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    error.FriendlyErrorMessage = vex.FriendlyErrorMessage;
                    error.ErrorMessages = vex.ErrorMessages;
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    error.FriendlyErrorMessage = e.Message;
                    break;
            }

            var result = JsonSerializer.Serialize(error);
            await response.WriteAsync(result);
        }
    }
}