namespace AuthZyin.Authorization
{
    using System;
    using System.Text.Json;
    using AuthZyin.Authentication;
    using Microsoft.AspNetCore.Http;
    using AuthZyin.Authorization.Client;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// Interface to construct required authorization data
    /// </summary>
    public interface IAuthZyinContext
    {
        /// <summary>
        /// Gets client context used for client authorization
        /// </summary>
        object ClientContext { get; }
    }

    /// <summary>
    /// AuthZyinContext managing generation of data required during authorization
    /// </summary>
    public class AuthZyinContext<T> : IAuthZyinContext
        where T : class
    {
        /// <summary>
        /// Authorization user context
        /// </summary>
        private readonly AuthZyinUserContext userContext;

        /// <summary>
        // Authorization policy list
        /// </summary>
        private readonly IEnumerable<(string name, AuthorizationPolicy policy)> policies;

        /// <summary>
        /// custom data for authorization purpose
        /// </summary>
        private readonly T CustomData;

        /// <summary>
        /// Custom claim type to process. This will be used to construct JsonData member in the return
        /// </summary>
        protected virtual string CustomClaimTypeToProcess => null;

        /// <summary>
        /// Gets an object representing the authorization context to send to client
        /// </summary>
        /// <returns>client data object</returns>
        public object ClientContext => new ClientContext<T>(this.userContext, this.CustomData, this.policies);

        /// <summary>
        /// Initializes a new instance of the AuthZyinContext class
        /// </summary>
        /// <param name="policyList">policy list</param>
        /// <param name="contextAccessor">httpContextAccessor</param>
        public AuthZyinContext(
            IAuthorizationPolicyList policyList,
            IHttpContextAccessor contextAccessor)
            : this(policyList?.Policies, new AadClaimsAccessor(contextAccessor?.HttpContext?.User))
        {
        }

        /// <summary>
        /// Initializes a new instance of the AuthZyinContext class
        /// </summary>
        /// <param name="policies">list of policies</param>
        /// <param name="contextAccessor">httpContextAccessor</param>
        public AuthZyinContext(
            IEnumerable<(string name, AuthorizationPolicy policy)> policies,
            AadClaimsAccessor claimsAccessor)
        {
            this.policies = policies ?? throw new ArgumentNullException(nameof(policies));
            if (claimsAccessor == null)
            {
                throw new ArgumentNullException(nameof(claimsAccessor));
            }

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