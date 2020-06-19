namespace test
{
    using System;
    using Xunit;
    using AuthZyin.Authorization.Requirements;
    using AuthZyin.Authorization.JPathRequirements;
    using AuthZyin.Authorization;
    using System.Collections.Generic;
    using System.Reflection;

    public class JsonPathConstantRequirementTest
    {
        private static readonly TestCustomData data = new TestCustomData();
        private static readonly int intConst = 2342667;
        private static readonly Guid guidConst = Guid.NewGuid();
        private static readonly string stringConst = $"this is a string for {nameof(SetsConstResourceAndPathAndDirectionCorrectly)}";
        private static readonly DateTime dateConst = DateTime.UtcNow;
        private static readonly object objectConst = new object();

        public static readonly IEnumerable<object[]> ConstTypeNegativeCase = new List<object[]>
        {
            new object[] { guidConst },
            new object[] { objectConst },
        };

        public static readonly IEnumerable<object[]> ConstTypePositiveCase = new List<object[]>
        {
            new object[] { OperatorType.Equals, intConst },
            new object[] { OperatorType.GreaterThan, intConst },
            new object[] { OperatorType.Contains, intConst },
            new object[] { OperatorType.Equals, stringConst },
            new object[] { OperatorType.GreaterThan, stringConst },
            new object[] { OperatorType.Contains, stringConst },
            new object[] { OperatorType.Equals, dateConst },
            new object[] { OperatorType.GreaterThan, dateConst },
            new object[] { OperatorType.Contains, dateConst },
        };

        public static readonly IEnumerable<object[]> ConstRequirementEvaluateBehaviors = new List<object[]>
        {
            new object[] { OperatorType.Equals, data.JPathIntValue, data.IntValue, true },
            new object[] { OperatorType.Equals, data.JPathIntValue, data.IntValue - 1, false },
            new object[] { OperatorType.GreaterThan, data.JPathIntValue, data.IntValue - 1, true },
            new object[] { OperatorType.Equals, data.JPathStringValue, data.StringValue, true },
            new object[] { OperatorType.Equals, data.JPathStringValue, null, false },
            new object[] { OperatorType.GreaterThan, data.JPathStringValue, string.Empty, true },
            new object[] { OperatorType.Contains, data.JPathArrayValue + "[*]", data.StringValue, true },
            new object[] { OperatorType.Contains, data.JPathArrayValue + "[*]", "random non existing string value", false },
        };

        [Fact]
        public void ConsructorThrowsOnInvalidArg()
        {
            Assert.Throws<ArgumentNullException>(() => new JsonPathConstantRequirement<TestCustomData, int>(
                OperatorType.GreaterThan,
                null,
                3));

            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonPathConstantRequirement<TestCustomData, int>(
                OperatorType.Invalid,
                "",
                3));

            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonPathConstantRequirement<TestCustomData, int>(
                OperatorType.RequiresRole,
                string.Empty,
                3));

            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonPathConstantRequirement<TestCustomData, int>(
                OperatorType.Or,
                "$",
                3));
        }

        [Theory]
        [MemberData(nameof(ConstTypeNegativeCase))]
        public void TypeParamDoesntAllowNonIConvertible(object constValue)
        {
            var genericType = typeof(JsonPathConstantRequirement<,>);
            var constType = constValue.GetType();
            Assert.Throws<ArgumentException>(() => genericType.MakeGenericType(typeof(TestCustomData), constType));
        }

        [Theory]
        [MemberData(nameof(ConstTypePositiveCase))]
        public void SetsConstResourceAndPathAndDirectionCorrectly(OperatorType operatorType, object constValue)
        {
            var contextPath = "$.IntMember$#$%#FD";

            var genericType = typeof(JsonPathConstantRequirement<,>);
            var constRequirementType = genericType.MakeGenericType(typeof(TestCustomData), constValue.GetType());
            var requirement = Activator.CreateInstance(constRequirementType, operatorType, contextPath, constValue);

            Assert.Equal(contextPath, this.GetPropertyValue(constRequirementType, "DataJPath", requirement));
            Assert.Equal(ValueWrapperResource<int>.ValueJsonPath, this.GetPropertyValue(constRequirementType, "ResourceJPath", requirement));
            Assert.Equal(operatorType, this.GetPropertyValue(constRequirementType, "Operator", requirement));
            Assert.Equal(Direction.ContextToResource, this.GetPropertyValue(constRequirementType, "Direction", requirement));
            Assert.Equal(constValue, this.GetPropertyValue(constRequirementType, "ConstValue", requirement));
        }

        [Theory]
        [MemberData(nameof(ConstRequirementEvaluateBehaviors))]
        public void ValidateConstRequirementBehavior(
            OperatorType operatorType,
            string dataJPath,
            object constValue,
            bool expectedReseult)
        {
            var authZyinContext = new TestContext();

            var genericType = typeof(JsonPathConstantRequirement<,>);
            var constType = constValue != null ? constValue.GetType() : typeof(string);             //special case for null value and defaul the type to string
            var constRequirementType = genericType.MakeGenericType(typeof(TestCustomData), constType);

            if (constValue == null)
            {
                // cosnt value can never be null. The constructor will throw but exception will be converted to TargetInvocationException by Reflection.
                Assert.Throws<TargetInvocationException>(() => Activator.CreateInstance(constRequirementType, operatorType, dataJPath, constValue));
            }
            else
            {
                // Create the requirement based on the parameter and validate w/ or w/o resource
                var requirement = Activator.CreateInstance(constRequirementType, operatorType, dataJPath, constValue);
                var evaluateMethod = this.GetEvaluateMethod(constRequirementType);
                Assert.Equal(expectedReseult, (bool)evaluateMethod.Invoke(requirement, new object[] { authZyinContext, null as Resource }));
                Assert.Equal(expectedReseult, (bool)evaluateMethod.Invoke(requirement, new object[] { authZyinContext, new TestResourceInvalid() }));
            }
        }

        private object GetPropertyValue(Type type, string memberName, object target)
        {
            var propertyInfo = type.GetMember(memberName)[0] as PropertyInfo;
            return propertyInfo.GetValue(target);
        }

        private MethodInfo GetEvaluateMethod(Type type)
        {
            return type.GetMethod("Evaluate");
        }
   }
}