namespace AuthZyin.Authentication
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Authentication extensions
    /// </summary>
    public static class AuthZyinAuthenticationExtensions
    {
        /// <summary>
        /// Add Aad based Jwt Bearer authentication
        /// </summary>
        /// <param name="services">service collection</param>
        /// <returns>service collection</returns>
        public static IServiceCollection AddAadJwtBearer(
            this IServiceCollection services,
            string authority,
            string aadAppId)
        {
            if (authority == null)
            {
                throw new ArgumentNullException(nameof(authority));
            }

            if (aadAppId == null)
            {
                throw new ArgumentNullException(nameof(aadAppId));
            }

            // Add JWT bearer token authentication
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    // Configure JWT bearer token validation parameters
                    options.Authority = authority;
                    options.SaveToken = false;
                    options.TokenValidationParameters.ValidateIssuer = true;
                    options.TokenValidationParameters.IssuerValidator = AadIssuerValidator.ValidateAadIssuer;
                    options.TokenValidationParameters.ValidateAudience = true;
                    options.TokenValidationParameters.ValidAudiences = new string[] { aadAppId, $"api://{aadAppId}" };

                    // Enable empty event handler for debugging purpose
                    options.Events = new JwtBearerEvents()
                    {
                        OnTokenValidated = (context) =>
                        {
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = (context) =>
                        {
                            return Task.CompletedTask;
                        },
                    };
                });

            return services;
        }
    }
}