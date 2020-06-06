namespace AuthZyin.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;

    /// <summary>
    /// Aad claims accessor to simplify certain operations
    /// </summary>
    public class AadClaimsAccessor
    {
        /// <summary>
        /// Role claim type
        /// </summary>
        public static readonly string RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

        /// <summary>
        /// Tenant id claim type
        /// </summary>
        public static readonly string TenantIdClaimType = "http://schemas.microsoft.com/identity/claims/tenantid";

        /// <summary>
        /// User id claim type
        /// </summary>
        public static readonly string UserIdClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        /// <summary>
        /// User name claim type
        /// </summary>
        public static readonly string NameClaimType = "name";

        /// <summary>
        /// User email claim type
        /// </summary>
        public static readonly string EmailClaimType = "preferred_username";
        
        /// <summary>
        /// Gets the tenant id claim value
        /// </summary>
        public string TenantId => this.claimsIdentity.FindFirst(TenantIdClaimType)?.Value;

        /// <summary>
        /// Gets the user id claim value
        /// </summary>
        public string UserId => this.claimsIdentity.FindFirst(UserIdClaimType)?.Value;

        /// <summary>
        /// Gets the user name claim value
        /// </summary>
        public string UserName => this.claimsIdentity.FindFirst(NameClaimType)?.Value;

        /// <summary>
        /// Gets the user email claim value
        /// </summary>
        public string UserEmail => this.claimsIdentity.FindFirst(EmailClaimType)?.Value;

        /// <summary>
        /// Gets the list of roles available to current user
        /// </summary>
        public IEnumerable<string> Roles => this.claimsIdentity.FindAll(RoleClaimType).Select(x => x.Value);

        /// <summary>
        /// Claims identity
        /// </summary>
        private ClaimsIdentity claimsIdentity { get; }

        /// <summary>
        /// Initializes a new instance of AadClaimsAccessor
        /// </summary>
        /// <param name="ci">claims identity</param>
        public AadClaimsAccessor(ClaimsIdentity ci)
        {
            this.claimsIdentity = ci ?? throw new ArgumentNullException(nameof(ci));
        }

        /// <summary>
        /// Initializes a new instance of AadClaimsAccessor
        /// </summary>
        /// <param name="principal">claims principal</param>
        public AadClaimsAccessor(ClaimsPrincipal principal) : this(principal?.Identity as ClaimsIdentity) {}

        /// <summary>
        /// Does cliams contains the specified role claim
        /// </summary>
        /// <param name="role">role</param>
        /// <returns>true if there is a role claim with the specified value</returns>
        public bool HasRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                throw new ArgumentNullException(nameof(role));
            }

            return this.claimsIdentity.HasClaim(RoleClaimType, role);
        }

        /// <summary>
        /// Get a claim value given the claim type
        /// </summary>
        /// <param name="type">claim type</param>
        /// <returns>claim value or null</returns>
        public string GetClaimValue(string type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            
            return this.claimsIdentity.FindFirst(type)?.Value;
        }
    }
}