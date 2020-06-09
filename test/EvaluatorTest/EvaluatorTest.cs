namespace test
{
    using System;
    using Newtonsoft.Json.Linq;
    using Xunit;
    using AuthZyin.Authorization.Requirements;
    using System.Collections.Generic;

    public class RequirementEvaluatorTest
    {
        public static readonly IEnumerable<object[]> evaluators = new List<object[]>
        {
            new object[] { new EqualsEvaluator(), },
            new object[] { new GreaterThanEvaluator(), },
        };

        [Fact]
        public void EvaluateThrowsOnInvalidArg()
        {
            var evaluator = new EqualsEvaluator();

            // context is null
            Assert.Throws<ArgumentNullException>(() => evaluator.Evaluate(null));

            var context = new EvaluatorContext();

            // CustomData is null
            Assert.Throws<ArgumentNullException>(() => evaluator.Evaluate(null));
            context.CustomData = JObject.FromObject(new TestCustomData());

            // Resource is null
            Assert.Throws<ArgumentNullException>(() => evaluator.Evaluate(context));
            context.Resource = JObject.FromObject(new TestResource());

            // Direction is invalid
            Assert.Throws<ArgumentOutOfRangeException>(() => evaluator.Evaluate(context));
            context.Direction = (Direction)3;
            Assert.Throws<ArgumentOutOfRangeException>(() => evaluator.Evaluate(context));
            context.Direction = Direction.ResourceToContext;

            // CustomDataPath is null or white spaces
            Assert.Throws<ArgumentNullException>(() => evaluator.Evaluate(context));
            context.CustomDataPath = "    \t";
            Assert.Throws<ArgumentNullException>(() => evaluator.Evaluate(context));
            context.CustomDataPath = "$.something";

            // ResourcePath is null or white spaces
            Assert.Throws<ArgumentNullException>(() => evaluator.Evaluate(context));
            context.ResourcePath = "    \t  ";
            Assert.Throws<ArgumentNullException>(() => evaluator.Evaluate(context));
        }

        // Helper method to test JValue based evaluator - equals, greater than etc.
        [Theory]
        [MemberData(nameof(evaluators))]
        public static void TestJValueCommonInvalidScenarios(RequirementEvaluator evaluator)
        {
            // Custom data path doesn't point to valid value
            var context = EvaluatorTestHelper.CreateEvaluatorContext().context;
            context.CustomDataPath = "$.Nonexisting";
            Assert.False(evaluator.Evaluate(context));

            // Resource data path doesn't point to valid value
            context = EvaluatorTestHelper.CreateEvaluatorContext().context;
            context.ResourcePath = "$.Nonexisting";
            Assert.False(evaluator.Evaluate(context));

            // Context member is not a value
            context = EvaluatorTestHelper.CreateEvaluatorContext().context;
            context.CustomDataPath = $"$.ArrayValue";
            Assert.False(evaluator.Evaluate(context));

            // Resource data is not a value
            context = EvaluatorTestHelper.CreateEvaluatorContext().context;
            context.ResourcePath = "$.NestedData.ArrayValue";
            Assert.False(evaluator.Evaluate(context));

            // Different token type
            context = EvaluatorTestHelper.CreateEvaluatorContext().context;
            context.CustomDataPath = "$.IntValueInString";
            context.ResourcePath = "$.NestedData.IntValue";
            Assert.False(evaluator.Evaluate(context));
        }
   }

    public class EvaluatorTestHelper
    {
        public static (TestCustomData data, TestResource resource, EvaluatorContext context) CreateEvaluatorContext()
        {
            var data = new TestCustomData();
            var resource = new TestResource();
            var context = new EvaluatorContext();
            context.CustomData = JObject.FromObject(data);
            context.Resource = JObject.FromObject(resource);
            context.CustomDataPath = $"$.{nameof(data.StringValue)}";
            context.ResourcePath = $"$.NestedData.{nameof(resource.NestedData.StringValue)}";
            context.Direction = Direction.ContextToResource;

            return (data, resource, context);
        }
    }
}