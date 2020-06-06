namespace sample.AuthN
{
    using AuthZyin.Authorization;

    public class Requirements
    {
        public static readonly string CustomerRole = "Customer";

        public static readonly GreaterThanValueRequirement<CustomData> AgeAbove21Requirement
            = new GreaterThanValueRequirement<CustomData>("$.Age", 21);

        public static readonly JsonPathContainsRequirement<CustomData, Bar> HasAcceptedPaymentMethodRequirement
            = new JsonPathContainsRequirement<CustomData, Bar>("$.PaymentMethods[*].Type", "$.AcceptedPaymentMethod", Direction.ContextToResource);
    }
}