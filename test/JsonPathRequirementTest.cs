namespace test
{
    using System;
    using Xunit;
    using AuthZyin.Authorization.Requirements;
    using AuthZyin.Authorization.JPathRequirements;

    public class JsonPathRequirementTest
    {
        [Fact]
        public void ConsructorThrowsOnInvalidArg()
        {
            var resource = new TestResource();

            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonPathRequirement<TestCustomData, TestResource>(
                OperatorType.Invalid,
                "",
                "",
                Direction.ContextToResource));

            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonPathRequirement<TestCustomData, TestResource>(
                OperatorType.Or,
                "$",
                "",
                Direction.ContextToResource));

            Assert.Throws<ArgumentNullException>(() => new JsonPathRequirement<TestCustomData, TestResource>(
                OperatorType.GreaterThan,
                null,
                resource.JPathIntValue,
                Direction.ContextToResource));

            Assert.Throws<ArgumentNullException>(() => new JsonPathRequirement<TestCustomData, TestResource>(
                OperatorType.RequiresRole,
                "abc",
                null,
                Direction.ContextToResource));
        }
        
        [Fact]
        public void EvaluateMethodThrowsWithNullCustomData()
        {
            var dataJPathIntValue = new TestCustomData().JPathIntValue;

            var context = TestContext.CreateDefaultTestContext(setNullCustomData: true);
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
            var context = TestContext.CreateDefaultTestContext();
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