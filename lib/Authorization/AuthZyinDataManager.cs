namespace AuthZyin.Authorization
{
    using System;
    using System.Text.Json;
    using AuthZyin.Authentication;
    using Microsoft.AspNetCore.Http;
    using AuthZyin.Authorization.Client;

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
        /// Authorization user context
        /// </summary>
        private readonly AuthZyinUserContext userContext;

        /// <summary>
        // Authorization policy list
        /// </summary>
        private readonly IAuthorizationPolicyList policyList;

        /// <summary>
        /// custom data for authorization purpose
        /// </summary>
        private readonly T CustomData;

        /// <summary>
        /// Custom claim type to process. This will be used to construct JsonData member in the return
        /// </summary>
        protected virtual string CustomClaimTypeToProcess => null;

        /// <summary>
        /// Interface implementation, get an object representing the authorization data to send to client
        /// </summary>
        /// <returns>client data object</returns>
        public object ClientData => new AuthZyinClientData<T>(this.userContext, this.CustomData, this.policyList.Policies);

        /// <summary>
        /// Initializes a new instance of the AuthZyinClientDataManager class
        /// </summary>
        /// <param name="policyList">policy list</param>
        /// <param name="contextAccessor">httpContextAccessor</param>
        public AuthZyinDataManager(IAuthorizationPolicyList policyList, IHttpContextAccessor contextAccessor)
        {
            this.policyList = policyList ?? throw new ArgumentNullException(nameof(policyList));
            
            var principal = contextAccessor?.HttpContext?.User ?? throw new ArgumentNullException(nameof(contextAccessor));
            var claimsAccessor = new AadClaimsAccessor(principal);
            this.userContext = new AuthZyinUserContext(claimsAccessor);

            // Retrieve the custom data json string from claims as well (if any).
            // It's denoted by a virtual member CustomClaimTypeToProcess.
            // Usually it's not safe to call virtual member in the constructor, but it's safe
            // here since CustomClaimTypeToProcess is just meant to return a string constant.
            var customDataJsonString = this.CustomClaimTypeToProcess != null ? claimsAccessor.GetClaimValue(this.CustomClaimTypeToProcess) : null;
            this.CustomData = customDataJsonString != null ? JsonSerializer.Deserialize<T>(customDataJsonString) : null;
        }
    }
}