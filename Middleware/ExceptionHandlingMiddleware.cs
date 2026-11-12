using System.Net;
using System.Text.Json;

namespace PolyLearnAI.Api.Middleware;

/// <summary>Bắt mọi exception chưa xử lý và trả về JSON thống nhất.</summary>
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path);

            var (statusCode, message) = ex switch
            {
                BadHttpRequestException badReq => ((int)HttpStatusCode.BadRequest, badReq.Message),
                KeyNotFoundException notFound => ((int)HttpStatusCode.NotFound, notFound.Message),
                UnauthorizedAccessException unauth => ((int)HttpStatusCode.Unauthorized, unauth.Message),
                _ => ((int)HttpStatusCode.InternalServerError, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.")
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var body = new
            {
                status = statusCode,
                message,
                detail = env.IsDevelopment() && statusCode == (int)HttpStatusCode.InternalServerError ? ex.Message : null
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(body, new JsonSerializerOptions(JsonSerializerDefaults.Web)));
        }
    }
}
