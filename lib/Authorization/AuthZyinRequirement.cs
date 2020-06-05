namespace AuthZyin.Authorization
{
    using AuthZyin.Authentication;
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// Authorization requirement
    /// </summary>
    public class AuthZyinRequirement : IAuthorizationRequirement
    {
        // Evaluate current requirement against given user and resource
        public virtual bool Evaluate(AadClaimsAccessor claimsAccessor, object Resource)
        {
            // TODO[sidecus]
            return true;
        }
    }
}