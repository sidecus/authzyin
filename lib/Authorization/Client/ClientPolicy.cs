namespace AuthZyin.Authorization.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Authorization.Infrastructure;

    /// <summary>
    /// Policy object for client to process
    /// </summary>
    public class ClientPolicy
    {
        /// <summary>
        /// List of requirements
        /// </summary>
        private readonly List<AbstractRequirement> requirements;

        /// <summary>
        /// Gets or sets policy name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets the requirement list. In fact all requirements to send to client should be of type AbstractRequirement.
        /// TODO[sidecus]: Workaround for new System.Text.Json serialization
        /// The reason to use List<object> instead of List<AbstractRequirement> as the item type is to force System.Text.Json
        /// to serialize all members in web api response.
        /// https://github.com/dotnet/runtime/issues/31742 & https://github.com/dotnet/runtime/issues/29937
        /// </summary>
        public List<object> Requirements { get; }

        /// <summary>
        /// Initializes a new instance of AuthZyinClientPolicy class
        /// </summary>
        /// <param name="name">policy name</param>
        /// <param name="policy">policy object</param>
        public ClientPolicy(string name, AuthorizationPolicy policy)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Requirements = policy.Requirements
                .Select(r => this.GetClientRequirement(r) as object)
                .Where(r => r != null)
                .ToList();
        }

        /// <summary>
        /// Get a requirement to be serialized to client
        /// </summary>
        /// <param name="requirement">authorization requirement</param>
        /// <returns>AbstractRequirement to use, or null which means current requirement should be ignored</returns>
        public AbstractRequirement GetClientRequirement(IAuthorizationRequirement requirement)
        {
            if (requirement is RolesAuthorizationRequirement roleRequirement)
            {
                // special processing for RolesAuthorizationRequirement since it's one of the built in requirements
                return new RequiresRoleRequirement(roleRequirement.AllowedRoles);
            }
            else if (requirement is AbstractRequirement abstractRequirement)
            {
                // for other AuthZyin requirements, pass through
                return abstractRequirement;
            }

            // ignore all other requirements for now
            return null;
        }
    }
}