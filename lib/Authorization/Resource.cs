namespace AuthZyin.Authorization
{
    /// <summary>
    /// Authorization resource base class
    /// </summary>
    public abstract class Resource
    {
    }

    /// <summary>
    /// Dummy resource to handle constant processing in requirement instead of real resource
    /// </summary>
    /// <typeparam name="T">constant type</typeparam>
    public sealed class DummyResource<T> : Resource
    {
        /// <summary>
        /// constant value. The member name must match JsonPathConstantRequirement.DummyResourceValueJPath
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Initializes a new instance of DummyResource
        /// </summary>
        public DummyResource(T value)
        {
            this.Value = value;
        }
    }
}