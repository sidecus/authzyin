namespace AuthZyin.Authorization
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// AuthZinHandler which handles our requirements - needs to be registered as scoped
    /// </summary>
    public class AuthZyinHandler: IAuthorizationHandler
    {
        /// <summary>
        /// AuthZyin authorization context
        /// </summary>
        private readonly IAuthZyinContext authZyinContext;

        /// <summary>
        /// logger instance
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new intance of AuthZyinHandler
        /// </summary>
        /// <param name="authZyinContext">context</param>
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
            var requirements = context.Requirements.OfType<AuthZyinRequirement>();

            foreach (var requirement in requirements)
            {
                if (requirement.Evaluate(this.authZyinContext, context.Resource))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}