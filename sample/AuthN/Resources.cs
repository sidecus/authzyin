namespace sample.AuthN
{
    using AuthZyin.Authorization;

    public class Bar : Resource
    {
        /// <summary>
        /// Accepted payment method
        /// </summary>
        public string AcceptedPaymentMethod { get; set; }
    }

    public class Beer : Resource
    {
        public int Price { get; set; }
    }
}