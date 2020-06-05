namespace AuthZyin.Authorization
{
    using System;
    using Newtonsoft.Json.Linq;

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
    public abstract class JsonPathBaseRequirement<TContextCustomData, TResource> : AbstractRequirement<TContextCustomData, TResource>
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
        public JsonPathBaseRequirement(string contextPath, string resourcePath, Direction direction)
        {
            this.ContextJPath = contextPath ?? throw new ArgumentNullException(nameof(contextPath));
            this.ResourceJPath = resourcePath ?? throw new ArgumentNullException(nameof(resourcePath));
            this.Direction = direction;
        }

        /// <summary>
        // Evaluate current requirement against given user and typed resource.
        /// </summary>
        /// <param name="context">authorization data context</param>
        /// <param name="typedResource">resource object</param>
        /// <returns>true if allowed</returns>
        protected sealed override bool Evaluate(AuthZyinContext<TContextCustomData> context, TResource resource)
        {
            var contextJObject = JObject.FromObject(context);
            var resourceJObj = JObject.FromObject(context);

            if (contextJObject == null || resourceJObj == null)
            {
                return false;
            }

            return this.EvaluateFromJObjects(contextJObject, resourceJObj);
        }

        /// <summary>
        /// Evaluate two JObjects based on the JsonPaths configured with the intended operation
        /// </summary>
        /// <param name="contextJObject">JObject representing context</param>
        /// <param name="resourceJObj">JObject representing resource</param>
        /// <returns>true if requirement is satisfied</returns>
        protected abstract bool EvaluateFromJObjects(JObject contextJObject, JObject resourceJObj);
    }
}
