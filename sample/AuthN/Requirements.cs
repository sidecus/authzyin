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
        public static readonly JsonPathConstantRequirement<AuthorizationData, bool> HasDriversLicense = new JsonPathConstantRequirement<AuthorizationData, bool>(
            operatorType: OperatorType.Equals,
            dataPath: "$.WithDriversLicense",
            constValue: true);

        // Has Passport
        public static readonly JsonPathConstantRequirement<AuthorizationData, bool> HasPassport = new JsonPathConstantRequirement<AuthorizationData, bool>(
            operatorType: OperatorType.Equals,
            dataPath: "$.WithPassport",
            constValue: true);

        // Has valid ID
        public static readonly OrRequirement HasValidId = new OrRequirement(HasDriversLicense, HasPassport);

        // const Age above 21
        public static readonly JsonPathConstantRequirement<AuthorizationData, int> AgeAbove21 = new JsonPathConstantRequirement<AuthorizationData, int>(
            operatorType: OperatorType.GreaterThan,
            dataPath: "$.Age",
            constValue: 21);

        // Has a payment method which the place accepts
        public static readonly JsonPathRequirement<AuthorizationData, Place> HasAcceptedPaymentMethod = new JsonPathRequirement<AuthorizationData, Place>(
            operatorType: OperatorType.Contains,
            dataPath: "$.PaymentMethods[*].Type",
            resourcePath: "$.AcceptedPaymentMethods[0]",
            direction: Direction.ContextToResource);

        // Age requirement based on resource
        public static readonly JsonPathRequirement<AuthorizationData, AgeLimitedPlace> MeetsMinAgeLimit = new JsonPathRequirement<AuthorizationData, AgeLimitedPlace>(
            operatorType: OperatorType.GreaterThanOrEqualTo,
            dataPath: "$.Age",
            resourcePath: "$.MinAge",
            direction: Direction.ContextToResource);

        public static readonly JsonPathRequirement<AuthorizationData, AgeLimitedPlace> MeetsMaxAgeLimit = new JsonPathRequirement<AuthorizationData, AgeLimitedPlace>(
            operatorType: OperatorType.GreaterThanOrEqualTo,
            dataPath: "$.Age",
            resourcePath: "$.MaxAge",
            direction: Direction.ResourceToContext);
    }
}