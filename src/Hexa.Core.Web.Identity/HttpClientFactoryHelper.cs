namespace Hexa.Core.Web.Identity
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;

    public static class HttpClientFactoryHelper
    {
        public const string CorrelationIdHeaderKey = "X-Correlation-ID";        
        private static string TokenUrl;
        private static string ClientName;
        private static string ClientSecret;

        private static void Configure(string tokenUrl, string clientName, string clientSecret)
        {
            TokenUrl = tokenUrl;
            ClientName = clientName;
            ClientSecret = clientSecret;
        }

        public static IServiceCollection UseHttpClientFactory(this IServiceCollection services, string tokenUrl, string clientName, string clientSecret)
        {
            Configure(tokenUrl, clientName, clientSecret);

            services.AddScoped(
                      typeof(IHttpClientFactory),
                      (serviceProvider) =>
                          {
                              var httpContextAccessor = serviceProvider.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
                              string correlationToken = httpContextAccessor?.HttpContext?.Response?.Headers[CorrelationIdHeaderKey];

                              var idToken = httpContextAccessor?.HttpContext.GetTokenAsync("id_token").GetAwaiter().GetResult();
                              if (string.IsNullOrEmpty(idToken))
                              {
                                  var principal = httpContextAccessor.HttpContext.User as ClaimsPrincipal;
                                  idToken = principal?.FindFirst("id_token")?.Value;
                              }

                              return new HttpClientFactory(
                                        TokenUrl,
                                        ClientName,
                                        ClientSecret,
                                        correlationToken,
                                        idToken);
                          });
            return services;
        }
    }
}