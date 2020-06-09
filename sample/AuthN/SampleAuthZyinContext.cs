namespace sample.AuthN
{
    using System;
    using System.Text.Json;
    using AuthZyin.Authorization;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Authorization context for the sample project
    /// </summary>
    public class SampleAuthZyinContext : AuthZyinContext<AuthorizationData>
    {
        /// <summary>
        // Implements the custom data factory.
        // You can derive your own AuthZyinContext and use more dependencies
        // to produce the factory method.
        /// </summary>
        /// <returns>a factory method which generates the required custom data</returns>
        protected override Func<AuthorizationData> customDataFactory => () =>
        {
            // Retrieves the custom data json string from claims (if any).
            // It's denoted by a virtual member CustomClaimTypeToProcess.
            var claim = this.claimsAccessor.GetClaim(AuthorizationData.ClaimType);
            return AuthorizationData.FromClaim(claim);
        };

        /// <summary>
        /// Initializes a new instance of the SampleAuthZyinContext class
        /// </summary>
        /// <param name="policyList">policy list</param>
        /// <param name="contextAccessor">httpContextAccessor</param>
        public SampleAuthZyinContext(IAuthorizationPolicyList policyList, IHttpContextAccessor contextAccessor)
            : base(policyList, contextAccessor)
        {
        }
    }
}