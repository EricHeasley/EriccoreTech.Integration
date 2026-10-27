using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace EriccoreTech.Integration.Api {
    public static class AuthenticationExtensions {

        // Fail closed: the host cannot start without an HTTPS authority and an audience.
        // Every endpoint requires an authenticated user unless it opts out with AllowAnonymous.
        public static IServiceCollection AddIntegrationAuthentication(this IServiceCollection services, IConfiguration configuration) {
            var authority = configuration["Authentication:Authority"];
            var audience = configuration["Authentication:Audience"];
            if (string.IsNullOrWhiteSpace(authority) || string.IsNullOrWhiteSpace(audience)
                || !Uri.TryCreate(authority, UriKind.Absolute, out var authorityUri)
                || authorityUri.Scheme != Uri.UriSchemeHttps) {
                throw new InvalidOperationException("Configure HTTPS Authentication:Authority and Authentication:Audience.");
            }

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.Authority = authority;
                    options.Audience = audience;
                    options.RequireHttpsMetadata = true;
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                });
            services.AddAuthorization(options => {
                options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                options.AddPolicy("IntegrationOperator", policy => policy.RequireRole("integration-operator"));
            });
            return services;
        }

    }
}
