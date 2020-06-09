namespace AuthZyin.Authorization.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Authorization.Infrastructure;
    using AuthZyin.Authorization.Requirements;

    /// <summary>
    /// Policy object for client to process
    /// </summary>
    public class ClientPolicy
    {
        /// <summary>
        /// List of requirements
        /// </summary>
        private readonly List<Requirement> requirements;

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
        public IEnumerable<object> Requirements => this.requirements;

        /// <summary>
        /// Initializes a new instance of AuthZyinClientPolicy class
        /// </summary>
        /// <param name="name">policy name</param>
        /// <param name="policy">policy object</param>
        public ClientPolicy(string name, AuthorizationPolicy policy)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            
            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            this.requirements = policy.Requirements
                .Select(r => this.GetClientRequirement(r))
                .Where(r => r != null)
                .ToList();
        }

        /// <summary>
        /// Get a requirement to be serialized to client
        /// </summary>
        /// <param name="requirement">authorization requirement</param>
        /// <returns>AbstractRequirement to use, or null which means current requirement should be ignored</returns>
        public Requirement GetClientRequirement(IAuthorizationRequirement requirement)
        {
            if (requirement is RolesAuthorizationRequirement roleRequirement)
            {
                // special processing for RolesAuthorizationRequirement since it's one of the built in requirements
                return new ClientRoleRequirement(roleRequirement.AllowedRoles);
            }
            else if (requirement is Requirement authZyinRequirement)
            {
                // for other AuthZyin requirements, pass through
                return authZyinRequirement;
            }

            // ignore all other requirements for now
            return null;
        }
    }
}