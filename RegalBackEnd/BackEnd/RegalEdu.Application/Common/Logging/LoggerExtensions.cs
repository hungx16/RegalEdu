using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Shared;

namespace RegalEdu.Application.Common.Logging
{
    public static class LoggerExtensions
    {
        public static IHttpContextAccessor? HttpContextAccessor { get; set; }

        public static Result LogAndFail<T>(
            this ILogger<T> logger,
            ILocalizationService localizer,
            string key,
            params object[] args)
        {
            string message = (args == null || args.Length == 0)
                ? localizer[key]
                : localizer.Format(key, args);

            string ipAddress = GetIpAddress();

            logger.LogWarning("[VALIDATION] ==> {Message} | IP: {IpAddress}", message, ipAddress);

            return Result.Failure(message);
        }

        public static Result LogErrorAndFail<T>(
            this ILogger<T> logger,
            ILocalizationService localizer,
            Exception exception,
            string key,
            params object[] args)
        {
            string message = (args == null || args.Length == 0)
                ? localizer[key]
                : localizer.Format(key, args);

            string ipAddress = GetIpAddress();
            string fullExceptionMessage = Functions.GetFullExceptionMessage(exception);

            logger.LogError(exception, "[ERROR] ==> {Message} | FullException: {FullExceptionMessage} | IP: {IpAddress}",
                message, fullExceptionMessage, ipAddress);

            return Result.Failure(fullExceptionMessage);
        }

        public static void LogSuccess<T>(this ILogger<T> logger, string action, string model, string user)
        {
            string ipAddress = GetIpAddress();

            logger.LogInformation("[SUCCESS] ==> [{Action}] {Model} | User: {User} | IP: {IpAddress}",
                action, model, user, ipAddress);
        }

        public static void LogFail<T>(this ILogger<T> logger, string action, string model, string user, string error)
        {
            string ipAddress = GetIpAddress();

            logger.LogWarning("[FAIL] ==> [{Action}] {Model} | User: {User} | IP: {IpAddress} | Error: {Error}",
                action, model, user, ipAddress, error);
        }

        private static string GetIpAddress()
        {
            var httpContext = HttpContextAccessor?.HttpContext;

            if (httpContext == null)
                return "HttpContext is null";

            // ƯU TIÊN: lấy từ Middleware Capture
            if (httpContext.Items.ContainsKey("RequestIpAddress"))
            {
                return httpContext.Items["RequestIpAddress"]?.ToString() ?? "Unknown IP (from Items)";
            }

            // Fallback: X-Forwarded-For
            var xff = httpContext.Request?.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(xff))
                return xff;

            // Fallback: RemoteIpAddress
            var remoteIp = httpContext.Connection?.RemoteIpAddress?.ToString();
            if (!string.IsNullOrEmpty(remoteIp))
                return remoteIp;

            return "Unknown IP (no Items, no XFF, no RemoteIP)";
        }
    }
}
