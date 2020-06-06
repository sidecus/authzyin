namespace sample.AuthN
{
    using AuthZyin.Authorization;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Authorization context for the sample project
    /// </summary>
    public class SampleAuthZyinContext : AuthZyinContext<CustomData>
    {
        /// <summary>
        /// Custom claim to process
        /// </summary>
        protected override string ClaimTypeForCustomData => CustomData.ClaimType;
 
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