namespace  test
{
    using Xunit;
    using AuthZyin.Authorization;

    public class ResourceTest
    {
        [Fact]
        public void ResourceJObjectBehavior()
        {
            var resource = new ValueWrapperResource<int>(5);

            var jObj1 = resource.GetResourceAsJObject();
            var jObj2 = resource.GetResourceAsJObject();

            // Make sure we don't do JObject conversion twice
            Assert.Same(jObj1, jObj2);
            Assert.Equal(5, resource.Value);

            // Make sure data is correct
            var convertedBack = jObj1.ToObject<ValueWrapperResource<int>>();
            Assert.Equal(5, convertedBack.Value);
        }

        [Fact]
        public void ConstantWrapperResourceValueJsonPath()
        {
            // Ensure the wrapper resource use a member named "Value" to save the constant right operand.
            // It's needed since the client is betting it to be harded coded to "$.Value".
            Assert.Equal("$.Value", ValueWrapperResource<int>.ValueJsonPath);
        }
    }
}