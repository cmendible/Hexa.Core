using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using IdentityServer4;
using Microsoft.AspNetCore.Builder;

namespace Hexa.Core.Web.Identity
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCookieAndOpenIdConnect(this IApplicationBuilder app, string authority, string clientId, bool requireHttpsMetadata)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "Cookies",
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                AuthenticationScheme = "oidc",
                SignInScheme = "Cookies",

                Authority = authority,
                RequireHttpsMetadata = requireHttpsMetadata,

                ClientId = clientId,

                ResponseType = "id_token",

                GetClaimsFromUserInfoEndpoint = true,
                SaveTokens = true,
            });

            return app;
        }

        public static IApplicationBuilder UseIdentityServerAuthentication(this IApplicationBuilder app, string authority, IEnumerable<string> allowedScopes, bool requireHttpsMetadata)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                Authority = authority,
                RequireHttpsMetadata = requireHttpsMetadata,
                AllowedScopes = allowedScopes.Append(IdentityServerConstants.StandardScopes.OpenId).ToList(),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
            });

            return app;
        } 

    }
}