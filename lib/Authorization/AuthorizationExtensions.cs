namespace AuthZyin.Authorization
{
    using System;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Auth extensions
    /// </summary>
    public static class AuthorizationExtensions
    {
        /// <summary>
        /// Add authorization
        /// </summary>
        /// <param name="services">service collection</param>
        /// <param name="cofigure">authorization options configuration action</param>
        /// <returns>service collection</returns>
        public static IServiceCollection AddAuthZyinAuthorization(this IServiceCollection services, Action<AuthZyinAuthorizationOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            // Add current assembly as an ApplicationPart for the controller to work
            services.AddControllers().AddApplicationPart(typeof(AuthorizationExtensions).Assembly);

            // Add http context accessor - needed by AuthZyinContext registration
            services.AddHttpContextAccessor();

            // Add authorization and register the policy list instance
            var authZyinOptions = new AuthZyinAuthorizationOptions();
            configure(authZyinOptions);
            services.AddAuthorization(authZyinOptions.ConfigureAuthorizationOptions);
            services.AddSingleton<IAuthorizationPolicyList>(authZyinOptions);

            // Add scoped authorization handler
            services.AddScoped<IAuthorizationHandler, AuthZyinHandler>();

            return services;
        }
    }
}
