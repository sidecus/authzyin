namespace AuthZyin.Authorization
{
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// JsonPathEqualsRequirement which checks whether properties from context and resource are equal
    /// </summary>
    /// <typeparam name="TContextCustomData">Type of custom data in AuthZyinContext</typeparam>
    /// <typeparam name="TResource">Type of Resource</typeparam>
    public class JsonPathEqualsRequirement<TContextCustomData, TResource> : JsonPathBaseRequirement<TContextCustomData, TResource>
        where TContextCustomData: class
        where TResource: AuthZyinResource
    {
        /// <summary>
        /// RequirementType used by client lib
        /// </summary>
        public sealed override string Type => "Equals";

        /// <summary>
        /// Initializes a new instance of ContainsRequirement which uses Json Path to check requirement satisfaction
        /// </summary>
        /// <param name="contextPath">jsonPath to context object</param>
        /// <param name="resourcePath">jsonPath to resource object</param>
        public JsonPathEqualsRequirement(string contextPath, string resourcePath) : base (contextPath, resourcePath, Direction.ContextToResource) {}

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

            var left = contextJObject.SelectToken(this.ContextJPath);
            var right = resourceJObj.SelectToken(this.ResourceJPath);

            if (left == null || right == null)
            {
                return false;
            }

            return JToken.DeepEquals(left, right);
        }
    }
}
