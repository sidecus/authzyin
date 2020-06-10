namespace test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using AuthZyin.Authorization;
    using AuthZyin.Authorization.Client;
    using Microsoft.AspNetCore.Authorization;
    using Xunit;

    public class AuthZyinContextTest
    {
        [Fact]
        public void ConstructorThrowsOnInvalidArg()
        {
            var policies = new List<(string name, AuthorizationPolicy policy)>{ ("nullpolicy", null), };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            
            Assert.Throws<ArgumentNullException>(() => new AuthZyinContext<TestCustomData>(null, claimsPrincipal));
            Assert.Throws<ArgumentNullException>(() => new AuthZyinContext<TestCustomData>(policies, null));
        }

        [Fact]
        public void UserContextAndPoliciesAreConstructedCorrectly()
        {
            var userId = Guid.NewGuid().ToString();
            var userName = "testusre";
            var roles = new string[] { "role1", "role2" };
            var testCustomData = new TestCustomData
            {
                IntValue = 234226546,
            };
            var policyBuilder = new AuthorizationPolicyBuilder();
            roles.ToList().ForEach(r => policyBuilder.RequireRole(r));

            var policies = new List<(string name, AuthorizationPolicy policy)>
            {
                ("rquiresRolePolicy", policyBuilder.Build()),
            };

            var context = TestContext.CreateTestContext(
                userId,
                userName,
                roles,
                policies,
                testCustomData);
                
            Assert.Same(context.Policies, policies);
            Assert.NotNull(context.UserContext);
            Assert.Same(context.UserContext.UserId, userId);
            Assert.Same(context.UserContext.UserName, userName);

            // Make sure custom data factory is used
            Assert.NotNull(context.Data);
            Assert.Same(context.Data, testCustomData);

            // Check client context generation
            var clientContext = context.ClientContext as ClientContext<TestCustomData>;
            Assert.NotNull(clientContext);
            Assert.Same(clientContext.UserContext, context.UserContext);
            Assert.Same(clientContext.Data, context.Data);
        }
    }
}