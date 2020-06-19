namespace test
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using AuthZyin.Authorization;
    using Microsoft.AspNetCore.Authorization;
    using Xunit;
    using Moq;
    using Moq.Protected;

    public class AuthZyinContextTest
    {
        [Fact]
        public void ConstructorThrowsOnInvalidArg()
        {
            var policies = new List<(string name, AuthorizationPolicy policy)>{ ("nullpolicy", null), };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            
            Assert.Throws<ArgumentNullException>(() => new AuthZyinContext<TestCustomData>(null, claimsPrincipal));
            Assert.Throws<ArgumentNullException>(() => new AuthZyinContext<TestCustomData>(policies, null));
        }

        [Fact]
        public void UserContextAndPoliciesAreConstructedCorrectly()
        {
            // Use TestContext proxy as the concrete type
            var context = new TestContext() as AuthZyinContext<TestCustomData>;

            // Make sure Test context is inherited from AuthZyinContext
            Assert.NotNull(context);
                
            Assert.Same(context.Policies, TestContext.DefaultPolicies);
            Assert.NotNull(context.UserContext);
            Assert.Same(context.UserContext.UserId, TestContext.DefaultUserId);
            Assert.Same(context.UserContext.UserName, TestContext.DefaultUserName);

            // Make sure data is constructed and returned from both the getter and the GetData method
            var data = context.Data;
            Assert.NotNull(data);
            Assert.True(data is TestCustomData);
            Assert.Same(data, context.GetData());

            // Make sure the result JObject data is correct
            var dataJObj = context.GetDataAsJObject();
            var dataConverted = dataJObj.ToObject<TestCustomData>();
            Assert.NotSame(data, dataConverted);
            Assert.Equal(data.IntValue, dataConverted.IntValue);
            Assert.Equal(data.StringValue, dataConverted.StringValue);
            Assert.Equal(data.BiggerIntValue, dataConverted.BiggerIntValue);
            Assert.Equal(data.ArrayValue[0], dataConverted.ArrayValue[0]);
            Assert.Equal(data.DateValue, dataConverted.DateValue);
        }

        [Fact]
        public void VerifyLazyMembersAreLazy()
        {
            // Mock a inherited class instance of AuthZyinContext<TestCustomData>
            var mockContext = new Mock<AuthZyinContext<TestCustomData>>(TestContext.DefaultPolicies, TestContext.DefaultClaimsPrincipal)
            {
                CallBase = true,
            };

            var data = new TestCustomData();
            mockContext.Protected().Setup<TestCustomData>("CreateData").Returns(data);
            var context = mockContext.Object;

            // Accessing data multiple times only triggers the CreateData method once
            var data1 = context.Data;
            mockContext.Protected().Verify("CreateData", Times.Once());
            var data2 = context.Data;
            mockContext.Protected().Verify("CreateData", Times.Once());
            Assert.Same(data1, data2);
            var data3 = context.GetData();
            mockContext.Protected().Verify("CreateData", Times.Once());
            Assert.Same(data1, data3);

            // Accessing GetDataAsJObject doesn't cause multiple JObject constructs
            var jObj1 = context.GetDataAsJObject();
            Assert.NotNull(jObj1);
            mockContext.Protected().Verify("CreateData", Times.Once());
            var jObj2 = context.GetDataAsJObject();
            Assert.Same(jObj1, jObj2);
        }
    }
}