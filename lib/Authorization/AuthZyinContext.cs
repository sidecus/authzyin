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
    /// <typeparam name="T">CustomData type which can be used during authorization. Loaded from claims using ClaimTypeForCustomData</typeparam>
    /// </summary>
    public class AuthZyinContext<T> : IAuthZyinContext where T : class
    {
        /// <summary>
        /// Claims accessor
        /// </summary>
        protected AadClaimsAccessor claimsAccessor { get ;}

        /// <summary>
        /// Authorization user context
        /// </summary>
        public UserContext UserContext { get; }

        /// <summary>
        // Authorization policy list
        /// </summary>
        public IEnumerable<(string name, AuthorizationPolicy policy)> Policies { get; }

        /// <summary>
        /// custom data for authorization purpose
        /// </summary>
        public T CustomData { get; }

        /// <summary>
        /// Gets an object representing the authorization context to send to client
        /// Implementing IAuthZyinContext.
        /// </summary>
        public object ClientContext => new ClientContext<T>(this);

        /// <summary>
        /// Initializes a new instance of the AuthZyinContext class. This constructor is for DI purpose.
        /// </summary>
        /// <param name="policyList">policy list</param>
        /// <param name="httpContextAccessor">httpContextAccessor</param>
        public AuthZyinContext(IAuthorizationPolicyList policyList, IHttpContextAccessor httpContextAccessor)
        {
            this.Policies = policyList?.Policies ?? throw new ArgumentNullException(nameof(policyList));
            if (httpContextAccessor?.HttpContext?.User == null)
            {
                throw new ArgumentNullException(nameof(httpContextAccessor));
            }

            this.claimsAccessor = new AadClaimsAccessor(httpContextAccessor?.HttpContext?.User);
            this.UserContext = new UserContext(claimsAccessor);

            // Usually it's not safe to call virtual member in the constructor, but it's relatively safe
            // here since GetCustomData is not supposed to reference anything from the current object being constructed.
            this.CustomData = this.CreateCustomData();
        }

        /// <summary>
        /// Get custom data to be associated with the context object.
        /// Can be overriden to add additional data for authorization purpose.
        /// </summary>
        /// <returns>CustomData of type T</returns>
        protected virtual T CreateCustomData()
        {
            return null;
        }
    }
}