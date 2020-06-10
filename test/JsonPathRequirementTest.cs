namespace test
{
    using System;
    using Xunit;
    using AuthZyin.Authorization.Requirements;
    using Newtonsoft.Json.Linq;

    public class DummyJsonPathRequirement : JsonPathRequirement<TestCustomData, TestResource>
    {
        public static readonly string DataPath = "dummyDataPath";
        public static readonly string ResourcePath = "dummyResourcePath";
        public new EvaluatorContext GetEvaluatorContext(JObject dataJObject, JObject resourceJObj)
        {
            return base.GetEvaluatorContext(dataJObject, resourceJObj);
        }

        public DummyJsonPathRequirement(Direction direction)
            : base(RequirementOperatorType.GreaterThan, DataPath, ResourcePath, direction)
        {
        }
    }

    public class JsonPathRequirementTest
    {
        [Fact]
        public void ConsructorThrowsOnInvalidArg()
        {
            var resource = new TestResource();

            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonPathRequirement<TestCustomData, TestResource>(
                RequirementOperatorType.Invalid,
                "",
                "",
                Direction.ContextToResource));

            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonPathRequirement<TestCustomData, TestResource>(
                RequirementOperatorType.Or,
                "$",
                "",
                Direction.ContextToResource));

            Assert.Throws<ArgumentNullException>(() => new JsonPathRequirement<TestCustomData, TestResource>(
                RequirementOperatorType.GreaterThan,
                null,
                resource.JPathIntValue,
                Direction.ContextToResource));

            Assert.Throws<ArgumentNullException>(() => new JsonPathRequirement<TestCustomData, TestResource>(
                RequirementOperatorType.RequiresRole,
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
                RequirementOperatorType.Equals,
                dataJPathIntValue,
                resource.JPathIntValue,
                Direction.ContextToResource);

            // Throws if no CustomData is provided - JsonPathRequirement needs custom data
            Assert.Throws<ArgumentNullException>(() => equalsRequirement.Evaluate(context, resource));
        }

        [Fact]
        public void GetEvaluatorContextWorksAsExpected()
        {
            var dataJObject = JObject.FromObject(new { IntValue = 2});
            var resourceJObject = JObject.FromObject(new { SomeOtherValue = "sdfsdf"});

            // context to resource
            var contextToResource = new DummyJsonPathRequirement(Direction.ContextToResource);
            var evaluatorContext = contextToResource.GetEvaluatorContext(dataJObject, resourceJObject);
            Assert.Same(evaluatorContext.LeftJObject, dataJObject);
            Assert.Same(evaluatorContext.LeftJPath, DummyJsonPathRequirement.DataPath);
            Assert.Same(evaluatorContext.RightJObject, resourceJObject);
            Assert.Same(evaluatorContext.RightJPath, DummyJsonPathRequirement.ResourcePath);

            // resource to context
            var resourceToContext = new DummyJsonPathRequirement(Direction.ResourceToContext);
            evaluatorContext = resourceToContext.GetEvaluatorContext(dataJObject, resourceJObject);
            Assert.Same(evaluatorContext.LeftJObject, resourceJObject);
            Assert.Same(evaluatorContext.LeftJPath, DummyJsonPathRequirement.ResourcePath);
            Assert.Same(evaluatorContext.RightJObject, dataJObject);
            Assert.Same(evaluatorContext.RightJPath, DummyJsonPathRequirement.DataPath);
        }

        [Fact]
        public void JsonPathRequirementPositiveBehavior()
        {
            var context = TestContext.CreateDefaultTestContext();
            var resource = new TestResource();

            var greaterThanRequirement = new JsonPathRequirement<TestCustomData, TestResource>(
                RequirementOperatorType.GreaterThan,
                context.CustomData.JPathIntValue,
                resource.JPathNestedDataSmallerIntValue,
                Direction.ContextToResource);
            Assert.True(greaterThanRequirement.Evaluate(context, resource));

            var containsRequirement = new JsonPathRequirement<TestCustomData, TestResource>(
                RequirementOperatorType.GreaterThan,
                context.CustomData.JPathIntValue,
                resource.JPathNestedDataSmallerIntValue,
                Direction.ContextToResource);
            Assert.True(containsRequirement.Evaluate(context, resource));

            var equalsRequirement = new JsonPathRequirement<TestCustomData, TestResource>(
                RequirementOperatorType.Equals,
                context.CustomData.JPathIntValue,
                resource.JPathNestedIntValue,
                Direction.ContextToResource);
            Assert.True(equalsRequirement.Evaluate(context, resource));
        }
    }
}