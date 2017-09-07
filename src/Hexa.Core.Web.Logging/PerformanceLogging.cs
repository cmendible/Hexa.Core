namespace Hexa.Core.Web.Logging
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Serilog;
    
    public class PerformanceLogging
    {
        private readonly RequestDelegate next;
        private readonly ILogger log = Serilog.Log.ForContext<PerformanceLogging>();

        public PerformanceLogging(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            await next(context).ConfigureAwait(false);
            stopWatch.Stop();
            log.Information(
              "Request: {@Method} {@Path} executed in {RequestTime:000} ms",
              context.Request.Method,
              context.Request.Path,
              stopWatch.ElapsedMilliseconds);
        }
    }
}