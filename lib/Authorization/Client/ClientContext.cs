namespace AuthZyin.Authorization.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Class representing context data to send to the client for client authorization.
    /// </summary>
    /// <typeparam name="T">Custom data type</typeparam>
    public class ClientContext<T> where T : class
    {
        /// <summary>
        /// Gets the user context, which will also be sent to client
        /// </summary>
        /// <value></value>
        public UserContext UserContext { get; }

        /// <summary>
        /// Gets the polices to be used by the client
        /// </summary>
        public List<ClientPolicy> Policies { get; }

        /// <summary>
        /// Gets or sets custom data used to do "resource" based authorization on client. It's of type T.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Initializes a new instance of the ClientContext class to send to client
        /// </summary>
        /// <param name="context">AuthZyin context</param>
        public ClientContext(AuthZyinContext<T> context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            this.UserContext = context.UserContext;
            this.Policies = context.Policies.Select(x => new ClientPolicy(x.name, x.policy)).ToList();
            this.Data = context.Data;
        }
    }
}