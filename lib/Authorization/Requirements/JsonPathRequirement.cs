namespace AuthZyin.Authorization
{
    using System;

    /// <summary>
    /// Operation direction. For example, when direction == ContextToResource, the "Contains" operation means info extracted from context
    /// contains info extracted from resource. And vice versa.
    /// </summary>
    public enum Direction
    {
        ContextToResource = 1,
        ResourceToContext = 2,
    }

    /// <summary>
    /// Authorization requirement which uses JsonPath for filtering and comparison.
    /// Server evaluation is done via Newtonsoft.Json.
    /// Client evaluation is done via JsonPath plus - https://github.com/s3u/JSONPath;
    /// </summary>
    /// <typeparam name="TContextCustomData">Type of custom data in AuthZyinContext</typeparam>
    /// <typeparam name="TResource">Type of Resource</typeparam>
    public abstract class JsonPathRequirement<TContextCustomData, TResource> : AuthZyinRequirement<TContextCustomData, TResource>
        where TContextCustomData: class
        where TResource: AuthZyinResource
    {
        public string ContextJPath { get; }

        public string ResourceJPath { get; }

        public Direction Direction { get; }

        /// <summary>
        /// Initializes a new instance of JsonPathRequirement which uses Json Path to check requirement satisfaction
        /// </summary>
        /// <param name="contextPath">jsonPath to context object</param>
        /// <param name="resourcePath">jsonPath to resource object</param>
        /// <param name="direction">operation direction</param>
        public JsonPathRequirement(string contextPath, string resourcePath, Direction direction)
        {
            this.ContextJPath = contextPath ?? throw new ArgumentNullException(nameof(contextPath));
            this.ResourceJPath = resourcePath ?? throw new ArgumentNullException(nameof(resourcePath));
            this.Direction = direction;
        }
    }
}
