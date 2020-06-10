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
                context.LeftJPath = "$.Nonexisting";
                Assert.False(evaluator.Evaluate(context));

                // Resource data path doesn't point to valid value
                context = EvaluatorTestHelper.CreateEvaluatorContext().context;
                context.RightJPath = "$.Nonexisting";
                Assert.False(evaluator.Evaluate(context));
            }

            // Left operand is not an IEnumerable<JValue>
            {
                var (data, resource, context) = EvaluatorTestHelper.CreateEvaluatorContext();
                context.LeftJPath = "$";
                Assert.False(evaluator.Evaluate(context));
            }

            // right operand is not a value
            {
                var (data, resource, context) = EvaluatorTestHelper.CreateEvaluatorContext();
                context.LeftJPath = data.JPathArrayValue + "[*]";
                context.RightJPath = resource.JPathNestedDataStringArrayValue;
                Assert.False(evaluator.Evaluate(context));
            }

            // Different token type
            {
                var (data, resource, context) = EvaluatorTestHelper.CreateEvaluatorContext();
                context.LeftJPath = data.JPathIntValue;
                context.RightJPath = resource.JPathNestedDataStringArrayValue + "[0]";
                Assert.False(evaluator.Evaluate(context));
            }

            // doesnt' contain
            {
                var (data, resource, context) = EvaluatorTestHelper.CreateEvaluatorContext();
                context.LeftJPath = data.JPathStringValue;
                context.RightJPath = resource.JPathNestedDataStringArrayValue + "[0]";
                Assert.False(evaluator.Evaluate(context));
            }
        }

        [Fact]
        public void ContainsEvaluatorPositiveBehavior()
        {
            var evaluator = new ContainsEvaluator();
            var (data, resource, context) = EvaluatorTestHelper.CreateEvaluatorContext();

            // left contains right - int
            context.LeftJPath = data.JPathIntValue;
            context.RightJPath = resource.JPathIntValue;
            Assert.True(evaluator.Evaluate(context));

            // left contains right - string
            context.LeftJPath = data.JPathArrayValue + "[*]";
            context.RightJPath = resource.JPathNestedDataStringValue;
            Assert.True(evaluator.Evaluate(context));

            // Guid contains
            context.LeftJPath = data.JPathGuidValue;
            context.RightJPath = resource.JPathNestedGuidValue;
            Assert.True(evaluator.Evaluate(context));
        }
     }
}