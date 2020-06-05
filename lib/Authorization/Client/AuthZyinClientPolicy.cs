namespace AuthZyin.Authorization.Client
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

        /// <summary>
        /// Initializes a new instance of AuthZyinClientPolicy class
        /// </summary>
        /// <param name="name">policy name</param>
        /// <param name="policy">policy object</param>
        public AuthZyinClientPolicy(string name, AuthorizationPolicy policy)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Requirements = policy.Requirements.Select(x => x.GetType().Name).ToList();
        }
    }
}