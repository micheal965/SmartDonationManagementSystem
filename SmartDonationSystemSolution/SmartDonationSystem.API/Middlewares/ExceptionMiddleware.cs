using System.Net;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly IWebHostEnvironment _env;
    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionMiddleware> logger;

    public ExceptionMiddleware(IWebHostEnvironment env, RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _env = env;
        this.next = next;
        this.logger = logger;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            //in case of production-> you need to log error in Database
            await HandleExceptionAsync(context, ex);
        }
    }
    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.Headers.ContentType = "application/json";
        HttpStatusCode statusCode;
        switch (ex)
        {
            case ArgumentNullException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                statusCode = HttpStatusCode.BadRequest;
                break;

            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                statusCode = HttpStatusCode.Unauthorized;
                break;

            case KeyNotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                statusCode = HttpStatusCode.NotFound;
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                statusCode = HttpStatusCode.InternalServerError;
                break;
        }

        var response = _env.IsDevelopment() ? Result<object>.StatusCode(statusCode, ex.Message, ex.StackTrace.ToString())
                                            : Result<object>.StatusCode(statusCode, ex.Message);
        return context.Response.WriteAsJsonAsync(response);
    }
}
