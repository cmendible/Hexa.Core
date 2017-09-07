namespace Hexa.Core.Web.Identity
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.IdentityModel.Protocols;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;
    using Microsoft.IdentityModel.Tokens;

    public class IdTokenMiddleware
    {
        private readonly RequestDelegate next;
        private readonly string stsDiscoveryEndpoint;
        private readonly string validAudience;
        private readonly string authority;

        public IdTokenMiddleware(RequestDelegate next, string authority, string validAudience, string stsDiscoveryEndpoint)
        {
            this.next = next;
            this.stsDiscoveryEndpoint = stsDiscoveryEndpoint;
            this.validAudience = validAudience;
            this.authority = authority;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Headers.ContainsKey(HttpClientFactory.EndUserHeaderName))
            {
                context.Response.StatusCode = 400; // Bad Request                
                await context.Response.WriteAsync("pos-end-user is missing");
                return;
            }

            try 
            {
                var endUserToken = context.Request.Headers[HttpClientFactory.EndUserHeaderName].First();

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenValidationParameters = await GetTokenValidationParameters(stsDiscoveryEndpoint, validAudience, authority);

                SecurityToken validtoken;
                ClaimsPrincipal userPrincipal = tokenHandler.ValidateToken(
                    endUserToken,
                    tokenValidationParameters,
                    out validtoken);

                var identity = context.User.Identity as ClaimsIdentity;
                identity.AddClaim(new Claim("id_token", endUserToken));

                context.Items["pos-end-user"] = userPrincipal;
            }
            catch
            {
                context.Response.StatusCode = 401; // UnAuthorized
                await context.Response.WriteAsync("Invalid pos-end-user");
                return;
            }

            await next(context).ConfigureAwait(false);
        }

        private static async Task<TokenValidationParameters> GetTokenValidationParameters(string stsDiscoveryEndpoint, string audience, string authority)
        {
            ConfigurationManager<OpenIdConnectConfiguration> configManager =
                new ConfigurationManager<OpenIdConnectConfiguration>(stsDiscoveryEndpoint, new OpenIdConnectConfigurationRetriever());

            OpenIdConnectConfiguration config = await configManager.GetConfigurationAsync().ConfigureAwait(false);

            return new TokenValidationParameters()
            {
                ValidateIssuer = false,
                IssuerSigningKeys = config.SigningKeys,
                ValidAudience = audience,
                ValidIssuer = authority
            };
        }
    }

    public static class IdTokenMiddlewareExtensions
    {
        // string stsDiscoveryEndpoint = "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration"
        public static IApplicationBuilder UseIdToken(this IApplicationBuilder app, string stsDiscoveryEndpoint)
        {
            app.UseMiddleware<IdTokenMiddleware>(stsDiscoveryEndpoint);

            return app;
        }
    }
}
