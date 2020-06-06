namespace AuthZyin.Authorization.Client
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A special RequiresRole requirement - only used to deserialize to client. Don't use on the server.
    /// </summary>
    public class RequiresRoleRequirement : Requirement
    {
        /// <summary>
        /// The required role
        /// </summary>
        private readonly IEnumerable<string> allowedRoles;

        /// <summary>
        /// Requirement type override
        /// </summary>
        public sealed override string RequirementType => "RequiresRole";

        /// <summary>
        /// Get allowed roles.
        /// TODO[sidecus]: Workaround for new System.Text.Json serialization
        /// https://github.com/dotnet/runtime/issues/31742
        /// https://github.com/dotnet/runtime/issues/29937
        /// </summary>
        public IEnumerable<object> AllowedRoles => this.allowedRoles;

        /// <summary>
        /// Initializes a new ClientReuqiresRoleRequirement from RolesAuthorizationRequirement
        /// </summary>
        /// <param name="allowedRoles">allowed roles - user must have at least one role from this list</param>
        public RequiresRoleRequirement(IEnumerable<string> allowedRoles)
        {
            this.allowedRoles = allowedRoles ?? throw new ArgumentNullException(nameof(allowedRoles));
        }

        /// <summary>
        /// Evaluate method implementation.
        /// </summary>
        /// <param name="context">authorization data context</param>
        /// <param name="resource">resource object</param>
        /// <remarks> Not implemented on the server since we don't use this. This is just used to seriaze the RequiresRoleRequirement to the client.</remarks>
        /// <returns>true if allowed</returns>
        public override bool Evaluate(IAuthZyinContext context, object resource)
        {
            throw new NotImplementedException();
        }
    }
}