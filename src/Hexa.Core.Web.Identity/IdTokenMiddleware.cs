namespace Hexa.Core.Web.Identity
{
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.IdentityModel.Tokens;

    public class IdTokenMiddleware
    {
        private readonly RequestDelegate next;
        private readonly X509Certificate2 signingCredential;
        private readonly string validAudience;
        private readonly string authority;

        public IdTokenMiddleware(RequestDelegate next, string authority, string validAudience, X509Certificate2 signingCredential)
        {
            this.next = next;
            this.signingCredential = signingCredential;
            this.validAudience = validAudience;
            this.authority = authority;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey(HttpClientFactory.EndUserHeaderName))
            {
                var endUserToken = context.Request.Headers[HttpClientFactory.EndUserHeaderName].First();

                var tokenHandler = new JwtSecurityTokenHandler();

                SecurityToken validtoken;
                var userPrincipal = tokenHandler.ValidateToken(
                    endUserToken,
                    new TokenValidationParameters()
                    {
                        ValidAudience = validAudience,
                        ValidIssuer = authority,
                        IssuerSigningKey = new X509SecurityKey(signingCredential),
                        // ValidateLifetime = false,
                        IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) => new List<X509SecurityKey> { new X509SecurityKey(signingCredential) }
                    },
                    out validtoken);

                var identity = context.User.Identity as ClaimsIdentity;
                identity.AddClaim(new Claim("id_token", endUserToken));

                context.Items["pos-end-user"] = userPrincipal;
            }

            await next(context).ConfigureAwait(false);

            // // public async Task Logout()
            // // {
            // //     await HttpContext.Authentication.SignOutAsync("Cookies");
            // //     await HttpContext.Authentication.SignOutAsync("oidc");
            // // }

        }
    }

    public static class IdTokenMiddlewareExtensions
    {
        public static IApplicationBuilder UseIdToken(this IApplicationBuilder app, X509Certificate2 signingCredential)
        {
            app.UseMiddleware<IdTokenMiddleware>(signingCredential);

            return app;
        }
    }
}
