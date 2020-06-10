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
            context.LeftJObject = JObject.FromObject(new TestCustomData());

            // Resource is null
            Assert.Throws<ArgumentNullException>(() => evaluator.Evaluate(context));
            context.RightJObject = JObject.FromObject(new TestResource());

            // CustomDataPath is null or white spaces
            Assert.Throws<ArgumentNullException>(() => evaluator.Evaluate(context));
            context.LeftJPath = "    \t";
            Assert.Throws<ArgumentNullException>(() => evaluator.Evaluate(context));
            context.LeftJPath = "$.something";

            // ResourcePath is null or white spaces
            Assert.Throws<ArgumentNullException>(() => evaluator.Evaluate(context));
            context.RightJPath = "    \t  ";
            Assert.Throws<ArgumentNullException>(() => evaluator.Evaluate(context));
        }

        // Helper method to test JValue based evaluator - equals, greater than etc.
        [Theory]
        [MemberData(nameof(evaluators))]
        public static void TestJValueCommonInvalidScenarios(RequirementEvaluator evaluator)
        {
            // Custom data path doesn't point to valid value
            var context = EvaluatorTestHelper.CreateEvaluatorContext().context;
            context.LeftJPath = "$.Nonexisting";
            Assert.False(evaluator.Evaluate(context));

            // Resource data path doesn't point to valid value
            context = EvaluatorTestHelper.CreateEvaluatorContext().context;
            context.RightJPath = "$.Nonexisting";
            Assert.False(evaluator.Evaluate(context));

            // Context member is not a value
            context = EvaluatorTestHelper.CreateEvaluatorContext().context;
            context.LeftJPath = $"$.ArrayValue";
            Assert.False(evaluator.Evaluate(context));

            // Resource data is not a value
            context = EvaluatorTestHelper.CreateEvaluatorContext().context;
            context.RightJPath = "$.NestedData.ArrayValue";
            Assert.False(evaluator.Evaluate(context));

            // Different token type
            context = EvaluatorTestHelper.CreateEvaluatorContext().context;
            context.LeftJPath = "$.IntValueInString";
            context.RightJPath = "$.NestedData.IntValue";
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
            context.LeftJObject = JObject.FromObject(data);
            context.RightJObject = JObject.FromObject(resource);
            context.LeftJPath = data.JPathStringValue;
            context.RightJPath = resource.JPathNestedDataStringValue;

            return (data, resource, context);
        }
    }
}