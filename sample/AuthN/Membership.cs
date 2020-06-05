namespace sample.AuthN
{
    using System.Text.Json;
    using System.Security.Claims;
    using System.Collections.Generic;
    using AuthZyin.Authorization;

    public class Department : AuthZyinResource
    {
        public int RegionId { get; set; }
        public int DepartmentId { get; set; }
    }

    public class Membership
    {
        public static readonly string ClaimType = "authzyin.sample.membership";

        public List<Department> AdminOf { get; set; }

        public Membership()
        {
            this.AdminOf = new List<Department>
            {
                new Department { RegionId = 1, DepartmentId = 1 },
                new Department { RegionId = 1, DepartmentId = 2},
                new Department { RegionId = 2, DepartmentId = 1 },
            };
        }

        public Claim GetClaim() => new Claim(ClaimType, JsonSerializer.Serialize(this));
    }
}