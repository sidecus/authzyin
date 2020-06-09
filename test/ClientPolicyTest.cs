namespace test
{
    using System;
    using System.Linq;
    using AuthZyin.Authorization.Client;
    using AuthZyin.Authorization.Requirements;
    using Microsoft.AspNetCore.Authorization;
    using Xunit;

    public class ClientPolicyTest
    {
        private readonly AuthorizationPolicy policy;

        public ClientPolicyTest()
        {
            this.policy = new AuthorizationPolicyBuilder()
                .RequireRole(new [] { nameof(ClientPolicyTest) })
                .RequireAuthenticatedUser()
                .AddRequirements(new [] { TestRequirement.TrueRequirement, TestRequirement.FalseRequirement})
                .Build();
        }

        [Fact]
        public void ConstructorThrowsOnInvalidArg()
        {
            Assert.Throws<ArgumentNullException>(() => new ClientPolicy(null, this.policy));
            Assert.Throws<ArgumentNullException>(() => new ClientPolicy("policy", null));
        }

        [Fact]
        public void ConvertsRequirementsCorrectly()
        {
            var clientPolicy = new ClientPolicy("policy", this.policy);

            Assert.Equal(4, this.policy.Requirements.Count());

            // Should be 3 instead of 4 since we'll be ignoring DenyAnonymouseAuthorizationRole
            Assert.Equal(3, clientPolicy.Requirements.Count());

            Assert.True(clientPolicy.Requirements.All(r => this.IsTargetRequirement(r)));
            
            Assert.Single(clientPolicy.Requirements, r => r is ClientRoleRequirement);
            Assert.Single(clientPolicy.Requirements, TestRequirement.TrueRequirement);
            Assert.Single(clientPolicy.Requirements, TestRequirement.FalseRequirement);
        }

        private bool IsTargetRequirement(object r)
        {
            return r is Requirement;
        }
    }
}
