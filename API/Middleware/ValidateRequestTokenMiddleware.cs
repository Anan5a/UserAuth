using API.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace API.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ValidateRequestTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        public ValidateRequestTokenMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _config = configuration;
        }

        public Task Invoke(HttpContext httpContext)
        {

            string? token = httpContext.Request.Headers["Request-Token"];

            if (string.IsNullOrEmpty(token))
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                httpContext.Response.WriteAsync("Unauthorized request. Invalid token. Access to this resource is forbidden.");
                return Task.CompletedTask;
            }

            if (!string.IsNullOrEmpty(ConfigHelper.GetAPIKey(_config)) && !ConfigHelper.GetAPIKey(_config).Equals(token))
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                httpContext.Response.WriteAsync("Unauthorized request. Invalid token. Access to this resource is forbidden.");
                return Task.CompletedTask;
            }

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ValidateRequestTokenMiddlewareExtensions
    {
        public static IApplicationBuilder UseValidateRequestTokenMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ValidateRequestTokenMiddleware>();
        }
    }
}
