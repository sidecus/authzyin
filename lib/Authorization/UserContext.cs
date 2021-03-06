namespace AuthZyin.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AuthZyin.Authentication;

    /// <summary>
    /// UserContext class contains core user claim values to use for authorization purpose.
    /// This is used by both server authorization and client authorization
    /// </summary>
    public class UserContext
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
        /// Initializes a new UserContext from an AadClaimsAccessor object
        /// </summary>
        /// <param name="claimsAccessor">claims accessor</param>
        public UserContext(AadClaimsAccessor claimsAccessor)
        {
            if (claimsAccessor == null)
            {
                throw new ArgumentNullException(nameof(claimsAccessor));
            }

            this.UserId = claimsAccessor.UserId;
            this.UserName = claimsAccessor.UserName;
            this.TenantId = claimsAccessor.TenantId;
            this.Roles = claimsAccessor.Roles.ToList();
        }
    }
}