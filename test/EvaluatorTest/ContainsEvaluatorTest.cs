namespace test
{
    using System;
    using Xunit;
    using AuthZyin.Authorization.Requirements;
    using AuthZyin.Authorization.JPathRequirements;
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;

    public class ContainsEvaluatorTest
    {
        private static readonly TestCustomData data = new TestCustomData();
        private static readonly TestResource resource = new TestResource();
        private static readonly JObject dataJObj = JObject.FromObject(data);
        private static readonly JObject resourceObj = JObject.FromObject(resource);

        public static readonly IEnumerable<object[]> negativeCases = new List<object[]>
        {
            // Custom data path doesn't point to valid value
            new object[] { dataJObj, "$.Nonexisting", resourceObj, resource.JPathNestedDataStringValue },

            // Resource data path doesn't point to valid value
            new object[] { dataJObj, data.JPathStringValue, resourceObj, "$.Nonexisting" },

            // Left operand is not an IEnumerable<JValue>
            new object[] { dataJObj, "$", resourceObj, resource.JPathNestedDataStringValue },

            // right operand is not a value
            new object[] { dataJObj, data.JPathArrayValue + "[*]", resourceObj, resource.JPathNestedDataStringArrayValue },

            // Different token type
            new object[] { dataJObj, data.JPathIntValue, resourceObj, resource.JPathNestedDataStringArrayValue + "[0]" },

            // doesnt' contain
            new object[] { dataJObj, data.JPathStringValue, resourceObj, resource.JPathNestedDataStringArrayValue + "[0]" },
        };

        public static readonly IEnumerable<object[]> positiveCases = new List<object[]>
        {
            // contains - int
            new object[] { dataJObj, data.JPathIntValue, resourceObj, resource.JPathIntValue },

            // ontains - string
            new object[] { dataJObj, data.JPathArrayValue + "[*]", resourceObj, resource.JPathNestedDataStringValue },

            // Guid contains
            new object[] { dataJObj, data.JPathGuidValue, resourceObj, resource.JPathNestedGuidValue },
        };

        [Fact]
        public void ThrowsOnNullContxt()
        {
            // context is null
            Assert.Throws<ArgumentNullException>(() => new ContainsEvaluator().Evaluate(null));
        }

        [Theory]
        [MemberData(nameof(negativeCases))]
        public void ContainsEvaluatorNegativeBehavior(
            JObject dataJObj,
            string dataJPath,
            JObject resourceJObj,
            string resourceJPath)
        {
            var evaluator = new ContainsEvaluator();
            var context = new EvaluatorContext(dataJObj, dataJPath, resourceJObj, resourceJPath, Direction.ContextToResource);

            Assert.False(evaluator.Evaluate(context));
        }

        [Theory]
        [MemberData(nameof(positiveCases))]
        public void ContainsEvaluatorPositiveBehavior(
            JObject dataJObj,
            string dataJPath,
            JObject resourceJObj,
            string resourceJPath)
        {
            var evaluator = new ContainsEvaluator();
            var context = new EvaluatorContext(dataJObj, dataJPath, resourceJObj, resourceJPath, Direction.ContextToResource);
            Assert.True(evaluator.Evaluate(context));
        }
     }
}