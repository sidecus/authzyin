namespace AuthZyin.Authorization
{
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// JsonPathEqualsRequirement which checks whether properties from context and resource are equal
    /// </summary>
    /// <typeparam name="TContextCustomData">Type of custom data in AuthZyinContext</typeparam>
    /// <typeparam name="TResource">Type of Resource</typeparam>
    public sealed class JsonPathEqualsRequirement<TContextCustomData, TResource> : JsonPathBaseRequirement<TContextCustomData, TResource>
        where TContextCustomData: class
        where TResource: AuthZyinResource
    {
        /// <summary>
        /// RequirementType used by client lib
        /// </summary>
        public sealed override string RequirementType => "Equals";

        /// <summary>
        /// Initializes a new instance of ContainsRequirement which uses Json Path to check requirement satisfaction
        /// </summary>
        /// <param name="contextPath">jsonPath to context object</param>
        /// <param name="resourcePath">jsonPath to resource object</param>
        public JsonPathEqualsRequirement(string contextPath, string resourcePath) : base (contextPath, resourcePath, Direction.ContextToResource) {}

        /// <summary>
        /// Evaluate two JObjects based on the JsonPaths configured with the intended operation
        /// </summary>
        /// <param name="contextJObject">JObject representing context</param>
        /// <param name="resourceJObject">JObject representing resource</param>
        /// <returns>true if requirement is satisfied</returns>
        protected sealed override bool EvaluateFromJObjects(JObject contextJObject, JObject resourceJObject)
        {
            var left = contextJObject.SelectToken(this.ContextJPath);
            var right = resourceJObject.SelectToken(this.ResourceJPath);

            if (left == null || right == null)
            {
                return false;
            }

            return JToken.DeepEquals(left, right);
        }
    }
}
