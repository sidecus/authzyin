namespace test
{
    using System;
    using Newtonsoft.Json.Linq;
    using Xunit;
    using AuthZyin.Authorization.Requirements;
    using System.Collections.Generic;

    public class DummyValusEvaluator : ValuesEvaluator
    {
        protected override bool EvaluateValues(JValue leftValue, JValue rightValue)
        {
            throw new NotImplementedException();
        }
    }

    public class ValuesEvaluatorTest
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

            // Left operand is not an value
            new object[] { dataJObj, data.JPathArrayValue, resourceObj, resource.JPathNestedDataStringValue },

            // right operand is not a value
            new object[] { dataJObj, data.JPathStringValue, resourceObj, resource.JPathNestedDataStringArrayValue },

            // Different token type
            new object[] { dataJObj, data.JPathIntValue, resourceObj, resource.JPathNestedDateValue },
        };

        [Fact]
        public void ConstructorThrowsOnInvalidArg()
        {
            Assert.Throws<ArgumentNullException>(() => new DummyValusEvaluator().Evaluate(null));
        }

        // Helper method to test JValue based evaluator - equals, greater than etc.
        [Theory]
        [MemberData(nameof(negativeCases))]
        public static void TestJValueCommonInvalidScenarios(
            JObject dataJObj,
            string dataJPath,
            JObject resourceJObj,
            string resourceJPath)
        {
            var context = new EvaluatorContext(dataJObj, dataJPath, resourceJObj, resourceJPath, Direction.ContextToResource);
            Assert.False(new DummyValusEvaluator().Evaluate(context));
        }
   }
}