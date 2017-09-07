namespace Hexa.Core.Web.Logging
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Serilog;

    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMonitoringAndLogging(
          this IApplicationBuilder app,
          Func<Task<bool>> serviceStatusCheck)
        {
            app.UseMiddleware<GlobalErrorLogging>();
            app.UseMiddleware<CorrelationToken>();
            app.UseMiddleware<RequestLogging>();
            app.UseMiddleware<PerformanceLogging>();
            app.UseMiddleware<ServiceStatusMiddleware>(serviceStatusCheck);

            return app;
        }
    }
}
