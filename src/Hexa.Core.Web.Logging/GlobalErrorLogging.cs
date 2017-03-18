namespace Hexa.Core.Web.Logging
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Serilog;

    public class GlobalErrorLogging
    {

        private readonly RequestDelegate next;
        private readonly ILogger log;

        public GlobalErrorLogging(RequestDelegate next, ILogger log)
        {
            this.next = next;
            this.log = log;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Error(ex, "Unhandled exception");
            }
        }
    }
}