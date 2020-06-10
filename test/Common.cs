namespace test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authorization;
    using AuthZyin.Authorization.Requirements;
    using AuthZyin.Authorization;
    using AuthZyin.Authentication;
    using Newtonsoft.Json.Linq;

    public class TestRequirement<T> : Requirement<TestCustomData, T> where T: Resource
    {
        private bool needResource = false;
        private bool result = false;
        protected override bool NeedResource => this.needResource;

        public string Marker => "$$TestRequirement$$";

        public TestRequirement(bool needResource, bool result)
        {
            this.needResource = needResource;
            this.result = result;
        }

        protected override bool Evaluate(AuthZyinContext<TestCustomData> context, T resource)
        {
            return result;
        }
    }

    public class TestRequirement : TestRequirement<Resource>
    {
        public TestRequirement(bool needResource, bool result) : base(needResource, result)
        {
        }

        public static readonly TestRequirement TrueRequirement = new TestRequirement(false, result:true);
        public static readonly TestRequirement FalseRequirement = new TestRequirement(false, result:false);
    }

    public class TestCustomData
    {
        public static readonly int IntConst = 98873452;
        public static readonly string StringConst = "#%\t^$\r\n%^\"\\.$^@#@sdfsdf";
        public static readonly DateTime DateConst = DateTime.UtcNow;
        public static readonly Guid GuidConst = Guid.NewGuid();

        public int IntValue { get; set; } = IntConst;
        public int BiggerIntValue { get; set; } = IntConst + 1;
        public int SmallerIntValue { get; set; } = IntConst - 1;
        public string IntValueInString { get; set; } = IntConst.ToString();
        public string StringValue { get; set; } = StringConst;
        public Guid GuidValue { get; set; } = GuidConst;
        public DateTime DateValue { get; set; } = DateConst;
        public string[] ArrayValue { get; set; } = new [] { StringConst + "abcd", StringConst, StringConst.Substring(0, StringConst.Length - 2) };

        public string JPathIntValue => $"$.{nameof(this.IntValue)}";
        public string JPathStringValue => $"$.{nameof(this.StringValue)}";
        public string JPathDateValue => $"$.{nameof(this.DateValue)}";
        public string JPathArrayValue => $"$.{nameof(this.ArrayValue)}";
        public string JPathGuidValue => $"$.{nameof(this.GuidValue)}";
    }

    public class TestResource : Resource
    {
        public TestCustomData NestedData { get; set; } = new TestCustomData();
        public int IntValue { get; set; } = TestCustomData.IntConst;

        public string JPathIntValue => $"$.{nameof(this.IntValue)}";
        public string JPathNestedDataBiggerIntValue => $"$.{nameof(this.NestedData)}.{nameof(this.NestedData.BiggerIntValue)}";
        public string JPathNestedDataSmallerIntValue => $"$.{nameof(this.NestedData)}.{nameof(this.NestedData.SmallerIntValue)}";
        public string JPathNestedDataStringArrayValue => $"$.{nameof(this.NestedData)}.{nameof(this.NestedData.ArrayValue)}";
        public string JPathNestedDateValue => $"$.{nameof(this.NestedData)}.{nameof(this.NestedData.DateValue)}";
        public string JPathNestedDataStringValue => $"$.{nameof(this.NestedData)}.{nameof(this.NestedData.StringValue)}";
        public string JPathNestedIntValue => $"$.{nameof(this.NestedData)}.{nameof(this.NestedData.IntValue)}";
        public string JPathNestedGuidValue => $"$.{nameof(this.NestedData)}.{nameof(this.NestedData.GuidValue)}";
    }

    public class TestContext : AuthZyinContext<TestCustomData>
    {
        private readonly TestCustomData testCustomData;

        public TestContext(
            IEnumerable<(string name, AuthorizationPolicy policy)> policies,
            ClaimsPrincipal claimsPrincipal,
            TestCustomData testCustomData) : base(policies, claimsPrincipal)
        {
            this.testCustomData = testCustomData;
        }

        protected override Func<TestCustomData> customDataFactory => () => this.testCustomData;

        public static TestContext CreateTestContext(
            string userId,
            string userName,
            IEnumerable<string> roles,
            List<(string name, AuthorizationPolicy policy)> policies,
            TestCustomData testCustomData)
        {
            var userIdClaim = new Claim(AadClaimsAccessor.UserIdClaimType, userId);
            var userNameClaim = new Claim(AadClaimsAccessor.NameClaimType, userName);

            var identity = new ClaimsIdentity();
            identity.AddClaims(new Claim[] { userIdClaim, userNameClaim });
            identity.AddClaims(roles.Select(r => new Claim(AadClaimsAccessor.RoleClaimType, r)));

            return new TestContext(policies, new ClaimsPrincipal(identity), testCustomData);
        }

        public static TestContext CreateDefaultTestContext(bool setNullCustomData = false)
        {
            var userId = Guid.NewGuid().ToString();
            var userName = "testusre";
            var roles = new string[] { "role1", "role2" };
            var testCustomData = new TestCustomData();
            var policyBuilder = new AuthorizationPolicyBuilder();
            roles.ToList().ForEach(r => policyBuilder.RequireRole(r));
            var policy1 = policyBuilder.Build();
            var policy2 = policyBuilder.Build();

            var policies = new List<(string name, AuthorizationPolicy policy)>
            {
                (nameof(policy1), policy1), (nameof(policy2), policy2),
            };

            return TestContext.CreateTestContext(
                userId,
                userName,
                roles,
                policies,
                setNullCustomData ? null : testCustomData);
        }
    }
}