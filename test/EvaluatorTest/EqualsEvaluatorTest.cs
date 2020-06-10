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
            context.LeftJPath = "$.StringValue";
            context.RightJPath = "$.NestedData.ArrayValue[0]";
            Assert.False(evaluator.Evaluate(context));
        }

        [Fact]
        public void EqualsEvaluatorPositiveBehavior()
        {
            var evaluator = new EqualsEvaluator();
            var (data, resource, context) = EvaluatorTestHelper.CreateEvaluatorContext();

            context.LeftJPath = data.JPathIntValue;
            context.RightJPath = resource.JPathNestedIntValue;
            Assert.True(evaluator.Evaluate(context));

            context.LeftJPath = data.JPathStringValue;
            context.RightJPath = resource.JPathNestedDataStringValue;
            Assert.True(evaluator.Evaluate(context));

            context.LeftJPath = data.JPathGuidValue;
            context.RightJPath = resource.JPathNestedGuidValue;
            Assert.True(evaluator.Evaluate(context));

            context.LeftJPath = data.JPathDateValue;
            context.RightJPath = resource.JPathNestedDateValue;
            Assert.True(evaluator.Evaluate(context));

            // with array filter
            context.LeftJPath = data.JPathArrayValue + "[1]";
            context.RightJPath = resource.JPathNestedDataStringArrayValue + "[1]";
            Assert.True(evaluator.Evaluate(context));
        }
    }
}
