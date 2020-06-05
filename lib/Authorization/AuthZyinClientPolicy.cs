namespace AuthZyin.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// Policy object for client to process
    /// </summary>
    public class AuthZyinClientPolicy
    {
        /// <summary>
        /// Gets or sets policy name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets the requirement list
        /// </summary>
        public List<string> Requirements { get; }

        public AuthZyinClientPolicy(string name, AuthorizationPolicy policy)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Requirements = policy.Requirements.Select(x => x.GetType().Name).ToList();
        }
    }
}