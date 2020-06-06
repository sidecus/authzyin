namespace AuthZyin.Authorization
{
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// JsonPathEqualsRequirement which compares properties from context or resource based on the Direction.
    /// </summary>
    /// <typeparam name="TContextCustomData">Type of custom data in AuthZyinContext</typeparam>
    /// <typeparam name="TResource">Type of Resource</typeparam>
    public sealed class JsonPathGreaterThanRequirement<TContextCustomData, TResource> : JsonPathBaseRequirement<TContextCustomData, TResource>
        where TContextCustomData: class
        where TResource: AuthZyinResource
    {
        /// <summary>
        /// RequirementType used by client lib
        /// </summary>
        public sealed override string RequirementType => "GreaterThan";

        /// <summary>
        /// Initializes a new instance of JsonPathGreaterThanRequirement which uses Json Path to check requirement satisfaction
        /// </summary>
        /// <param name="contextPath">jsonPath to context object</param>
        /// <param name="resourcePath">jsonPath to resource object</param>
        /// <param name="direction">comparison direction</param>
        public JsonPathGreaterThanRequirement(string contextPath, string resourcePath, Direction direction) : base(contextPath, resourcePath, direction) {}

        /// <summary>
        /// Evaluate two JObjects based on the JsonPaths configured with the intended operation
        /// </summary>
        /// <param name="contextJObject">JObject representing context</param>
        /// <param name="resourceJObject">JObject representing resource</param>
        /// <returns>true if requirement is satisfied</returns>
        protected sealed override bool EvaluateFromJObjects(JObject contextJObject, JObject resourceJObject)
        {
            var contextToken = contextJObject.SelectToken(this.ContextJPath);
            var resourceToken = resourceJObject.SelectToken(this.ResourceJPath);

            var left = (this.Direction == Direction.ContextToResource) ? contextToken : resourceToken;
            var right = (this.Direction == Direction.ContextToResource) ? resourceToken : contextToken;

            if (left?.Type != JTokenType.Integer || right?.Type != JTokenType.Integer)
            {
                return false;
            }

            // We are using integer as the type here. This can be extended to other data types.
            return (int)left > (int)right;
        }
    }
}
