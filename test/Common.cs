namespace test
{
    using System;
    using Xunit;
    using AuthZyin.Authorization.Requirements;
    using AuthZyin.Authorization;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Authorization;
    using System.Security.Claims;

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
    }
}