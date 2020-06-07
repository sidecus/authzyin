namespace sample.AuthN
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Authorization;

    public class SamplePolicies
    {
        public static readonly Dictionary<string, AuthorizationPolicy> Policies = new Dictionary<string, AuthorizationPolicy>();

        // Declarative policy without using resource - CanDrink
        public static readonly AuthorizationPolicy CanDrink = new AuthorizationPolicyBuilder()
            .RequireRole(Requirements.CustomerRole)
            .AddRequirements(
                Requirements.HasValidId,
                Requirements.AgeAbove21)
            .Build();

        // Imperative policy on top of CanDrink - need Bar object as resource
        public static readonly AuthorizationPolicy CanEnterBar = new AuthorizationPolicyBuilder()
            .AddRequirements(Requirements.HasAcceptedPaymentMethod)
            .Build();

        // Imperative policy on top of CanEnterBar - need Bar object as resource
        public static readonly AuthorizationPolicy CanBuyDrink = new AuthorizationPolicyBuilder()
            .RequireRole(Requirements.CustomerRole)
            .Build();

        static SamplePolicies()
        {
            Policies.Add(nameof(CanDrink), CanDrink);
            Policies.Add(nameof(CanEnterBar), CanEnterBar);
            Policies.Add(nameof(CanBuyDrink), CanBuyDrink);
        }
    }
}