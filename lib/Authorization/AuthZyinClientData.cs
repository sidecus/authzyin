namespace AuthZyin.Authorization
{
    using System.Collections.Generic;

    /// <summary>
    /// Policy object for client to process
    /// </summary>
    public class AuthZyinClientPolicy
    {
        /// <summary>
        /// Gets or sets policy name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the requirement list
        /// </summary>
        public List<string> Requirements { get; set; }
    }

    /// <summary>
    /// Interface to construct required authorization data for client
    /// </summary>
    /// <typeparam name="T">Custom data type</typeparam>
    public class AuthZyinClientData<T>
    {
        /// <summary>
        /// Gets or sets user id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets user name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets tenant id
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets user roles (string list)
        /// </summary>
        public List<string> Roles { get; set; }

        public List<AuthZyinClientPolicy> Policies { get; set; }

        /// <summary>
        /// Gets or sets custom data used to do "resource" based authorization on client.
        /// It's of type T
        /// </summary>
        public T CustomData { get; set; }
    }
}