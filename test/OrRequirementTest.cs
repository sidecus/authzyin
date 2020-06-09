namespace test
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using AuthZyin.Authorization;
    using AuthZyin.Authorization.Requirements;
    using Microsoft.AspNetCore.Authorization;
    using Xunit;

    public class OrRequirementTest
    {
        private AuthZyinContext<TestCustomData> context;

        private TestRequirement<Resource> trueRequirement = new TestRequirement<Resource>(false, result:true);
        private TestRequirement<Resource> falseRequirement = new TestRequirement<Resource>(false, result:false);

        public OrRequirementTest()
        {
            var policies = new List<(string name, AuthorizationPolicy policy)>{ ("nullpolicy", null), };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            this.context = new AuthZyinContext<TestCustomData>(policies, claimsPrincipal);
        }

        [Fact]
        public void OrRequirementConstructorThrowsOnInvalidArg()
        {
            Assert.Throws<ArgumentNullException>(() => new OrRequirement());
            Assert.Throws<ArgumentNullException>(() => new OrRequirement(null));
            Assert.Throws<ArgumentNullException>(() => new OrRequirement(new TestRequirement<Resource>[]{}));
        }

        [Fact]
        public void OrRequirementBehavesCorrectly()
        {
            var trueFalseOr = new OrRequirement(trueRequirement, falseRequirement);
            Assert.Equal(RequirementOperatorType.Or, trueFalseOr.Operator);
            Assert.True(trueFalseOr.Evaluate(this.context, null));
            Assert.Equal(2, trueFalseOr.Children.Length);

            var falseTrueOr = new OrRequirement(falseRequirement, trueRequirement);
            Assert.Equal(RequirementOperatorType.Or, falseTrueOr.Operator);
            Assert.True(falseTrueOr.Evaluate(this.context, null));
            Assert.Equal(2, trueFalseOr.Children.Length);

            var allFalseOr = new OrRequirement(falseRequirement, falseRequirement);
            Assert.Equal(RequirementOperatorType.Or, allFalseOr.Operator);
            Assert.False(allFalseOr.Evaluate(this.context, null));
            Assert.Equal(2, trueFalseOr.Children.Length);
        }

        [Fact]
        public void OrRequirementChildrenSerializesCorrectly()
        {
            var trueFalseOr = new OrRequirement(trueRequirement, falseRequirement);
            var json = System.Text.Json.JsonSerializer.Serialize(trueFalseOr);

            // Check the JSON string to make sure the derived requirement is serialized correctly
            // via OrRequirement.children
            var position = -1;
            for (var i = 0; i < trueFalseOr.Children.Length; i ++)
            {
                position = json.IndexOf(trueRequirement.Marker, position + 1);
                Assert.True(position > 0);
            }

            // Make sure we don't support deserialization - it's not needed and can be dangerous.
            Assert.Throws<NotSupportedException>(() => System.Text.Json.JsonSerializer.Deserialize<OrRequirement>(json));
        }
    }
}