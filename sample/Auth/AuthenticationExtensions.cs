namespace sample.Auth
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.DependencyInjection;
    using AuthZyin.Authentication;

    /// <summary>
    /// Authentication extensions
    /// </summary>
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// Add Aad based Jwt Bearer authentication
        /// </summary>
        /// <param name="services">service collection</param>
        /// <returns>service collection</returns>
        public static IServiceCollection AddAadJwtBearerAuthentication(this IServiceCollection services, AuthConfig authConfig)
        {
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
                    options.Authority = authConfig.Authority;
                    options.SaveToken = false;
                    options.TokenValidationParameters.ValidateIssuer = true;
                    options.TokenValidationParameters.IssuerValidator = AadIssuerValidator.ValidateAadIssuer;
                    options.TokenValidationParameters.ValidateAudience = true;
                    options.TokenValidationParameters.ValidAudiences = new string[] { authConfig.AadAppId, $"api://{authConfig.AadAppId}" };

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