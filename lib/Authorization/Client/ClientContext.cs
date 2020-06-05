namespace AuthZyin.Authorization.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// Interface to construct required authorization context data for client
    /// </summary>
    /// <typeparam name="T">Custom data type</typeparam>
    public class ClientContext<T>
    {
        /// <summary>
        /// Gets the user context, which will also be sent to client
        /// </summary>
        /// <value></value>
        public AuthZyinUserContext UserContext { get; }

        /// <summary>
        /// Gets the polices to be used by the client
        /// </summary>
        public List<ClientPolicy> Policies { get; }

        /// <summary>
        /// Gets or sets custom data used to do "resource" based authorization on client. It's of type T.
        /// </summary>
        public T CustomData { get; set; }

        /// <summary>
        /// Initializes a new instance of the AuthZyinClientData class (initialized without CustomData)
        /// </summary>
        /// <param name="userContext">AuthZyin user context</param>
        /// <param name="customData">custom data</param>
        /// <param name="policies">policy list</param>
        public ClientContext(
            AuthZyinUserContext userContext,
            T customData,
            IEnumerable<(string name, AuthorizationPolicy policy)> policies)
        {
            if (userContext == null)
            {
                throw new ArgumentNullException(nameof(userContext));
            }

            if (policies == null)
            {
                throw new ArgumentNullException(nameof(policies));
            }

            this.UserContext = userContext;
            this.Policies = policies.Select(x => new ClientPolicy(x.name, x.policy)).ToList();
            this.CustomData = customData;
        }
    }
}