namespace test
{
    using System;
    using Xunit;
    using AuthZyin.Authorization.Requirements;
    using AuthZyin.Authorization;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Authorization;
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
            new object[] { RequirementOperatorType.Equals, intConst },
            new object[] { RequirementOperatorType.GreaterThan, intConst },
            new object[] { RequirementOperatorType.Contains, intConst },
            new object[] { RequirementOperatorType.Equals, stringConst },
            new object[] { RequirementOperatorType.GreaterThan, stringConst },
            new object[] { RequirementOperatorType.Contains, stringConst },
            new object[] { RequirementOperatorType.Equals, dateConst },
            new object[] { RequirementOperatorType.GreaterThan, dateConst },
            new object[] { RequirementOperatorType.Contains, dateConst },
        };

        public static readonly IEnumerable<object[]> ConstRequirementEvaluateBehaviors = new List<object[]>
        {
            new object[] { RequirementOperatorType.Equals, data.JPathIntValue, data.IntValue, true },
            new object[] { RequirementOperatorType.Equals, data.JPathIntValue, data.IntValue - 1, false },
            new object[] { RequirementOperatorType.GreaterThan, data.JPathIntValue, data.IntValue - 1, true },
            new object[] { RequirementOperatorType.Equals, data.JPathStringValue, data.StringValue, true },
            new object[] { RequirementOperatorType.Equals, data.JPathStringValue, null, false },
            new object[] { RequirementOperatorType.GreaterThan, data.JPathStringValue, string.Empty, true },
            new object[] { RequirementOperatorType.Contains, data.JPathArrayValue + "[*]", data.StringValue, true },
            new object[] { RequirementOperatorType.Contains, data.JPathArrayValue + "[*]", "random non existing string value", false },
        };

        [Fact]
        public void ConsructorThrowsOnInvalidArg()
        {
            Assert.Throws<ArgumentNullException>(() => new JsonPathConstantRequirement<TestCustomData, int>(
                RequirementOperatorType.GreaterThan,
                null,
                3));

            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonPathConstantRequirement<TestCustomData, int>(
                RequirementOperatorType.Invalid,
                "",
                3));

            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonPathConstantRequirement<TestCustomData, int>(
                RequirementOperatorType.RequiresRole,
                string.Empty,
                3));

            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonPathConstantRequirement<TestCustomData, int>(
                RequirementOperatorType.Or,
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
        public void SetsConstResourceAndPathAndDirectionCorrectly(RequirementOperatorType operatorType, object constValue)
        {
            var contextPath = "$.IntMember$#$%#FD";

            var genericType = typeof(JsonPathConstantRequirement<,>);
            var constRequirementType = genericType.MakeGenericType(typeof(TestCustomData), constValue.GetType());
            var requirement = Activator.CreateInstance(constRequirementType, operatorType, contextPath, constValue);

            Assert.Equal(contextPath, this.GetPropertyValue(constRequirementType, "DataJPath", requirement));
            Assert.Equal(ConstantWrapperResource<int>.ValueJsonPath, this.GetPropertyValue(constRequirementType, "ResourceJPath", requirement));
            Assert.Equal(operatorType, this.GetPropertyValue(constRequirementType, "Operator", requirement));
            Assert.Equal(Direction.ContextToResource, this.GetPropertyValue(constRequirementType, "Direction", requirement));
            Assert.Equal(constValue, this.GetPropertyValue(constRequirementType, "ConstValue", requirement));
        }

        [Theory]
        [MemberData(nameof(ConstRequirementEvaluateBehaviors))]
        public void ValidateConstRequirementBehavior(
            RequirementOperatorType operatorType,
            string dataJPath,
            object constValue,
            bool expectedReseult)
        {
            var authZyinContext = TestContext.CreateDefaultTestContext();

            var genericType = typeof(JsonPathConstantRequirement<,>);
            var constType = constValue != null ? constValue.GetType() : typeof(string);             //special case for null value and defaul the type to string
            var constRequirementType = genericType.MakeGenericType(typeof(TestCustomData), constType);
            var requirement = Activator.CreateInstance(constRequirementType, operatorType, dataJPath, constValue);

            var evaluateMethod = this.GetEvaluateMethod(constRequirementType);
            Assert.Equal(expectedReseult, (bool)evaluateMethod.Invoke(requirement, new object[] { authZyinContext, null as Resource }));
            Assert.Equal(expectedReseult, (bool)evaluateMethod.Invoke(requirement, new object[] { authZyinContext, new TestResourceInvalid() }));
        }

        [Fact]
        public void ConstantWrapperResourceValueJsonPath()
        {
            // Ensure the wrapper resource use a member named "Value" to save the constant right operand.
            // It's needed since the client is betting it to be harded coded to "$.Value".
            Assert.Equal("$.Value", ConstantWrapperResource<int>.ValueJsonPath);
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