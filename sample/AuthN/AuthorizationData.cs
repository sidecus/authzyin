namespace sample.AuthN
{
    using System.Text.Json;
    using System.Security.Claims;
    using System.Collections.Generic;
    using System;

    /// <summary>
    /// pay methods
    /// </summary>
    public class PaymentMethod
    {
        public static readonly string Cash = "Cash";
        public static readonly string Visa = "Visa";
        public static readonly string MasterCard = "MasterCard";

        public string Type { get; set;  }
        public int Credit { get; set; }
    }

    /// <summary>
    /// Sample authorization data which can be used to authorize user in addition to things like roles
    /// </summary>
    public class AuthorizationData
    {
        public static readonly string ClaimType = "authzyin.sample.authorizationdata";

        /// <summary>
        /// Of age 30
        /// </summary>
        public int Age => 40;

        /// <summary>
        /// Have driver's license with you?
        /// </summary>
        public bool WithDriversLicense => false;

        /// <summary>
        /// Have passport with you?
        /// </summary>
        public bool WithPassport => true;

        /// <summary>
        /// Available payment methods
        /// </summary>
        /// <value></value>
        public List<PaymentMethod> PaymentMethods { get; }

        /// <summary>
        /// Initializes a new instance of personal data
        /// </summary>
        public AuthorizationData()
        {
            this.PaymentMethods = new List<PaymentMethod>
            {
                new PaymentMethod { Type = PaymentMethod.Cash, Credit = 20 },
                new PaymentMethod { Type = PaymentMethod.Visa, Credit = 20 },
            };
        }

        /// <summary>
        /// Convert current object to a claim
        /// </summary>
        /// <returns>claim</returns>
        public Claim ToClaim()
        {
            return new Claim(AuthorizationData.ClaimType, JsonSerializer.Serialize(this));
        }

        /// <summary>
        /// Construct a new AuthorizationData from claims
        /// </summary>
        /// <returns>AuthorizationData object</returns>
        public static AuthorizationData FromClaim(Claim claim)
        {
            if (claim == null || claim.Value == null)
            {
                return null;
            }

            if (claim.Type != AuthorizationData.ClaimType)
            {
                throw new ArgumentOutOfRangeException("unexpected claim type");
            }

            return JsonSerializer.Deserialize<AuthorizationData>(claim.Value);
        }
    }
}