namespace test
{
    using System;
    using System.Linq;
    using AuthZyin.Authorization.Client;
    using AuthZyin.Authorization.Requirements;
    using Microsoft.AspNetCore.Authorization;
    using Xunit;

    public class ClientRoleRequirementTest
    {
        [Fact]
        public void ConstructorThrowsOnInvalidArg()
        {
            Assert.Throws<ArgumentNullException>(() => new ClientRoleRequirement(null));
        }

        [Fact]
        public void OperatorAndRolesAreSetCorrectly()
        {
            var roles = new[] { "role1", "role2", "role3", };
            var requirement = new ClientRoleRequirement(roles);

            Assert.Equal(OperatorType.RequiresRole, requirement.Operator);
            Assert.Equal(requirement.AllowedRoles, roles);
        }
    }
}
