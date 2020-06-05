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
        /// Gets or sets the requirement list - note we are using object as the type here.
        /// In fact all requirements to send to lient should be of type AuthZyinRequirement.
        /// The reason to use object as the item type is to force System.Text.Json to serialize all members.
        /// Otherwise it only serialize the members from AuthZyinRequirement.
        /// We need the deeply polymorphic behavior here.
        /// </summary>
        public List<object> Requirements { get; }

        /// <summary>
        /// Initializes a new instance of AuthZyinClientPolicy class
        /// </summary>
        /// <param name="name">policy name</param>
        /// <param name="policy">policy object</param>
        public AuthZyinClientPolicy(string name, AuthorizationPolicy policy)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Requirements = policy.Requirements.Where(r => r is AuthZyinRequirement).Select(r => r as object).ToList();
        }
    }
}