namespace AuthZyin.Authorization
{
    using AuthZyin.Authentication;
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// Authorization resource base class
    /// </summary>
    public class AuthzinResource {}

    /// <summary>
    /// Authorization requirement
    /// </summary>
    public abstract class AuthZyinRequirement: IAuthorizationRequirement
    {
        /// <summary>
        // Evaluate current requirement against given user and resource
        /// </summary>
        /// <param name="claimsAccessor">cliams accessor representing current user</param>
        /// <param name="resource">resource object</param>
        /// <returns>true if allowed</returns>
        public abstract bool Evaluate(AadClaimsAccessor claimsAccessor, object resource);
    }

    /// <summary>
    /// Authorization requirement
    /// </summary>
    public abstract class AuthZyinRequirement<T> : AuthZyinRequirement
        where T: AuthzinResource
    {
        /// <summary>
        // Evaluate current requirement against given user and resource
        /// </summary>
        /// <param name="claimsAccessor">cliams accessor representing current user</param>
        /// <param name="resource">resource object</param>
        /// <returns>true if allowed</returns>
        public override bool Evaluate(AadClaimsAccessor claimsAccessor, object resource)
        {
            return this.EvaluateWithTypeResource(claimsAccessor, resource as T);
        }

        /// <summary>
        // Evaluate current requirement against given user and typed resource
        /// </summary>
        /// <param name="claimsAccessor">cliams accessor representing current user</param>
        /// <param name="resource">resource object</param>
        /// <returns>true if allowed</returns>
        protected abstract bool EvaluateWithTypeResource(AadClaimsAccessor claimsAccessor, T typedResource);
    }
}