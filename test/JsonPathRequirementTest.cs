namespace test
{
    using System;
    using Xunit;
    using AuthZyin.Authorization.Requirements;
    using AuthZyin.Authorization.JPathRequirements;

    public class JsonPathRequirementTest
    {
        private static readonly TestResource TestResource = new TestResource();
        public static readonly object[][] ConstrucNegativeCases = new []
        {
            new object[] { OperatorType.Invalid, string.Empty, string.Empty, Direction.ContextToResource },
            new object[] { OperatorType.Or, "$", string.Empty, Direction.ContextToResource },
            new object[] { OperatorType.GreaterThan, null, TestResource.JPathIntValue, Direction.ContextToResource },
            new object[] { OperatorType.RequiresRole, "abc", null, Direction.ResourceToContext },
            new object[] { OperatorType.RequiresRole, "abc", "dcd", (Direction) 0 },
        };

        [Theory]
        [MemberData(nameof(ConstrucNegativeCases))]
        public void ConsructorThrowsOnInvalidArg(
            OperatorType operatorType,
            string dataPath,
            string resourcePath,
            Direction direction
        )
        {
            Assert.ThrowsAny<ArgumentException>(() => new JsonPathRequirement<TestCustomData, TestResource>(
                operatorType,
                dataPath,
                resourcePath,
                direction));
        }
        
        [Fact]
        public void EvaluateMethodThrowsWithNullCustomData()
        {
            var dataJPathIntValue = new TestCustomData().JPathIntValue;

            var context = new TestContext(setNullCustomData: true);
            var resource = new TestResource();

            var equalsRequirement = new JsonPathRequirement<TestCustomData, TestResource>(
                OperatorType.Equals,
                dataJPathIntValue,
                resource.JPathIntValue,
                Direction.ContextToResource);

            // Throws if no CustomData is provided - JsonPathRequirement needs custom data
            Assert.Throws<ArgumentNullException>(() => equalsRequirement.Evaluate(context, resource));
        }

        [Fact]
        public void JsonPathRequirementPositiveBehavior()
        {
            var context = new TestContext();
            var resource = new TestResource();

            var greaterThanRequirement = new JsonPathRequirement<TestCustomData, TestResource>(
                OperatorType.GreaterThan,
                context.Data.JPathIntValue,
                resource.JPathNestedDataSmallerIntValue,
                Direction.ContextToResource);
            Assert.True(greaterThanRequirement.Evaluate(context, resource));

            var containsRequirement = new JsonPathRequirement<TestCustomData, TestResource>(
                OperatorType.GreaterThan,
                context.Data.JPathIntValue,
                resource.JPathNestedDataSmallerIntValue,
                Direction.ContextToResource);
            Assert.True(containsRequirement.Evaluate(context, resource));

            var equalsRequirement = new JsonPathRequirement<TestCustomData, TestResource>(
                OperatorType.Equals,
                context.Data.JPathIntValue,
                resource.JPathNestedIntValue,
                Direction.ContextToResource);
            Assert.True(equalsRequirement.Evaluate(context, resource));
        }
    }
}