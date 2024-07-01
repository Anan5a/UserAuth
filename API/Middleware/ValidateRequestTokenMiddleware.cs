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
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

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
                Logger.Info("No access token used for access");
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                httpContext.Response.WriteAsync("Unauthorized request. Invalid token. Access to this resource is forbidden.");
                return Task.CompletedTask;
            }

            if (!string.IsNullOrEmpty(ConfigHelper.GetAPIKey(_config)) && !ConfigHelper.GetAPIKey(_config).Equals(token))
            {
                Logger.Info("Invalid access token used for access: {0}", token);

                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                httpContext.Response.WriteAsync("Unauthorized request. Invalid token. Access to this resource is forbidden.");
                return Task.CompletedTask;
            }
            Logger.Info("API access granted");

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
