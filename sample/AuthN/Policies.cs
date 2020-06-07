namespace sample.AuthN
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Authorization;

    public class SamplePolicies
    {
        public static readonly Dictionary<string, AuthorizationPolicy> Policies = new Dictionary<string, AuthorizationPolicy>();

        public static readonly AuthorizationPolicy CanEnterBar = new AuthorizationPolicyBuilder()
            .RequireRole(Requirements.CustomerRole)
            .AddRequirements(
                Requirements.HasValidId,
                Requirements.AgeAbove21)
            .Build();

        public static readonly AuthorizationPolicy CanBuyDrink = new AuthorizationPolicyBuilder()
            .RequireRole(Requirements.CustomerRole)
            .AddRequirements(
                Requirements.HasValidId,
                Requirements.AgeAbove21,
                Requirements.HasAcceptedPaymentMethod)
            .Build();

        static SamplePolicies()
        {
            Policies.Add(nameof(CanEnterBar), CanEnterBar);
            Policies.Add(nameof(CanBuyDrink), CanBuyDrink);
        }
    }
}