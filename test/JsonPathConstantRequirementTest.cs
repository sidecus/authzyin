namespace test
{
    using System;
    using Xunit;
    using AuthZyin.Authorization.Requirements;
    using System.Security.Claims;
    using AuthZyin.Authorization;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Authorization;

    public class JsonPathConstantRequirementTest
    {
        [Fact]
        public void ConsructorThrowsOnInvalidArg()
        {
            Assert.Throws<ArgumentNullException>(() => new JsonPathConstantRequirement<TestCustomData, int>(
                RequirementOperatorType.GreaterThan,
                null,
                3,
                Direction.ContextToResource));

            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonPathConstantRequirement<TestCustomData, int>(
                RequirementOperatorType.Invalid,
                "",
                3,
                Direction.ContextToResource));

            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonPathConstantRequirement<TestCustomData, int>(
                RequirementOperatorType.RequiresRole,
                string.Empty,
                3,
                Direction.ContextToResource));

            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonPathConstantRequirement<TestCustomData, int>(
                RequirementOperatorType.Or,
                "$",
                3,
                Direction.ResourceToContext));
        }

        [Fact] void SetsConstResourceAndPathCorrectly()
        {
            var conextPath = "$.IntMember";
            var constRequirement = new JsonPathConstantRequirement<TestCustomData, int>(
                RequirementOperatorType.Equals,
                conextPath,
                3,
                Direction.ResourceToContext);
            
            Assert.Equal(Direction.ResourceToContext, constRequirement.Direction);
            Assert.Equal(conextPath, constRequirement.DataJPath);
            Assert.Equal(JsonPathConstantRequirement<TestCustomData, int>.ConstResourceValueJPath, constRequirement.ResourceJPath);
            Assert.Equal(RequirementOperatorType.Equals, constRequirement.Operator);
        }

        [Fact] void LoadsConstantWrapperResourceCorrectly()
        {
            var policies = new List<(string name, AuthorizationPolicy policy)>{ ("nullpolicy", null), };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            var testCustomData = new TestCustomData
            {
                IntMember = 1056,
                StringMember = "this is a string",
            };
            var context = new TestContext(policies, claimsPrincipal, testCustomData);

            var conextPath = "$.IntMember";
            var constRequirement = new JsonPathConstantRequirement<TestCustomData, int>(
                RequirementOperatorType.Equals,
                conextPath,
                1056,
                Direction.ResourceToContext);
            Assert.True(constRequirement.Evaluate(context, null));
        }
    }
}