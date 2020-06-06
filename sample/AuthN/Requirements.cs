namespace sample.AuthN
{
    using AuthZyin.Authorization;

    public class Requirements
    {
        public static readonly string CustomerRole = "Customer";

        public static readonly GreaterThanValueRequirement<PersonalData> AgeAbove21Requirement
            = new GreaterThanValueRequirement<PersonalData>("$.Age", 21);

        public static readonly JsonPathContainsRequirement<PersonalData, Bar> HasAcceptedPaymentMethodRequirement
            = new JsonPathContainsRequirement<PersonalData, Bar>("$.PaymentMethods[*].Type", "$.AcceptedPaymentMethod", Direction.ContextToResource);
    }
}