namespace sample.AuthN
{
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
        // Creates our own authorization data
        /// </summary>
        /// <returns>the authorization data object</returns>
        protected override AuthorizationData CreateData()
        {
            // Retrieves the custom data json string from claims (if any).
            // It's denoted by a virtual member CustomClaimTypeToProcess.
            var claim = this.claimsAccessor.GetClaim(AuthorizationData.ClaimType);
            return AuthorizationData.FromClaim(claim);
        }
    }
}