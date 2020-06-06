namespace sample.AuthN
{
    using System.Text.Json;
    using System.Security.Claims;
    using System.Collections.Generic;

    public class PaymentMethod
    {
        public string Type { get; set;  }
        public int Credit { get; set; }
    }

    public class CustomData
    {
        public static readonly string ClaimType = "authzyin.sample.personalinfo";

        /// <summary>
        /// Of age 30
        /// </summary>
        public int Age => 30;

        /// <summary>
        /// Available payment methods
        /// </summary>
        /// <value></value>
        public List<PaymentMethod> PaymentMethods { get; }

        /// <summary>
        /// Gets a claim to serialize into principal
        /// </summary>
        /// <returns></returns>
        public Claim GetClaim() => new Claim(ClaimType, JsonSerializer.Serialize(this));

        /// <summary>
        /// Initializes a new instance of personal data
        /// </summary>
        public CustomData()
        {
            this.PaymentMethods = new List<PaymentMethod>
            {
                new PaymentMethod { Type = "Cache", Credit = 20 },
                new PaymentMethod { Type = "Visa", Credit = 20 },
                new PaymentMethod { Type = "MasterCard", Credit = 100 },
            };
        }
    }
}