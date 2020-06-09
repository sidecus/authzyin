namespace test
{
    using System;
    using Xunit;
    using AuthZyin.Authorization.Requirements;
    using AuthZyin.Authorization;
    using System.Security.Claims;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Authorization;

    public class RequirementTest
    {
        private List<(string name, AuthorizationPolicy policy)> policies = new List<(string name, AuthorizationPolicy policy)>{ ("nullpolicy", null), };
        private ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        private AuthZyinContext<TestCustomData> context;
        private TestResource resource = new TestResource();
        private TestResource2 invalidResource = new TestResource2();

        public RequirementTest()
        {
            this.context = new AuthZyinContext<TestCustomData>(this.policies, this.claimsPrincipal);
        }

        [Fact]
        public void EvaluateThrowsOnInvalidContext()
        {
            var wrongContext = new AuthZyinContext<object>(this.policies, this.claimsPrincipal);
            var requirement = new TestRequirement<TestResource>(true, true);

            // context check
            Assert.Throws<ArgumentNullException>(() => requirement.Evaluate(null, resource));
            Assert.Throws<InvalidCastException>(() => requirement.Evaluate(wrongContext, resource));
        }

        [Fact]
        public void EvaluateThrowsOnInvalidResourceWhenRequired()
        {
            var requirement = new TestRequirement<TestResource>(true, true);

            // requires resource behavior
            Assert.Throws<ArgumentNullException>(() => requirement.Evaluate(context, null));
            Assert.Throws<InvalidCastException>(() => requirement.Evaluate(context, invalidResource));
        }

        [Fact]
        public void EvaluateWorksWithInvalidResourceWhenNotRequired()
        {
            var wrongContext = new AuthZyinContext<object>(this.policies, this.claimsPrincipal);
            var requirement = new TestRequirement<TestResource>(false, true);

            // doesn't require resource behavior
            Assert.True(requirement.Evaluate(context, null));
            Assert.True(requirement.Evaluate(context, invalidResource));
        }

        [Fact]
        public void EvaluateReturnsProtectedEvaluateResult()
        {
            var trueRequirement = new TestRequirement<TestResource>(false, true);
            Assert.True(trueRequirement.Evaluate(context, null));

            var falseRequirement = new TestRequirement<TestResource>(false, false);
            Assert.False(falseRequirement.Evaluate(context, null));
        }
    }
}
