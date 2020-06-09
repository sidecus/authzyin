namespace AuthZyin.Authorization
{
    /// <summary>
    /// Authorization resource base class
    /// </summary>
    public abstract class Resource
    {
    }

    /// <summary>
    /// Resource type to handle constant processing in requirement instead of real resource
    /// </summary>
    /// <typeparam name="T">constant type</typeparam>
    public sealed class ConstantWrapperResource<T> : Resource
    {
        /// <summary>
        /// constant value. The member name must match <see cref="ConstResourceValueJPath"/> defined in <see cref="JsonPathConstantRequirement."/>
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ConstantWrapperResource" /> the class
        /// </summary>
        public ConstantWrapperResource(T value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Return the name of the constant Value member. Used by 
        /// </summary>
        public static string GetValueMemberJPath()
        {
            return $"$.{nameof(Value)}";
        }
    }
}