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
        /// Gets client context used for client authorization.
        /// Use object because:
        /// 1. we don't know the exact type of CustomData here
        /// 2. object helps with System.Text.Json serialization to include all needed members.
        /// </summary>
        object ClientContext { get; }
    }

    /// <summary>
    /// AuthZyinContext managing generation of data required during authorization.
    /// This is used by the server. For client, there is a special ClientContext class used to serialize required info.
    /// </summary>
    public class AuthZyinContext<T> : IAuthZyinContext where T : class
    {
        /// <summary>
        /// Authorization user context
        /// </summary>
        public AuthZyinUserContext UserContext { get; }

        /// <summary>
        // Authorization policy list
        /// </summary>
        public IEnumerable<(string name, AuthorizationPolicy policy)> Policies { get; }

        /// <summary>
        /// custom data for authorization purpose
        /// </summary>
        public T CustomData { get; }

        /// <summary>
        /// Custom claim type to process. This will be used to construct JsonData member in the return
        /// </summary>
        protected virtual string ClaimTypeForCustomData => null;

        /// <summary>
        /// Gets an object representing the authorization context to send to client
        /// Implementing IAuthZyinContext.
        /// </summary>
        public object ClientContext => new ClientContext<T>(this);
        /// <summary>
        /// Initializes a new instance of the AuthZyinContext class. This constructor is for DI purpose.
        /// </summary>
        /// <param name="policyList">policy list</param>
        /// <param name="contextAccessor">httpContextAccessor</param>
        public AuthZyinContext(IAuthorizationPolicyList policyList, IHttpContextAccessor contextAccessor)
        {
            this.Policies = policyList?.Policies ?? throw new ArgumentNullException(nameof(policyList));
            if (contextAccessor?.HttpContext?.User == null)
            {
                throw new ArgumentNullException(nameof(contextAccessor));
            }

            var claimsAccessor = new AadClaimsAccessor(contextAccessor?.HttpContext?.User);
            this.UserContext = new AuthZyinUserContext(claimsAccessor);

            // Retrieve the custom data json string from claims as well (if any).
            // It's denoted by a virtual member CustomClaimTypeToProcess.
            // Usually it's not safe to call virtual member in the constructor, but it's safe
            // here since CustomClaimTypeToProcess is just meant to return a string constant.
            var customDataJsonString = this.ClaimTypeForCustomData != null ? claimsAccessor.GetClaimValue(this.ClaimTypeForCustomData) : null;
            this.CustomData = customDataJsonString != null ? JsonSerializer.Deserialize<T>(customDataJsonString) : null;
        }
    }
}