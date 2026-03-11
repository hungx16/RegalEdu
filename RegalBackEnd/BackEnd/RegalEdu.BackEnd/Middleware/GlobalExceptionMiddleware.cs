using RegalEdu.Application.Common.Exceptions;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Shared;
using System.Net;
using System.Text.Json;

namespace RegalEdu.Api.Middleware
{
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
                await _next(context); // chạy tiếp pipeline
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Service provider for resolving scoped services
            var localizationService = context.RequestServices.GetRequiredService<ILocalizationService>();

            // Lấy IP
            var ipAddress = Application.Common.Logging.LoggerExtensions.HttpContextAccessor?.HttpContext?.Items["RequestIpAddress"]?.ToString()
                         ?? Application.Common.Logging.LoggerExtensions.HttpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString()
                         ?? "Unknown IP";

            // Lấy UserId
            var userId = context.User?.FindFirst("sub")?.Value
                      ?? context.User?.FindFirst("userid")?.Value
                      ?? context.User?.Identity?.Name
                      ?? "Anonymous";

            // Lấy Path
            var path = context.Request?.Path.Value ?? "Unknown path";

            // Log error đầy đủ
            _logger.LogError(exception,
                "[ERROR] Unhandled exception from IP: {IpAddress} | User: {UserId} | Path: {Path} | Exception: {ExceptionMessage}",
                ipAddress, userId, path, exception.Message);

            context.Response.ContentType = "application/json";

            var response = new ApiResponse<object>();
            response.Succeeded = false;
            switch (exception)
            {
                case SimpleValidationException simpleValidationEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusCode = context.Response.StatusCode;
                    response.Errors = simpleValidationEx.Errors;
                    response.Message = "Validation failed.";
                    break;

                case ValidationException fluentValidationEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusCode = context.Response.StatusCode;
                    response.Errors = fluentValidationEx.Errors
                        .SelectMany(e => e.Value)
                        .ToList();
                    response.Message = "Validation failed.";
                    break;

                case NotFoundException notFoundEx:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusCode = context.Response.StatusCode;
                    response.Message = notFoundEx.Message;
                    break;

                case UnauthorizedAccessException unauthorizedEx:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.StatusCode = context.Response.StatusCode;
                    response.Message = unauthorizedEx.Message;
                    break;

                case Exception ex:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.StatusCode = context.Response.StatusCode;
                    response.Message = "Internal server error";
                    response.Errors = new List<string> { Functions.GetFullExceptionMessage(ex) }; 
                    break;
            }

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}
