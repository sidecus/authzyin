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
                3));

            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonPathConstantRequirement<TestCustomData, int>(
                RequirementOperatorType.Invalid,
                "",
                3));

            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonPathConstantRequirement<TestCustomData, int>(
                RequirementOperatorType.RequiresRole,
                string.Empty,
                3));

            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonPathConstantRequirement<TestCustomData, int>(
                RequirementOperatorType.Or,
                "$",
                3));
        }

        [Fact]
        public void SetsConstResourceAndPathAndDirectionCorrectly()
        {
            var conextPath = "$.IntMember";
            var constRequirement = new JsonPathConstantRequirement<TestCustomData, int>(
                RequirementOperatorType.Equals,
                conextPath,
                3);
            
            Assert.Equal(Direction.ContextToResource, constRequirement.Direction);
            Assert.Equal(conextPath, constRequirement.DataJPath);
            Assert.Equal(ConstantWrapperResource<int>.GetValueMemberJPath(), constRequirement.ResourceJPath);
            Assert.Equal(RequirementOperatorType.Equals, constRequirement.Operator);
        }

        [Fact]
        public void LoadsConstantWrapperResourceCorrectly()
        {
            var policies = new List<(string name, AuthorizationPolicy policy)>{ ("nullpolicy", null), };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            var testCustomData = new TestCustomData
            {
                IntValue = 1056,
                StringValue = "this is a string",
            };
            var context = new TestContext(policies, claimsPrincipal, testCustomData);

            var conextPath = testCustomData.JPathIntValue;
            var constRequirement = new JsonPathConstantRequirement<TestCustomData, int>(
                operatorType: RequirementOperatorType.Equals,
                dataPath: conextPath,
                constValue: 1056);
            Assert.True(constRequirement.Evaluate(context, null));
        }

        [Fact]
        public void ConstantWrapperResourceValueJsonPath()
        {
            // Ensure the wrapper resource use a member named "Value" to save the constant right operand.
            // It's needed since the client is betting it to be harded coded to "$.Value".
            Assert.Equal("$.Value", ConstantWrapperResource<int>.GetValueMemberJPath());
            Assert.Equal("$.Value", ConstantWrapperResource<string>.GetValueMemberJPath());
            Assert.Equal("$.Value", ConstantWrapperResource<DateTime>.GetValueMemberJPath());
        }
    }
}