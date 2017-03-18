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
          ILogger log,
          Func<Task<bool>> serviceStatusCheck)
        {
            app.UseMiddleware<GlobalErrorLogging>(log);
            app.UseMiddleware<CorrelationToken>();
            app.UseMiddleware<RequestLogging>(log);
            app.UseMiddleware<PerformanceLogging>(log);
            app.UseMiddleware<ServiceStatusMiddleware>(serviceStatusCheck);

            return app;
        }
    }
}
