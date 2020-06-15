namespace sample.AuthN
{
    using AuthZyin.Authorization.Requirements;
    using Microsoft.AspNetCore.Authorization;

    public class Policies
    {
        // Declarative policy for a generic place
        public static readonly AuthorizationPolicy IsCustomer = new AuthorizationPolicyBuilder()
            .RequireRole(Requirements.CustomerRole)
            .Build();

        // Declarative policy without using resource - CanDrinkAlchohol
        public static readonly AuthorizationPolicy CanDrinkAlchohol = new AuthorizationPolicyBuilder()
            .Combine(Policies.IsCustomer)
            .AddRequirements(
                Requirements.HasValidId,
                Requirements.AgeAbove21)
            .Build();

        // Imperative policy on top of CanDrink - need Bar object as resource
        public static readonly AuthorizationPolicy CanEnterBar = new AuthorizationPolicyBuilder()
            .Combine(Policies.CanDrinkAlchohol)
            .AddRequirements(Requirements.HasAcceptedPaymentMethod)
            .Build();

        // Imperative policy for places which has their own age limit
        public static readonly AuthorizationPolicy MeetsAgeRangeLimit = new AuthorizationPolicyBuilder()
            .RequireRole(Requirements.CustomerRole)
            .AddRequirements(
                Requirements.MeetsMinAgeLimit,
                Requirements.MeetsMaxAgeLimit,
                Requirements.HasAcceptedPaymentMethod)
            .Build();

        // Imperative policy on top of CanEnterBar - need Bar object as resource
        public static readonly AuthorizationPolicy CanBuyDrink = new AuthorizationPolicyBuilder()
            .RequireRole(Requirements.CustomerRole)
            .Build();
    }

    public class Requirements
    {
        public static readonly string CustomerRole = "Customer";

        // Has driver's license
        public static readonly EqualsValueRequirement<AuthorizationData, bool> HasDriversLicense =
            new EqualsValueRequirement<AuthorizationData, bool>(dataPath: "$.WithDriversLicense", value: true);

        // Has Passport
        public static readonly EqualsValueRequirement<AuthorizationData, bool> HasPassport =
            new EqualsValueRequirement<AuthorizationData, bool>(dataPath: "$.WithPassport", value: true);

        // Has valid ID
        public static readonly OrRequirement HasValidId = new OrRequirement(HasDriversLicense, HasPassport);

        // const Age greater than 20 (or GTE 21)
        public static readonly GreaterThanValueRequirement<AuthorizationData, int> AgeAbove21 =
            new GreaterThanValueRequirement<AuthorizationData, int>(dataPath: "$.Age", value: 20);

        // Has a payment method which the place accepts
        public static readonly ContainsRequirement<AuthorizationData, Place> HasAcceptedPaymentMethod =
            new ContainsRequirement<AuthorizationData, Place>(dataPath: "$.PaymentMethods[*].Type", resourcePath: "$.AcceptedPaymentMethods[0]", direction: Direction.ContextToResource);

        // Age requirement based on resource
        public static readonly GreaterThanOrEqualToRequirement<AuthorizationData, AgeLimitedPlace> MeetsMinAgeLimit =
            new GreaterThanOrEqualToRequirement<AuthorizationData, AgeLimitedPlace>(dataPath: "$.Age", resourcePath: "$.MinAge", direction: Direction.ContextToResource);

        public static readonly GreaterThanOrEqualToRequirement<AuthorizationData, AgeLimitedPlace> MeetsMaxAgeLimit =
            new GreaterThanOrEqualToRequirement<AuthorizationData, AgeLimitedPlace>(dataPath: "$.Age", resourcePath: "$.MaxAge", direction: Direction.ResourceToContext);
    }
}