namespace AuthZyin.Authorization
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AuthZyin.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// AuthZinHandler which handles our requirements
    /// </summary>
    public class AuthZyinHandler: IAuthorizationHandler
    {
        /// <summary>
        /// logger instance
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new intance of AuthZyinHandler
        /// </summary>
        /// <param name="logger">logger instance</param>
        public AuthZyinHandler(ILogger<AuthZyinHandler> logger)
        {
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
            var claimsAccessor = new AadClaimsAccessor(context.User);
            var requirements = context.Requirements.OfType<AuthZyinRequirement>();

            foreach (var requirement in requirements)
            {
                if (requirement.Evaluate(claimsAccessor, context.Resource))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}