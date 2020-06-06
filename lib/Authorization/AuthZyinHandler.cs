namespace AuthZyin.Authorization
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// AuthZinHandler which handles our requirements.
    /// Must be registered as scoped since we depend on IAuthZyinContext.
    /// </summary>
    public class AuthZyinHandler: IAuthorizationHandler
    {
        /// <summary>
        /// AuthZyin context for authorization
        /// </summary>
        private readonly IAuthZyinContext authZyinContext;

        /// <summary>
        /// logger instance
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new intance of AuthZyinHandler
        /// </summary>
        /// <param name="authZyinContext">AuthZyin context for authorization</param>
        /// <param name="logger">logger instance</param>
        public AuthZyinHandler(IAuthZyinContext authZyinContext, ILogger<AuthZyinHandler> logger)
        {
            this.authZyinContext = authZyinContext ?? throw new ArgumentNullException(nameof(authZyinContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles requirements
        /// </summary>
        /// <param name="context">authorization context</param>
        /// <param name="requirement">IAuthZyinRequirement instance</param>
        /// <returns>task</returns>
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            foreach (var requirement in context.Requirements.OfType<Requirement>())
            {
                if (requirement.Evaluate(authZyinContext, context.Resource))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}