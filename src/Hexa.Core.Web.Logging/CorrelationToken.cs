namespace Hexa.Core.Web.Logging
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Serilog.Context;

    public class CorrelationToken
    {

        private const string CorrelationIdHeaderKey = "X-Correlation-ID";

        private readonly RequestDelegate next;

        public CorrelationToken(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            Guid correlationId;
            if (!(context.Request.Headers.ContainsKey(CorrelationIdHeaderKey)
              && Guid.TryParse(context.Request.Headers[CorrelationIdHeaderKey], out correlationId)))
            {
                correlationId = Guid.NewGuid();
            }

            context.Response.Headers.Add(CorrelationIdHeaderKey, correlationId.ToString());

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await next(context).ConfigureAwait(false);
            }
        }
    }
}