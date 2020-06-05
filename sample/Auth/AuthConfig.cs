namespace sample.Auth
{
    /// <summary>
    /// Authentication config object
    /// </summary>
    public class AuthConfig
    {
        /// <summary>
        /// AAD v2 authority
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// AAD v2 client app id, used for audience validation
        /// </summary>
        public string AadAppId { get; set; }
    }
}