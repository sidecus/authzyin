namespace sample
{
    using Microsoft.IdentityModel.Tokens;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;

    public static class IssuerValidator
    {
        /// <summary>
        /// Validate the issuer for multi-tenant applications of various audience (Work and School account, or Work and School accounts +
        /// Personal accounts)
        /// </summary>
        /// <param name="issuer">Issuer to validate (will be tenanted)</param>
        /// <param name="securityToken">Received Security Token</param>
        /// <param name="validationParameters">Token Validation parameters</param>
        /// <remarks>The issuer is considered as valid if it has the same http scheme and authority as the
        /// authority from the configuration file, has a tenant Id, and optionally v2.0 (this web api
        /// accepts both V1 and V2 tokens).
        /// Authority aliasing is also taken into account</remarks>
        /// <returns>The <c>issuer</c> if it's valid, or otherwise <c>SecurityTokenInvalidIssuerException</c> is thrown</returns>
        public static string ValidateAadIssuer(string issuer, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            // Extracting the tenant ID
            var tenantId = (securityToken as JwtSecurityToken)?.Claims.FirstOrDefault(c => c.Type == "tid")?.Value;
            if (tenantId == null)
            {
                throw new SecurityTokenInvalidIssuerException("Expecting a valid AAD JWT token with tid claim.");
            }

            // Get all valid issuers and try to see whether current issuer is in the list
            var allValidIssuers = GetAllValidIssuers(validationParameters, tenantId);
            if (allValidIssuers.Contains(issuer))
            {
                return issuer;
            }

            throw new SecurityTokenInvalidIssuerException("Issuer does not match the valid issuers");
        }

        /// <summary>
        /// Get all valid issuers considering multi tenant scenarios as well as AAD v1 and v2 issuer
        /// </summary>
        /// <param name="validationParameters"></param>
        /// <param name="currentTenantId"></param>
        /// <returns></returns>
        private static List<string> GetAllValidIssuers(TokenValidationParameters validationParameters, string currentTenantId)
        {
            // Get all configured issuers in the validation parameter
            var configuredIssuers = validationParameters.ValidIssuers ?? Enumerable.Empty<string>();
            if (validationParameters.ValidIssuer != null)
            {
                configuredIssuers = configuredIssuers.Append(validationParameters.ValidIssuer);
            }

            // Get all valid issuers by replacing {TenantId} with the true tennat id if any
            var allValidIssuers = new List<string>(configuredIssuers.Select(x => GetTenantedIssuer(x, currentTenantId)));

            // Consider the aliases (https://login.microsoftonline.com (v2.0 tokens) => https://sts.windows.net (v1.0 tokens) )
            allValidIssuers.AddRange(allValidIssuers.Select(i => i.Replace("https://login.microsoftonline.com", "https://sts.windows.net")).ToArray());

            // Consider tokens provided both by v1.0 and v2.0 issuers
            allValidIssuers.AddRange(allValidIssuers.Select(i => i.Replace("/v2.0", "/")).ToArray());

            return allValidIssuers;
        }

        /// <summary>
        /// Get a tenant issuer if it contains {tenantid}
        /// </summary>
        /// <param name="issuer">issuer string</param>
        /// <param name="currentTenantId">current tenant id</param>
        /// <returns>a valid issuer with {tenantid} replaced by current tenant id</returns>
        private static string GetTenantedIssuer(string issuer, string currentTenantId)
        {
            return issuer.Replace("{tenantid}", currentTenantId);
        }
    }
}