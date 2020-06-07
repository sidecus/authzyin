namespace sample
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using sample.AuthN;

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
            this.AddCustomDataClaim(ci);

            return Task.FromResult(principal);
        }

        /// <summary>
        /// Add role claims
        /// </summary>
        /// <param name="ci">claims identity</param>
        private void AddRoleClaim(ClaimsIdentity ci)
        {
            ci.AddClaim(new Claim(ci.RoleClaimType, Requirements.CustomerRole));
        }

        /// <summary>
        /// Load and add custom data as part of the claim
        /// </summary>
        /// <param name="ci">claims identity</param>
        private void AddCustomDataClaim(ClaimsIdentity ci)
        {
            var data = new AuthorizationData();
            ci.AddClaim(data.ToClaim());
        }
    }
}