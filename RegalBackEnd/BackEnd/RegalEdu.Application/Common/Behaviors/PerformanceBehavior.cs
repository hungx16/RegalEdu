using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace RegalEdu.Application.Common.Behaviors
{
    public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly Stopwatch _timer;
        private readonly ILogger<TRequest> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PerformanceBehavior(
            ILogger<TRequest> logger,
            ICurrentUserService currentUserService,
            IHttpContextAccessor httpContextAccessor)
        {
            _timer = new Stopwatch();
            _logger = logger;
            _currentUserService = currentUserService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;
            var requestName = typeof(TRequest).Name;

            var userId = _currentUserService.UserCode ?? "No Id";
            var userName = _currentUserService.UserName ?? "No Name";

            // ƯU TIÊN đọc từ Items để đồng bộ với LoggerExtensions và GlobalExceptionMiddleware
            string ipAddress = _httpContextAccessor.HttpContext?.Items["RequestIpAddress"]?.ToString()
                            ?? _httpContextAccessor.HttpContext?.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                            ?? _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString()
                            ?? "Unknown IP";

            // Lấy Path (rất hữu ích khi đọc log)
            string path = _httpContextAccessor.HttpContext?.Request?.Path.Value ?? "Unknown path";

            try
            {
                if (elapsedMilliseconds > 1000)
                {
                    _logger.LogWarning("Long Running Request: {RequestName} ({ElapsedMilliseconds} ms) {@Request} | User: {UserId}-{UserName} | IP: {IpAddress} | Path: {Path}",
                        requestName, elapsedMilliseconds, request, userId, userName, ipAddress, path);
                }
                else
                {
                    _logger.LogInformation("Request: {RequestName} ({ElapsedMilliseconds} ms) {@Request} | User: {UserId}-{UserName} | IP: {IpAddress} | Path: {Path}",
                        requestName, elapsedMilliseconds, request, userId, userName, ipAddress, path);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging performance for request: {RequestName} | User: {UserId}-{UserName} | IP: {IpAddress} | Path: {Path}",
                    requestName, userId, userName, ipAddress, path);
            }

            return response;
        }
    }
}
