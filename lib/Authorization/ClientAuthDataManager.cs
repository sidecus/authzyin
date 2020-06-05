namespace AuthZyin.Authorization
{
    using System;
    using System.Linq;
    using System.Text.Json;
    using AuthZyin.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Interface to construct required authorization data for client
    /// </summary>
    public interface IAuthZyinClientDataManager
    {
        /// <summary>
        /// Generate client data used for client authorization
        /// </summary>
        /// <returns>data for client authorization</returns>
        object GetClientData();
    }

    /// <summary>
    /// DefaultClientAuthDataManager managing generation of client data used for client authorization
    /// </summary>
    public class DefaultClientAuthDataManager<T> : IAuthZyinClientDataManager
        where T : class
    {
        /// <summary>
        // Authorization policy list
        /// </summary>
        private readonly IAuthorizationPolicyList policyList;

        /// <summary>
        /// Cliams accessor
        /// </summary>
        private readonly AadClaimsAccessor claimsAccessor;

        /// <summary>
        /// Custom claim type to process. This will be used to construct JsonData member in the return
        /// </summary>
        protected virtual string CustomClaimTypeToProcess => null;

        /// <summary>
        /// Initializes a new instance of the AuthZyinClientDataManager class
        /// </summary>
        /// <param name="policyList">policy list</param>
        /// <param name="contextAccessor">httpContextAccessor</param>
        public DefaultClientAuthDataManager(IAuthorizationPolicyList policyList, IHttpContextAccessor contextAccessor)
        {
            this.policyList = policyList ?? throw new ArgumentNullException(nameof(policyList));
            var principal = contextAccessor?.HttpContext?.User ?? throw new ArgumentNullException(nameof(contextAccessor));
            this.claimsAccessor = new AadClaimsAccessor(principal);
        }

        /// <summary>
        /// Interface implementation, get an object representing the authorization data to send to client
        /// </summary>
        /// <returns>client data object</returns>
        public object GetClientData()
        {
            return this.GetTypedClientData();
        }

        /// <summary>
        /// Generate client data
        /// </summary>
        /// <returns></returns>
        protected AuthZyinClientData<T> GetTypedClientData()
        {
            return new AuthZyinClientData<T>()
            {
                UserId = this.claimsAccessor.UserId,
                UserName = this.claimsAccessor.UserName,
                TenantId = this.claimsAccessor.TenantId,
                Roles = this.claimsAccessor.Roles.ToList(),
                CustomData = this.GetCustomData(),
                Policies = this.policyList.Policies.Select(x => this.CreateClientPolicyObject(x.name, x.policy)).ToList(),
            };
        }

        /// <summary>
        /// Get custom json data to send to client
        /// </summary>
        /// <returns>custom data object</returns>
        protected T GetCustomData()
        {
            if (this.CustomClaimTypeToProcess == null)
            {
                return null;
            }

            var customClaimValue = this.claimsAccessor.GetClaimValue(this.CustomClaimTypeToProcess);
            return customClaimValue != null ? JsonSerializer.Deserialize<T>(customClaimValue) : null;
        }

        // Create a client policy object from name and policy
        private AuthZyinClientPolicy CreateClientPolicyObject(string name, AuthorizationPolicy policy)
        {
            var policyItem = new AuthZyinClientPolicy() { Name = name, };

            policyItem.Requirements = policy.Requirements.Select(x => x.GetType().Name).ToList();

            return policyItem;
        }
    }
}