namespace Hexa.Core.Web.Logging
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Serilog;

    public class RequestLogging
    {
        private readonly RequestDelegate next;
        private readonly ILogger log;

        public RequestLogging(RequestDelegate next, ILogger log)
        {
            this.next = next;
            this.log = log;
        }

        public async Task Invoke(HttpContext context)
        {
            log.Information("Incoming request: {@Method}, {@Path}, {@Headers}",
                          context.Request.Method,
                          context.Request.Path,
                          context.Request.Headers);

            await next(context);

            log.Information("Outgoing response: {@StatusCode}, {@Headers}",
                      context.Response.StatusCode,
                      context.Response.Headers);
        }
    }
}