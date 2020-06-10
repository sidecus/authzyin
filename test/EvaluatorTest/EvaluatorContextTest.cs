namespace test
{
    using System;
    using System.Linq;
    using Newtonsoft.Json.Linq;
    using Xunit;
    using AuthZyin.Authorization.Requirements;
    using System.Collections.Generic;

    public class EvaluatorContxtTest
    {
        private static readonly JObject dataObj = JObject.FromObject(new { IntValue = 3});
        private static readonly string dataJPath = "$.IntValue";
        private static readonly JObject resourceObj = JObject.FromObject(new { IntValue = 3});
        private static readonly string resourceJPath = "$.IntValue";

        public static readonly IEnumerable<object[]> InvalidConstructorArgs = new List<object[]>
        {
            new object[] { null, dataJPath, resourceObj, resourceJPath, Direction.ContextToResource },          // null data
            new object[] { dataObj, null, resourceObj, resourceJPath, Direction.ContextToResource },            // null data path
            new object[] { dataObj, "    \t", resourceObj, resourceJPath, Direction.ContextToResource },        // empty data path
            new object[] { dataObj, string.Empty, null, resourceJPath, Direction.ContextToResource },           // null resource
            new object[] { dataObj, string.Empty, resourceObj, null, Direction.ContextToResource },             // null resource path
            new object[] { dataObj, string.Empty, resourceObj, "  \t  ", Direction.ContextToResource },         // empty resource path
            new object[] { dataObj, string.Empty, resourceObj, resourceJPath, (Direction)100 },                 // invalid direction
        };

        [Theory]
        [MemberData(nameof(InvalidConstructorArgs))]
        public void ConstructorThrowsOnInvalidArg(
            JObject dataJObj,
            string dataJPath,
            JObject resourceJObj,
            string resourceJPath,
            Direction direction)
        {
            Assert.Throws<ArgumentNullException>(() => new EvaluatorContext(dataJObj, dataJPath, resourceJObj, resourceJPath, direction));
        }

        [Fact]
        public void InitializesLeftAndRightOperandsCorrectly()
        {
            var context = new EvaluatorContext(dataObj, dataJPath, resourceObj, resourceJPath, Direction.ContextToResource);
            Assert.Same(context.LeftJObject, dataObj);
            Assert.Same(context.LeftJPath, dataJPath);
            Assert.Same(context.RightJObject, resourceObj);
            Assert.Same(context.RightJPath, resourceJPath);

            context = new EvaluatorContext(dataObj, dataJPath, resourceObj, resourceJPath, Direction.ResourceToContext);
            Assert.Same(context.LeftJObject, resourceObj);
            Assert.Same(context.LeftJPath, resourceJPath);
            Assert.Same(context.RightJObject, dataObj);
            Assert.Same(context.RightJPath, dataJPath);
        }
    }
}