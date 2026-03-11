using Microsoft.AspNetCore.Http;
using RegalEdu.Application.Common.Interfaces;
using System.Security.Claims;

namespace RegalEdu.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public string EmployeeId => !string.IsNullOrEmpty(_httpContextAccessor.HttpContext?.User?.FindFirstValue("EmployeeId"))
            ? _httpContextAccessor.HttpContext?.User?.FindFirstValue("EmployeeId") ?? string.Empty
            : string.Empty;
        public string TeacherId => !string.IsNullOrEmpty(_httpContextAccessor.HttpContext?.User?.FindFirstValue("TeacherId"))
            ? _httpContextAccessor.HttpContext?.User?.FindFirstValue("TeacherId") ?? string.Empty
            : string.Empty;
        public string UserName => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

        public string UserCode => _httpContextAccessor.HttpContext?.User?.FindFirstValue("UserCode") ?? string.Empty;

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public string? GetCurrentUserId()
        {
            // Có thể trả về UserId, EmployeeId, hoặc UserCode tùy theo logic của bạn
            // Dưới đây là một số lựa chọn:

            // Option 1: Trả về EmployeeId
            return !string.IsNullOrEmpty(EmployeeId) ? EmployeeId : null;

            // Option 2: Trả về UserCode
            // return !string.IsNullOrEmpty(UserCode) ? UserCode : null;

            // Option 3: Trả về claim "sub" hoặc "userId"
            // return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) 
            //     ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub") 
            //     ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("userId");
        }

        public Guid? GetCurrentUserIdAsGuid()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return null;

            if (Guid.TryParse(userId, out Guid result))
                return result;

            return null;
        }
    }
}