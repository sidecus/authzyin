namespace AuthZyin.Authorization
{
    using System;
    using System.Linq;
    using System.Text.Json;
    using AuthZyin.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Interface to construct required authorization data
    /// </summary>
    public interface IAuthZyinDataManager
    {
        /// <summary>
        /// Generate client data used for client authorization
        /// </summary>
        /// <returns>data can be used for client authorization</returns>
        object ClientData { get; }
    }

    /// <summary>
    /// AuthZyinDataManager managing generation of data required during authorization
    /// </summary>
    public class AuthZyinDataManager<T> : IAuthZyinDataManager
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
        /// Interface implementation, get an object representing the authorization data to send to client
        /// </summary>
        /// <returns>client data object</returns>
        public object ClientData => this.GetClientData();

        /// <summary>
        /// Initializes a new instance of the AuthZyinClientDataManager class
        /// </summary>
        /// <param name="policyList">policy list</param>
        /// <param name="contextAccessor">httpContextAccessor</param>
        public AuthZyinDataManager(IAuthorizationPolicyList policyList, IHttpContextAccessor contextAccessor)
        {
            this.policyList = policyList ?? throw new ArgumentNullException(nameof(policyList));
            var principal = contextAccessor?.HttpContext?.User ?? throw new ArgumentNullException(nameof(contextAccessor));
            this.claimsAccessor = new AadClaimsAccessor(principal);
        }

        /// <summary>
        /// Generate client data
        /// </summary>
        /// <returns>AuthZyinClientData</returns>
        protected AuthZyinClientData<T> GetClientData()
        {
            return new AuthZyinClientData<T>(this.claimsAccessor, this.policyList.Policies, this.GetCustomData);
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
    }
}