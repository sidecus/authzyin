namespace sample.Auth
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;

    /// <summary>
    /// Claims transformer
    /// </summary>
    public class ClaimsTransformer : IClaimsTransformation
    {
        /// <summary>
        /// Transform claims
        /// </summary>
        /// <param name="principal"></param>
        /// <returns>claims principal</returns>
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var ci = principal.Identity as ClaimsIdentity;
            this.AddRoleClaim(ci);
            this.AddMembershipClaim(ci);

            return Task.FromResult(principal);
        }

        /// <summary>
        /// Add role claims
        /// </summary>
        /// <param name="ci">claims identity</param>
        private void AddRoleClaim(ClaimsIdentity ci)
        {
            ci.AddClaim(new Claim(ci.RoleClaimType, "user"));
        }

        /// <summary>
        /// Add custom membership claim
        /// </summary>
        /// <param name="ci">claims identity</param>
        private void AddMembershipClaim(ClaimsIdentity ci)
        {
            var membership = new Membership();

            ci.AddClaim(membership.GetClaim());
        }
    }
}