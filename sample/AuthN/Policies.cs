namespace sample.AuthN
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Authorization;

    public class SamplePolicies
    {
        public static Dictionary<string, AuthorizationPolicy> Policies = new Dictionary<string, AuthorizationPolicy>();

        static SamplePolicies()
        {
            var canEnterBar = new AuthorizationPolicyBuilder()
                .RequireRole(Requirements.CustomerRole)
                .AddRequirements(Requirements.AgeAbove21Requirement)
                .Build();
            Policies.Add(nameof(canEnterBar), canEnterBar);

            var canBuyDrink = new AuthorizationPolicyBuilder()
                .RequireRole(Requirements.CustomerRole)
                .AddRequirements(Requirements.AgeAbove21Requirement, Requirements.HasAcceptedPaymentMethodRequirement)
                .Build();
            Policies.Add(nameof(canBuyDrink), canBuyDrink);
        }
    }
}