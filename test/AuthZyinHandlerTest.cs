namespace test
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using AuthZyin.Authorization;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Authorization.Infrastructure;
    using Microsoft.Extensions.Logging;
    using Xunit;

    public class AuthZyinHandlerTest
    {
        private readonly AuthZyinContext<TestCustomData> context = new TestContext();
        private readonly ILogger<AuthZyinHandler> logger = TestLoggerFactory.CreateLogger<AuthZyinHandler>();

        [Fact]
        public void ConstructorThrowsOnInvalidArg()
        {
            Assert.Throws<ArgumentNullException>(() => new AuthZyinHandler(null, this.logger));
            Assert.Throws<ArgumentNullException>(() => new AuthZyinHandler(this.context, null));
        }

        [Fact]
        public async Task OnlyHandlesAuthZyinRequirement()
        {
            var testRole = "TestRole";
            var testRoleRequirement = new RolesAuthorizationRequirement(new[] { testRole });
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(identity.RoleClaimType, testRole));
            var principal = new ClaimsPrincipal(identity);
            var authorizationContext = new AuthorizationHandlerContext(new []{ testRoleRequirement }, principal, null);

            var handler = new AuthZyinHandler(this.context, this.logger);
            await handler.HandleAsync(authorizationContext);
            Assert.Single(authorizationContext.Requirements);
            Assert.Single(authorizationContext.PendingRequirements);
        }

        [Fact]
        public async Task SucceedsWhenAllRequirementsSucceed()
        {
            var principal = new ClaimsPrincipal();
            var authorizationContext = new AuthorizationHandlerContext(
                new []
                {
                    TestRequirement.TrueRequirement,
                    TestRequirement.TrueRequirement,
                    TestRequirement.TrueRequirement
                },
                principal,
                null);

            var handler = new AuthZyinHandler(this.context, this.logger);
            await handler.HandleAsync(authorizationContext);
            Assert.Empty(authorizationContext.PendingRequirements);
        }

        [Fact]
        public async Task FailsWhenOneRequirementFails()
        {
            var principal = new ClaimsPrincipal();
            var authorizationContext = new AuthorizationHandlerContext(
                new []
                {
                    TestRequirement.TrueRequirement,
                    TestRequirement.TrueRequirement,
                    TestRequirement.FalseRequirement
                },
                principal,
                null);

            var handler = new AuthZyinHandler(this.context, this.logger);
            await handler.HandleAsync(authorizationContext);
            Assert.Single(authorizationContext.PendingRequirements);
            Assert.All(authorizationContext.PendingRequirements, r => object.ReferenceEquals(r, TestRequirement.FalseRequirement));
        }
    }
}