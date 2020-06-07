namespace sample.AuthN
{
    using System.Collections.Generic;
    using AuthZyin.Authorization;


    public class Drink : Resource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
    }
    
    public class Bar : Resource
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<string> AcceptedPaymentMethods { get; set; }

        public static IEnumerable<Bar> Bars = new List<Bar>
        {
            new Bar
            {
                Id = 1,
                Name = "Dark Ravern",
                AcceptedPaymentMethods = new List<string>
                {
                    PaymentMethod.Visa,
                },
            },
            new Bar
            {
                Id = 2,
                Name = "Blue Sky",
                AcceptedPaymentMethods = new List<string>
                {
                    PaymentMethod.Cash,
                },
            },
            new Bar
            {
                Id = 2,
                Name = "One World",
                AcceptedPaymentMethods = new List<string>
                {
                    PaymentMethod.MasterCard,
                },
            }
        };
    }
}