using System.Net;
using System.Text.Json;
using Application.Exceptions;
using Common.Responses.Wrappers;

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
            
            var responseWrapper = await ResponseWrapper<string>.FailAsync(e.Message);

            switch (e)
            {
                case CustomValidationException vex:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var result = JsonSerializer.Serialize(responseWrapper);
            await response.WriteAsync(result);
        }
    }
}