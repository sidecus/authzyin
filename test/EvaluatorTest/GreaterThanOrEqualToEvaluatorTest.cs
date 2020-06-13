namespace test
{
    using System;
    using Xunit;
    using AuthZyin.Authorization.Requirements;
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;

    public class GreaterThanOrEqualToEvaluatorTest
    {
        private static readonly TestCustomData data = new TestCustomData();
        private static readonly TestResource resource = new TestResource();
        private static readonly JObject dataJObj = JObject.FromObject(data);
        private static readonly TestCustomData largerData = new TestCustomData()
        {
            DateValue = resource.NestedData.DateValue + new TimeSpan(1, 0, 0),
        };
        private static readonly JObject largerDataJObj = JObject.FromObject(largerData);

        private static readonly JObject resourceObj = JObject.FromObject(resource);

        public static readonly IEnumerable<object[]> negativeCases = new List<object[]>
        {
            new object[] { dataJObj, data.JPathIntValue, resourceObj, resource.JPathNestedDataBiggerIntValue },
            new object[] { dataJObj, data.JPathStringValue, resourceObj, resource.JPathNestedDataStringArrayValue + "[0]" },
            new object[] { resourceObj, resource.JPathNestedDateValue, largerDataJObj, largerData.JPathDateValue },
        };

        public static readonly IEnumerable<object[]> positiveCases = new List<object[]>
        {
            new object[] { dataJObj, data.JPathIntValue, resourceObj, resource.JPathNestedDataSmallerIntValue },
            new object[] { dataJObj, data.JPathIntValue, resourceObj, resource.JPathNestedIntValue },
            new object[] { dataJObj, data.JPathStringValue, resourceObj, resource.JPathNestedDataStringArrayValue + "[-1:]" },
            new object[] { dataJObj, data.JPathStringValue, resourceObj, resource.JPathNestedDataStringArrayValue + "[1]" },
            new object[] { largerDataJObj, largerData.JPathDateValue, resourceObj, resource.JPathNestedDateValue },
            new object[] { dataJObj, data.JPathDateValue, resourceObj, resource.JPathNestedDateValue },
        };

        [Fact]
        public void ConstructorThrowsOnInvalidArg()
        {
            // context is null
            Assert.Throws<ArgumentNullException>(() => new GreaterThanOrEqualToEvaluator().Evaluate(null));
        }

        [Theory]
        [MemberData(nameof(negativeCases))]
        public void GreaterThanOrEqualToEvaluatorNegativeBehavior(
            JObject dataJObj,
            string dataJPath,
            JObject resourceJObj,
            string resourceJPath)
        {
            var evaluator = new GreaterThanOrEqualToEvaluator();
            var context = new EvaluatorContext(dataJObj, dataJPath, resourceJObj, resourceJPath, Direction.ContextToResource);
            Assert.False(evaluator.Evaluate(context));
        }

        [Theory]
        [MemberData(nameof(positiveCases))]
        public void GreaterThanOrEqualToEvaluatorPositiveBehavior(
            JObject dataJObj,
            string dataJPath,
            JObject resourceJObj,
            string resourceJPath)
        {
            var evaluator = new GreaterThanOrEqualToEvaluator();
            var context = new EvaluatorContext(dataJObj, dataJPath, resourceJObj, resourceJPath, Direction.ContextToResource);
            Assert.True(evaluator.Evaluate(context));
       }
    }
}