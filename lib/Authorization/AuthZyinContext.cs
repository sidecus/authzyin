namespace AuthZyin.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using AuthZyin.Authentication;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Interface to construct required authorization data.
    /// !!!Must be registered as scoped!!!
    /// </summary>
    public interface IAuthZyinContext
    {
        /// <summary>
        /// User context contains basic user info plus roles
        /// </summary>
        UserContext UserContext { get; }

        /// <summary>
        // Authorization policy list
        /// </summary>
        IEnumerable<(string name, AuthorizationPolicy policy)> Policies { get; }

        /// <summary>
        /// Gets the data object as type object
        /// </summary>
        object GetData();

        /// <summary>
        /// Get the data object as JObject. This will be used by JSONPath based requirement evaluation.
        /// </summary>
        /// <returns>An JObject representing the data object</returns>
        JObject GetDataAsJObject();
    }

    /// <summary>
    /// AuthZyinContext managing data required during authorization.
    /// !!!Must be registered as scoped!!!
    /// <typeparam name="T">Type of the custom Data which can be used during authorization.</typeparam>
    /// </summary>
    public class AuthZyinContext<T> : IAuthZyinContext where T : class
    {
        /// <summary>
        /// Claims accessor
        /// </summary>
        protected AadClaimsAccessor claimsAccessor { get; }

        /// <summary>
        /// Lazy data T to avoid unnecessary computation if not needed.
        /// </summary>
        protected Lazy<T> lazyData { get; }

        /// <summary>
        /// Lazy JObject data which will be called by Json Path based requirements evaluation (if any).
        /// </summary>
        /// <value></value>
        protected Lazy<JObject> lazyJObject { get; }

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
        /// </summary>
        public T Data => this.lazyData.Value;

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
            this.lazyData = new Lazy<T>(this.CreateData);
            this.lazyJObject = new Lazy<JObject>(() => JObject.FromObject(this.Data));
        }

        /// <summary>
        /// Get the data object as JObject. This will be used by JSONPath based requirement evaluation.
        /// </summary>
        /// <returns>An JObject representing the data object</returns>
        public JObject GetDataAsJObject() => this.lazyJObject.Value;

        /// <summary>
        /// Gets an object representing the authorization context to send to client
        /// Implementing IAuthZyinContext.
        /// </summary>
        public object GetData() => this.Data;

        /// <summary>
        /// Gets data needed. Override this in your own AuthZyinContext implementation if you use JsonPath
        /// based requirements. This will only be called once for each Http request scope.
        /// </summary>
        /// <returns>Data object of type T</returns>
        protected virtual T CreateData() => null;
    }
}