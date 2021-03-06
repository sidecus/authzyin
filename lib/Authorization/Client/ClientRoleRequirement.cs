namespace AuthZyin.Authorization.Client
{
    using System;
    using System.Collections.Generic;
    using AuthZyin.Authorization.Requirements;

    /// <summary>
    /// A special RequiresRole requirement - only used to deserialize to client.
    /// !!!Don't use this to build policies!!!
    /// </summary>
    public sealed class ClientRoleRequirement : Requirement
    {
        /// <summary>
        /// Get allowed roles.
        /// </summary>
        public IEnumerable<string> AllowedRoles { get; }

        /// <summary>
        /// Initializes a new ClientReuqiresRoleRequirement from RolesAuthorizationRequirement
        /// </summary>
        /// <param name="allowedRoles">allowed roles - user must have at least one role from this list</param>
        public ClientRoleRequirement(IEnumerable<string> allowedRoles)
        {
            this.AllowedRoles = allowedRoles ?? throw new ArgumentNullException(nameof(allowedRoles));
            this.Operator = OperatorType.RequiresRole;
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