namespace sample.AuthN
{
    using System.Text.Json;
    using AuthZyin.Authorization;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Authorization context for the sample project
    /// </summary>
    public class SampleAuthZyinContext : AuthZyinContext<AuthorizationData>
    {
        /// <summary>
        /// Initializes a new instance of the SampleAuthZyinContext class
        /// </summary>
        /// <param name="policyList">policy list</param>
        /// <param name="contextAccessor">httpContextAccessor</param>
        public SampleAuthZyinContext(IAuthorizationPolicyList policyList, IHttpContextAccessor contextAccessor)
            : base(policyList, contextAccessor)
        {
        }


        /// <summary>
        /// Get authorization data from claims. In reality you can get this from anywhere
        /// </summary>
        /// <returns>additional data for authorization purpose</returns>
        protected override AuthorizationData CreateCustomData()
        {
            // Retrieves the custom data json string from claims (if any).
            // It's denoted by a virtual member CustomClaimTypeToProcess.
            // Usually it's not safe to call virtual member in the constructor, but it's safe
            // here since CustomClaimTypeToProcess is just meant to return a string constant.
            var claim = this.claimsAccessor.GetClaim(AuthorizationData.ClaimType);
            return AuthorizationData.FromClaim(claim);
        }
    }
}