using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Hexa.Core.Web.Identity
{
    // SAMPLE: https://github.com/rizamarhaban/IdentityServer4Demo/blob/master/MyApi/Startup.cs
    public static class ApplicationBuilderExtensions
    {
        // Ment to be used by a Web App
        public static IServiceCollection AddCookieAndOpenIdConnectAuthentication(this IServiceCollection services, string authority, string clientId, string clientSecret, bool requireHttpsMetadata)
        {
            services.AddAuthentication(o =>
            {
                o.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                o.DefaultAuthenticateScheme = OpenIdConnectDefaults.AuthenticationScheme;
                o.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            })
            .AddCookie()
            .AddOpenIdConnect(o =>
            {
                o.Authority = authority;
                o.RequireHttpsMetadata = requireHttpsMetadata;
                o.ClientId = clientId;
                o.ClientSecret = clientSecret;

                o.ResponseType = "code id_token";
                o.SaveTokens = true;

                o.Scope.Add("openid");
                o.Scope.Add("profile");

                o.GetClaimsFromUserInfoEndpoint = true;
            });

            return services;
        }

        // Ment to be used by a Web API
        public static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services, string authority, string audience, bool requireHttpsMetadata)
        {
            // services.AddCors(o =>
            // {
            //     o.AddPolicy("default", policy =>
            //     {
            //         policy.AllowAnyOrigin();
            //         policy.AllowAnyHeader();
            //         policy.AllowAnyMethod();
            //         policy.WithExposedHeaders("WWW-Authenticate");
            //     });
            // });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(o =>
                {
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

                })
                .AddJwtBearer(o =>
                    {
                        o.Authority = authority;
                        o.RequireHttpsMetadata = requireHttpsMetadata;
                        o.Audience = audience;
                        o.RequireHttpsMetadata = false;
                    });

            return services;
        }

    }
}