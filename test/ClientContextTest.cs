namespace test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AuthZyin.Authorization.Client;
    using Microsoft.AspNetCore.Authorization;
    using Xunit;

    public class ClientContextTest
    {
        [Fact]
        public void ConstructorThrowsOnInvalidArg()
        {
            Assert.Throws<ArgumentNullException>(() => new ClientContext(null));
        }

        [Fact]
        public void UserContextAndPoliciesAreConstructedCorrectly()
        {
            var context = new TestContext();
            var clientContext = new ClientContext(context);

            // Check client context generation
            Assert.Same(clientContext.UserContext, context.UserContext);
            Assert.Same(clientContext.Data, context.Data);
            Assert.Equal(context.Policies.Count(), context.Policies.Count());
            foreach (var policy in context.Policies)
            {
                Assert.Contains(clientContext.Policies, x => x.Name == policy.name);
            }
        }
    }
}