namespace sample.AuthN
{
    using AuthZyin.Authorization;

    public class Bar : AuthZyinResource
    {
        /// <summary>
        /// Accepted payment method
        /// </summary>
        public string AcceptedPaymentMethod { get; set; }
    }
}