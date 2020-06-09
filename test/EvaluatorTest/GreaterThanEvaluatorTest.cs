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
            context.Direction = Direction.ContextToResource;
            context.CustomDataPath = data.JPathIntValue;
            context.ResourcePath = resource.JPathIntValue;
            Assert.False(evaluator.Evaluate(context));

            // return false when value is less than instead of greater than
            context.Direction = Direction.ContextToResource;
            context.CustomDataPath = data.JPathIntValue;
            context.ResourcePath = resource.JPathNestedDataBiggerIntValue;
            Assert.False(evaluator.Evaluate(context));

            // same with reverse direction
            context.Direction = Direction.ResourceToContext;
            context.CustomDataPath = data.JPathIntValue;
            context.ResourcePath = resource.JPathNestedDataSmallerIntValue;
            Assert.False(evaluator.Evaluate(context));
        }

        [Fact]
        public void GreaterThanEvaluatorPositiveBehavior()
        {
            var evaluator = new GreaterThanEvaluator();
            var (data, resource, context) = EvaluatorTestHelper.CreateEvaluatorContext();

            // context > resource
            context.Direction = Direction.ContextToResource;
            context.CustomDataPath = data.JPathIntValue;
            context.ResourcePath = resource.JPathNestedDataSmallerIntValue;
            Assert.True(evaluator.Evaluate(context));

            // resource > context
            context.Direction = Direction.ResourceToContext;
            context.CustomDataPath = data.JPathIntValue;
            context.ResourcePath = resource.JPathNestedDataBiggerIntValue;
            Assert.True(evaluator.Evaluate(context));

            // string comparison with with array filter
            context.Direction = Direction.ResourceToContext;
            context.CustomDataPath = data.JPathStringValue;
            context.ResourcePath = resource.JPathNestedDataStringArrayValue + "[0]"; // first string is bigger
            Assert.True(evaluator.Evaluate(context));

            // DateTime - note we need to reset CustomData in the context object
            context.Direction = Direction.ContextToResource;
            data.DateValue = resource.NestedData.DateValue + new TimeSpan(1, 0, 0);
            context.CustomData = JObject.FromObject(data);
            context.CustomDataPath = data.JPathDateValue;
            context.ResourcePath = resource.JPathNestedDateValue;
            Assert.True(evaluator.Evaluate(context));
        }
     }
}