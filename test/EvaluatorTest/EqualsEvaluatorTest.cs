namespace test
{
    using Xunit;
    using AuthZyin.Authorization.Requirements;
    using System;
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;

    public class EqualsEvaluatorTest
    {
        private static readonly TestCustomData data = new TestCustomData();
        private static readonly TestResource resource = new TestResource();
        private static readonly JObject dataJObj = JObject.FromObject(data);
        private static readonly JObject resourceObj = JObject.FromObject(resource);

        public static readonly IEnumerable<object[]> positiveCases = new List<object[]>
        {
            new object[] { dataJObj, data.JPathIntValue, resourceObj, resource.JPathNestedIntValue },
            new object[] { dataJObj, data.JPathStringValue, resourceObj, resource.JPathNestedDataStringValue },
            new object[] { dataJObj, data.JPathGuidValue, resourceObj, resource.JPathNestedGuidValue },
            new object[] { dataJObj, data.JPathDateValue, resourceObj, resource.JPathNestedDateValue },
            new object[] { dataJObj, data.JPathArrayValue + "[1]", resourceObj, resource.JPathNestedDataStringArrayValue + "[1]" },
        };

        [Fact]
        public void ConstructorThrowsOnInvalidArg()
        {
            Assert.Throws<ArgumentNullException>(() => new EqualsEvaluator().Evaluate(null));
        }

        [Theory]
        [MemberData(nameof(positiveCases))]
        public void EqualsEvaluatorPositiveBehavior(
            JObject dataJObj,
            string dataJPath,
            JObject resourceJObj,
            string resourceJPath)
        {
            var evaluator = new EqualsEvaluator();
            var context = new EvaluatorContext(dataJObj, dataJPath, resourceJObj, resourceJPath, Direction.ContextToResource);
            Assert.True(evaluator.Evaluate(context));
        }
    }
}
