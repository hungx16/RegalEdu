namespace RegalEdu.Api.Middleware
{
    public class CaptureRequestIpMiddleware
    {
        private readonly RequestDelegate _next;

        public CaptureRequestIpMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ipAddress = context.Request?.Headers["X-Forwarded-For"].FirstOrDefault()
                         ?? context.Connection?.RemoteIpAddress?.ToString()
                         ?? "Unknown IP";

            context.Items["RequestIpAddress"] = ipAddress;

            await _next(context);
        }
    }

}
