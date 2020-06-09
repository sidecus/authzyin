namespace AuthZyin.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using AuthZyin.Authentication;
    using AuthZyin.Authorization.Client;

    /// <summary>
    /// Interface to construct required authorization data.
    /// !!!Must be registered as scoped!!!
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
    /// AuthZyinContext managing data required during authorization.
    /// !!!Must be registered as scoped!!!
    /// <typeparam name="T">CustomData type which can be used during authorization. Loaded from claims using ClaimTypeForCustomData</typeparam>
    /// </summary>
    public class AuthZyinContext<T> : IAuthZyinContext where T : class
    {
        /// <summary>
        /// private instance of the customDataInstance
        /// </summary>
        private T customDataInstance;

        /// <summary>
        /// Gets the custom data factory. Override this to extend the custom data loading behavior.
        // For example your own AuthZyinContext derived calss can depend on other services.
        /// </summary>
        protected virtual Func<T> customDataFactory { get; }

        /// <summary>
        /// Claims accessor
        /// </summary>
        protected AadClaimsAccessor claimsAccessor { get; }

        /// <summary>
        /// Authorization user context
        /// </summary>
        public UserContext UserContext { get; }

        /// <summary>
        // Authorization policy list
        /// </summary>
        public IEnumerable<(string name, AuthorizationPolicy policy)> Policies { get; }

        /// <summary>
        /// Custom data for authorization purpose.
        /// Retrieved via the custom data factory set in construtor.
        /// This is not thread safe so your factory method might be called multiple times.
        /// </summary>
        public T CustomData
        {
            get
            {
                if (this.customDataInstance == null &&
                    this.customDataFactory != null)
                {
                    this.customDataInstance = this.customDataFactory();
                }

                return this.customDataInstance;
            }
        }

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
        public AuthZyinContext(
            IAuthorizationPolicyList policyList,
            IHttpContextAccessor httpContextAccessor)
            : this(policyList?.Policies, httpContextAccessor?.HttpContext?.User)
        {
        }

        /// <summary>
        /// Initializes a new instance of AuthZyinContext for current user
        /// </summary>
        /// <param name="policies">policies defined</param>
        /// <param name="claimsPrincipal">claims principal</param>
        public AuthZyinContext(
            IEnumerable<(string name, AuthorizationPolicy policy)> policies,
            ClaimsPrincipal claimsPrincipal)
        {
            this.Policies = policies ?? throw new ArgumentNullException(nameof(policies));
            claimsPrincipal = claimsPrincipal ?? throw new ArgumentNullException(nameof(claimsPrincipal));

            this.claimsAccessor = new AadClaimsAccessor(claimsPrincipal);
            this.UserContext = new UserContext(claimsAccessor);
        }
    }
}