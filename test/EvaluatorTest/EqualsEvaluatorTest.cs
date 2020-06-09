namespace test
{
    using Xunit;
    using AuthZyin.Authorization.Requirements;

    public class EqualsEvaluatorTest
    {
        [Fact]
        public void EqualsEvaluatorNegativeBehavior()
        {
            var evaluator = new EqualsEvaluator();

            // RequirementEvaluatorTest.TestJValueCommonInvalidScenarios(evaluator);

            // Value doesn't equal
            var context = EvaluatorTestHelper.CreateEvaluatorContext().context;
            context.CustomDataPath = "$.StringValue";
            context.ResourcePath = "$.NestedData.ArrayValue[0]";
            Assert.False(evaluator.Evaluate(context));
        }

        [Fact]
        public void EqualsEvaluatorPositiveBehavior()
        {
            var evaluator = new EqualsEvaluator();
            var (data, resource, context) = EvaluatorTestHelper.CreateEvaluatorContext();

            context.CustomDataPath = $"$.{nameof(data.IntValue)}";
            context.ResourcePath = $"$.NestedData.{nameof(resource.NestedData.IntValue)}";
            Assert.True(evaluator.Evaluate(context));

            context.CustomDataPath = $"$.{nameof(data.StringValue)}";
            context.ResourcePath = $"$.NestedData.{nameof(resource.NestedData.StringValue)}";
            Assert.True(evaluator.Evaluate(context));

            context.CustomDataPath = $"$.{nameof(data.GuidValue)}";
            context.ResourcePath = $"$.NestedData.{nameof(resource.NestedData.GuidValue)}";
            Assert.True(evaluator.Evaluate(context));

            context.CustomDataPath = $"$.{nameof(data.DateValue)}";
            context.ResourcePath = $"$.NestedData.{nameof(resource.NestedData.DateValue)}";
            Assert.True(evaluator.Evaluate(context));

            // with array filter
            context.CustomDataPath = $"$.{nameof(data.ArrayValue)}[1]";
            context.ResourcePath = "$..ArrayValue[-1:]";
            Assert.True(evaluator.Evaluate(context));
        }
    }
}
