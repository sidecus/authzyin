namespace AuthZyin.Authorization
{
    using System;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Authorization resource base class
    /// </summary>
    public abstract class Resource
    {
        /// <summary>
        /// Lazy version of the JObject prepresentation of the current resource object
        /// </summary>
        protected Lazy<JObject> lazyJObject;

        /// <summary>
        /// Initializes a new instance of the Resource class.
        /// </summary>
        public Resource()
        {
            this.lazyJObject = new Lazy<JObject>(() => JObject.FromObject(this));
        }

        /// <summary>
        /// Get the current resource as a JObject
        /// </summary>
        /// <returns>JObject version of the current resource</returns>
        public JObject GetResourceAsJObject()
        {
            return this.lazyJObject.Value;
        }
    }

    /// <summary>
    /// Resource type to handle constant processing in requirement instead of real resource
    /// </summary>
    /// <typeparam name="T">constant type</typeparam>
    public sealed class ValueWrapperResource<T> : Resource
    {
        /// <summary>
        /// JsonPath to the value member
        /// </summary>
        /// <returns>Json path to get the value</returns>
        public static readonly string ValueJsonPath = $"$.{nameof(Value)}";

        /// <summary>
        /// constant value. The member name must match <see cref="ConstResourceValueJPath"/> defined in <see cref="JsonPathConstantRequirement."/>
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ConstantWrapperResource" /> the class
        /// </summary>
        public ValueWrapperResource(T value)
        {
            this.Value = value;
        }
    }
}