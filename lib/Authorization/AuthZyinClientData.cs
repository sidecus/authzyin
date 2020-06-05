namespace AuthZyin.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AuthZyin.Authentication;
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// Interface to construct required authorization data for client
    /// </summary>
    /// <typeparam name="T">Custom data type</typeparam>
    public class AuthZyinClientData<T>
    {
        /// <summary>
        /// Gets user id
        /// </summary>
        public string UserId { get; }

        /// <summary>
        /// Gets user name
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// Gets tenant id
        /// </summary>
        public string TenantId { get; }

        /// <summary>
        /// Gets user roles (string list)
        /// </summary>
        public List<string> Roles { get; }

        /// <summary>
        /// Gets the polices to be used by the client
        /// </summary>
        /// <value></value>
        public List<AuthZyinClientPolicy> Policies { get; }

        /// <summary>
        /// Gets or sets custom data used to do "resource" based authorization on client. It's of type T.
        /// </summary>
        public T CustomData { get; set; }

        /// <summary>
        /// Initializes a new instance of the AuthZyinClientData class (initialized without CustomData)
        /// </summary>
        /// <param name="claimsAccessor">claims accessor</param>
        /// <param name="policies">policy list</param>
        public AuthZyinClientData(
            AadClaimsAccessor claimsAccessor,
            IEnumerable<(string name, AuthorizationPolicy policy)> policies,
            Func<T> customDataFactory)
        {
            if (claimsAccessor == null)
            {
                throw new ArgumentNullException(nameof(claimsAccessor));
            }

            if (policies == null)
            {
                throw new ArgumentNullException(nameof(policies));
            }

            this.UserId = claimsAccessor.UserId;
            this.UserName = claimsAccessor.UserName;
            this.TenantId = claimsAccessor.TenantId;
            this.Roles = claimsAccessor.Roles.ToList();
            this.Policies = policies.Select(x => new AuthZyinClientPolicy(x.name, x.policy)).ToList();

            if (customDataFactory != null)
            {
                this.CustomData = customDataFactory();
            }
        }
    }
}