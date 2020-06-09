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
        public int IntMember { get; set; }
        public string StringMember { get; set; }
    }

    public class TestResource : Resource {};

    public class TestResource2 : Resource {};

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

        public static TestContext CreateDefaultTestContext()
        {
            var userId = Guid.NewGuid().ToString();
            var userName = "testusre";
            var roles = new string[] { "role1", "role2" };
            var testCustomData = new TestCustomData
            {
                IntMember = -2342546,
            };
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
                testCustomData);
        }
    }
}