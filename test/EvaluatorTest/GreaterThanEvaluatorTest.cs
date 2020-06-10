namespace test
{
    using System;
    using Xunit;
    using AuthZyin.Authorization.Requirements;
    using Newtonsoft.Json.Linq;

    public class GreaterThanEvaluatorTest
    {
        [Fact]
        public void GreaterThanEvaluatorNegativeBehavior()
        {
            var evaluator = new GreaterThanEvaluator();

            // RequirementEvaluatorTest.TestJValueCommonInvalidScenarios(evaluator);

            var (data, resource, context) = EvaluatorTestHelper.CreateEvaluatorContext();

            // return false when value is equal instead of greater than
            context.LeftJPath = data.JPathIntValue;
            context.RightJPath = resource.JPathIntValue;
            Assert.False(evaluator.Evaluate(context));

            // return false when value is less than instead of greater than
            context.LeftJPath = data.JPathIntValue;
            context.RightJPath = resource.JPathNestedDataBiggerIntValue;
            Assert.False(evaluator.Evaluate(context));
        }

        [Fact]
        public void GreaterThanEvaluatorPositiveBehavior()
        {
            var evaluator = new GreaterThanEvaluator();
            var (data, resource, context) = EvaluatorTestHelper.CreateEvaluatorContext();

            // context > resource
            context.LeftJPath = data.JPathIntValue;
            context.RightJPath = resource.JPathNestedDataSmallerIntValue;
            Assert.True(evaluator.Evaluate(context));

            // string comparison with with array filter
            context.LeftJPath = data.JPathStringValue;
            context.RightJPath = resource.JPathNestedDataStringArrayValue + "[-1:]"; // last value is smaller
            Assert.True(evaluator.Evaluate(context));

            // DateTime - note we need to reset CustomData in the context object
            data.DateValue = resource.NestedData.DateValue + new TimeSpan(1, 0, 0);
            context.LeftJObject = JObject.FromObject(data);
            context.LeftJPath = data.JPathDateValue;
            context.RightJPath = resource.JPathNestedDateValue;
            Assert.True(evaluator.Evaluate(context));
        }
     }
}