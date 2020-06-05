namespace sample.Auth
{
    using AuthZyin.Authorization;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Client data manager
    /// </summary>
    public class SampleClientDataManager : DefaultClientAuthDataManager<Membership>
    {
        /// <summary>
        /// Custom claim to process
        /// </summary>
        protected override string CustomClaimTypeToProcess => Membership.ClaimType;
 
        /// <summary>
        /// Initializes a new instance of the AuthZyinClientDataManager class
        /// </summary>
        /// <param name="policyList">policy list</param>
        /// <param name="contextAccessor">httpContextAccessor</param>
        public SampleClientDataManager(IAuthorizationPolicyList policyList, IHttpContextAccessor contextAccessor)
            : base(policyList, contextAccessor)
        {
        }
    }
}