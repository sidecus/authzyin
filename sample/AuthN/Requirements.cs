namespace sample.AuthN
{
    using AuthZyin.Authorization.Requirements;
    using Microsoft.AspNetCore.Authorization;

    public class Policies
    {
        // Declarative policy without using resource - CanDrink
        public static readonly AuthorizationPolicy AlchoholReady = new AuthorizationPolicyBuilder()
            .RequireRole(Requirements.CustomerRole)
            .AddRequirements(
                Requirements.HasValidId,
                Requirements.AgeAbove21)
            .Build();

        // Imperative policy on top of CanDrink - need Bar object as resource
        public static readonly AuthorizationPolicy CanEnterBar = new AuthorizationPolicyBuilder()
            .Combine(Policies.AlchoholReady)
            .AddRequirements(Requirements.HasAcceptedPaymentMethod)
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
            operatorType: RequirementOperatorType.Equals,
            dataPath: "$.WithDriversLicense",
            constValue: true);

        // Has Passport
        public static readonly JsonPathConstantRequirement<AuthorizationData, bool> HasPassport = new JsonPathConstantRequirement<AuthorizationData, bool>(
            operatorType: RequirementOperatorType.Equals,
            dataPath: "$.WithPassport",
            constValue: true);

        // Has valid ID
        public static readonly OrRequirement HasValidId = new OrRequirement(HasDriversLicense, HasPassport);

        // Age above 21
        public static readonly JsonPathConstantRequirement<AuthorizationData, int> AgeAbove21 = new JsonPathConstantRequirement<AuthorizationData, int>(
            operatorType: RequirementOperatorType.GreaterThan,
            dataPath: "$.Age",
            constValue: 21);

        // Has a payment method which the bar accepts
        public static readonly JsonPathRequirement<AuthorizationData, Bar> HasAcceptedPaymentMethod = new JsonPathRequirement<AuthorizationData, Bar>(
            operatorType: RequirementOperatorType.Contains,
            dataPath: "$.PaymentMethods[*].Type",
            resourcePath: "$.AcceptedPaymentMethods[0]",
            direction: Direction.ContextToResource);
    }
}