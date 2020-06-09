namespace test
{
    using System;
    using Xunit;
    using AuthZyin.Authorization.Requirements;
    using Newtonsoft.Json.Linq;

    public class ContainsEvaluatorTest
    {
        [Fact]
        public void ContainsEvaluatorNegativeBehavior()
        {
            var evaluator = new ContainsEvaluator();

            {
                // Custom data path doesn't point to valid value
                var context = EvaluatorTestHelper.CreateEvaluatorContext().context;
                context.CustomDataPath = "$.Nonexisting";
                Assert.False(evaluator.Evaluate(context));

                // Resource data path doesn't point to valid value
                context = EvaluatorTestHelper.CreateEvaluatorContext().context;
                context.ResourcePath = "$.Nonexisting";
                Assert.False(evaluator.Evaluate(context));
            }

            // Left operand is not an IEnumerable<JValue>
            {
                var (data, resource, context) = EvaluatorTestHelper.CreateEvaluatorContext();
                context.CustomDataPath = "$";
                Assert.False(evaluator.Evaluate(context));
            }

            // right operand is not a value
            {
                var (data, resource, context) = EvaluatorTestHelper.CreateEvaluatorContext();
                context.Direction = Direction.ContextToResource;
                context.CustomDataPath = data.JPathArrayValue + "[*]";
                context.ResourcePath = resource.JPathNestedDataStringArrayValue;
                Assert.False(evaluator.Evaluate(context));
            }

            // Different token type
            {
                var (data, resource, context) = EvaluatorTestHelper.CreateEvaluatorContext();
                context.Direction = Direction.ResourceToContext;
                context.CustomDataPath = data.JPathIntValue;
                context.ResourcePath = resource.JPathNestedDataStringArrayValue + "[0]";
                Assert.False(evaluator.Evaluate(context));
            }

            // doesnt' contain
            {
                var (data, resource, context) = EvaluatorTestHelper.CreateEvaluatorContext();
                context.Direction = Direction.ResourceToContext;
                context.CustomDataPath = data.JPathStringValue;
                context.ResourcePath = resource.JPathNestedDataStringArrayValue + "[0]";
                Assert.False(evaluator.Evaluate(context));
            }
        }

        [Fact]
        public void ContainsEvaluatorPositiveBehavior()
        {
            var evaluator = new ContainsEvaluator();
            var (data, resource, context) = EvaluatorTestHelper.CreateEvaluatorContext();

            // context contains resource - int
            context.Direction = Direction.ContextToResource;
            context.CustomDataPath = data.JPathIntValue;
            context.ResourcePath = resource.JPathIntValue;
            Assert.True(evaluator.Evaluate(context));

            // resource contains context - int
            context.Direction = Direction.ResourceToContext;
            context.CustomDataPath = data.JPathIntValue;
            context.ResourcePath = resource.JPathNestedIntValue;
            Assert.True(evaluator.Evaluate(context));

            // resource contains context - string
            context.Direction = Direction.ResourceToContext;
            context.CustomDataPath = data.JPathStringValue;
            context.ResourcePath = resource.JPathNestedDataStringArrayValue + "[*]";
            Assert.True(evaluator.Evaluate(context));

            // context contains resource - Guid
            context.Direction = Direction.ContextToResource;
            context.CustomDataPath = data.JPathGuidValue;
            context.ResourcePath = resource.JPathNestedGuidValue;
            Assert.True(evaluator.Evaluate(context));
        }
     }
}