using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations; // Chứa enum PermissionAction
using System.Net;

namespace RegalEdu.BackEnd.Middleware
{
    public class CheckPrivilegeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CheckPrivilegeMiddleware> _logger;

        public CheckPrivilegeMiddleware(RequestDelegate next, ILogger<CheckPrivilegeMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext, IServiceProvider provider)
        {
            // Bypass FormName requirement for SignalR notifications hub
            if (httpContext.Request.Path.StartsWithSegments("/hubs/notifications"))
            {
                await _next(httpContext);
                return;
            }
            // Nếu chưa login thì cho qua (có thể sửa lại thành chặn nếu muốn)
            if (httpContext.User?.Identity?.IsAuthenticated != true)
            {
                await _next(httpContext);
                return;
            }

            // Lấy UserId (hoặc EmployeeATID, tùy hệ thống)
            var userService = provider.GetService(typeof(ICurrentUserService)) as ICurrentUserService;
            string userId = userService?.UserCode ?? "";

            if (string.IsNullOrEmpty(userId))
            {
                await _next(httpContext);
                return;
            }

            // Lấy FormName từ header (có thể mở rộng lấy từ attribute hoặc endpoint metadata)
            string formName = httpContext.Request.Headers["FormName"].FirstOrDefault() ?? "";

            if (string.IsNullOrEmpty(formName))
            {
                _logger.LogWarning("Request {Path} missing FormName header, user {UserId}", httpContext.Request.Path, userId);
                // Nếu muốn chặn khi thiếu FormName thì return 403 ở đây
                // httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                // await httpContext.Response.WriteAsync("Missing FormName header");
                // return;
                // Nếu muốn cho qua thì:
                await _next(httpContext);
                return;
            }

            // Lấy action theo HTTP method
            PermissionAction action = PermissionAction.view;
            switch (httpContext.Request.Method.ToUpper())
            {
                case "GET": action = PermissionAction.view; break;
                case "POST": action = PermissionAction.add; break;
                case "PUT": action = PermissionAction.edit; break;
                case "DELETE": action = PermissionAction.delete; break;
                // case "PATCH": action = PermissionAction.edit; break;
                default: action = PermissionAction.view; break;
            }

            var permissionService = provider.GetService(typeof(IUserPermissionInfoService)) as IUserPermissionInfoService;
            if (permissionService == null)
            {
                _logger.LogError("IUserPermissionInfoService not registered in DI container!");
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await httpContext.Response.WriteAsync("Internal Server Error");
                return;
            }

            bool allowed = false;
            try
            {
                allowed = await permissionService.CheckPermissionByEmpAndFormAndAction(userId, formName, action);
                if (formName == "/hubs/notifications")
                {
                    allowed = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when checking permission for user {UserId} - Form: {FormName} - Action: {Action}", userId, formName, action);
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await httpContext.Response.WriteAsync("Internal Server Error");
                return;
            }

            if (!allowed)
            {
                _logger.LogWarning("User {UserId} is forbidden to access {FormName} with action {Action} (API {Path})", userId, formName, action, httpContext.Request.Path);
                httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsync("NotPrivilege");
                return;
            }

            // Nếu có quyền thì cho qua
            await _next(httpContext);
        }
    }

    public static class MiddlewareConfig
    {
        public static IApplicationBuilder UsePiplineMiddleware(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<CheckPrivilegeMiddleware>();
            return builder;
        }
    }
}
