namespace test
{
    using System;
    using Xunit;
    using AuthZyin.Authorization.Requirements;
    using AuthZyin.Authorization.JPathRequirements;
    using AuthZyin.Authorization;
    using System.Collections.Generic;

    public class ConcreteRequirementsTest
    {
        private static readonly TestCustomData dummyData = new TestCustomData();
        private static readonly TestResource MyResource = new TestResource();

        public static readonly IEnumerable<object[]> ConcreteRequirementsTestCases = new List<object[]>
        {
            new object[] { new EqualsValueRequirement<TestCustomData, int>(dummyData.JPathIntValue, dummyData.IntValue), true },
            new object[] { new EqualsValueRequirement<TestCustomData, int>(dummyData.JPathIntValue, dummyData.IntValue - 1), false },
            new object[] { new EqualsValueRequirement<TestCustomData, string>(dummyData.JPathStringValue, dummyData.StringValue), true },
            new object[] { new EqualsValueRequirement<TestCustomData, DateTime>(dummyData.JPathDateValue, dummyData.DateValue), true },
            new object[] { new GreaterThanValueRequirement<TestCustomData, int>(dummyData.JPathIntValue, dummyData.IntValue - 1), true },
            new object[] { new GreaterThanValueRequirement<TestCustomData, string>(dummyData.JPathStringValue, string.Empty), true },
            new object[] { new ContainsValueRequirement<TestCustomData, string>(dummyData.JPathArrayValue + "[*]", dummyData.StringValue), true },
            new object[] { new ContainsValueRequirement<TestCustomData, string>(dummyData.JPathArrayValue + "[*]", "random non existing string value"), false },
            new object[] { new EqualsRequirement<TestCustomData, TestResource>(dummyData.JPathIntValue, MyResource.JPathIntValue), true },
            new object[] { new EqualsRequirement<TestCustomData, TestResource>(dummyData.JPathDateValue, MyResource.JPathNestedDateValue), true },
            new object[] { new EqualsRequirement<TestCustomData, TestResource>(dummyData.JPathStringValue, MyResource.JPathNestedDateValue), false },
            new object[] { new GreaterThanRequirement<TestCustomData, TestResource>(dummyData.JPathArrayValue + "[0]", MyResource.JPathIntValue, Direction.ContextToResource), false },
            new object[] { new GreaterThanRequirement<TestCustomData, TestResource>(dummyData.JPathIntValue, MyResource.JPathNestedDataSmallerIntValue, Direction.ContextToResource), true },
            new object[] { new GreaterThanRequirement<TestCustomData, TestResource>(dummyData.JPathIntValue, MyResource.JPathNestedDataBiggerIntValue, Direction.ResourceToContext), true },            
            new object[] { new GreaterThanRequirement<TestCustomData, TestResource>(dummyData.JPathIntValue, MyResource.JPathNestedDataBiggerIntValue, Direction.ContextToResource), false },
            new object[] { new GreaterThanRequirement<TestCustomData, TestResource>(dummyData.JPathIntValue, MyResource.JPathNestedDataBiggerIntValue, Direction.ResourceToContext), true },            
            new object[] { new GreaterThanOrEqualToRequirement<TestCustomData, TestResource>(dummyData.JPathArrayValue + "[0]", MyResource.JPathNestedDataStringValue, Direction.ContextToResource), true },
            new object[] { new GreaterThanOrEqualToRequirement<TestCustomData, TestResource>(dummyData.JPathArrayValue + "[1]", MyResource.JPathNestedDataStringValue, Direction.ContextToResource), true },
            new object[] { new GreaterThanOrEqualToRequirement<TestCustomData, TestResource>(dummyData.JPathArrayValue + "[2]", MyResource.JPathNestedDataStringValue, Direction.ContextToResource), false },
            new object[] { new GreaterThanOrEqualToRequirement<TestCustomData, TestResource>(dummyData.JPathIntValue, MyResource.JPathIntValue, Direction.ContextToResource), true },
            new object[] { new GreaterThanOrEqualToRequirement<TestCustomData, TestResource>(dummyData.JPathIntValue, MyResource.JPathNestedDataBiggerIntValue, Direction.ResourceToContext), true },
            new object[] { new GreaterThanOrEqualToRequirement<TestCustomData, TestResource>(dummyData.JPathIntValue, MyResource.JPathNestedDataBiggerIntValue, Direction.ContextToResource), false },
            new object[] { new ContainsRequirement<TestCustomData, TestResource>(dummyData.JPathArrayValue + "[*]", MyResource.JPathNestedDataStringValue, Direction.ContextToResource), true },
            new object[] { new ContainsRequirement<TestCustomData, TestResource>(dummyData.JPathStringValue, MyResource.JPathNestedDataStringArrayValue + "[*]", Direction.ResourceToContext), true },
            new object[] { new ContainsRequirement<TestCustomData, TestResource>(dummyData.JPathArrayValue + "[*]", MyResource.JPathNestedDataBiggerIntValue, Direction.ContextToResource), false },
        };

        [Theory]
        [MemberData(nameof(ConcreteRequirementsTestCases))]
        public void TestConcreteRequirements(Requirement requirement, bool expectedResult)
        {
            var context = new TestContext();
            Assert.Equal(expectedResult, requirement.Evaluate(context, MyResource));
        }
    }
}