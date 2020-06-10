namespace sample.AuthN
{
    using System.Collections.Generic;
    using AuthZyin.Authorization;

    public class Place: Resource
    {
        public virtual string Policy => nameof(Policies.IsCustomer);

        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<string> AcceptedPaymentMethods { get; set; }
    }

    public class Bar: Place
    {
        public override string Policy => nameof(Policies.CanEnterBar);
        public bool HasHappyHour => true;
    }

    public class AgeLimitedPlace: Place
    {
        public override string Policy => nameof(Policies.MeetsAgeRangeLimit);
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
    }
}