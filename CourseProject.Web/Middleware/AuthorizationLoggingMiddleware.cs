// CourseProject.Web/Middleware/AuthorizationLoggingMiddleware.cs
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CourseProject.Web.Middleware
{
    public class AuthorizationLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthorizationLoggingMiddleware> _logger;

        public AuthorizationLoggingMiddleware(RequestDelegate next, ILogger<AuthorizationLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == 403)
            {
                var user = context.User.Identity.IsAuthenticated ? context.User.Identity.Name : "Unauthenticated user";
                var path = context.Request.Path;
                _logger.LogWarning("Unauthorized access attempt by {User} to {Path}", user, path);
            }
        }
    }
}
