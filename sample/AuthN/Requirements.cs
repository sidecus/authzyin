namespace sample.AuthN
{
    using AuthZyin.Authorization.Requirements;

    public class Requirements
    {
        public static readonly string CustomerRole = "Customer";

        // Has driver's license
        public static readonly JsonPathConstantRequirement<CustomData, bool> HasDriversLicense = new JsonPathConstantRequirement<CustomData, bool>(
            operatorType: RequirementOperatorType.Equals,
            dataPath: "$.WithDriversLicense",
            constValue: true,
            direction: Direction.ContextToResource);

        // Has Passport
        public static readonly JsonPathConstantRequirement<CustomData, bool> HasPassport = new JsonPathConstantRequirement<CustomData, bool>(
            operatorType: RequirementOperatorType.Equals,
            dataPath: "$.WithPassport",
            constValue: true,
            direction: Direction.ContextToResource);

        // Has valid ID
        public static readonly OrRequirement HasValidId = new OrRequirement(HasDriversLicense, HasPassport);

        // Age above 21
        public static readonly JsonPathConstantRequirement<CustomData, int> AgeAbove21 = new JsonPathConstantRequirement<CustomData, int>(
            operatorType: RequirementOperatorType.GreaterThan,
            dataPath: "$.Age",
            constValue: 21,
            direction: Direction.ContextToResource);

        // Has a payment method which the bar accepts
        public static readonly JsonPathRequirement<CustomData, Bar> HasAcceptedPaymentMethod = new JsonPathRequirement<CustomData, Bar>(
            operatorType: RequirementOperatorType.Contains,
            dataPath: "$.PaymentMethods[*].Type",
            resourcePath: "$.AcceptedPaymentMethod",
            direction: Direction.ContextToResource);
    }
}