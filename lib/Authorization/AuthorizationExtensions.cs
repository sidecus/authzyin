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

            // Configure authorization options. Use our own AuthZyinAuthorizationOptions to:
            // 1. Expose the list of policies
            // 2. Generate a new configue action to use against AuthorizationOptions.
            var authZyinOptions = new AuthZyinAuthorizationOptions();
            configure(authZyinOptions);

            // Register IAuthorizationPolicyList and enable authoriztion
            services.AddSingleton<IAuthorizationPolicyList>(authZyinOptions);
            services.AddAuthorization(authZyinOptions.CapturedConfigureAction);

            // Add authorization handler - must be registered as scoped
            services.AddScoped<IAuthorizationHandler, AuthZyinHandler>();

            return services;
        }
    }
}
