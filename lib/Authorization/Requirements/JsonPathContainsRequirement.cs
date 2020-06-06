namespace AuthZyin.Authorization
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// JsonPathContainsRequirement which checks whether context or resource contains info from the each other
    /// </summary>
    /// <typeparam name="TContextCustomData">Type of custom data in AuthZyinContext</typeparam>
    /// <typeparam name="TResource">Type of Resource</typeparam>
    public sealed class JsonPathContainsRequirement<TContextCustomData, TResource> : JsonPathBaseRequirement<TContextCustomData, TResource>
        where TContextCustomData: class
        where TResource: AuthZyinResource
    {
        /// <summary>
        /// RequirementType used by client lib
        /// </summary>
        public sealed override string RequirementType => "Contains";

        /// <summary>
        /// Initializes a new instance of JsonPathContainsRequirement which uses Json Path to check requirement satisfaction
        /// </summary>
        /// <param name="contextPath">jsonPath to context object</param>
        /// <param name="resourcePath">jsonPath to resource object</param>
        /// <param name="direction">contains direction</param>
        public JsonPathContainsRequirement(string contextPath, string resourcePath, Direction direction) : base (contextPath, resourcePath, direction) {}

        /// <summary>
        /// Evaluate two JObjects based on the JsonPaths configured with the intended operation
        /// </summary>
        /// <param name="contextJObject">JObject representing context</param>
        /// <param name="resourceJObject">JObject representing resource</param>
        /// <returns>true if requirement is satisfied</returns>
        protected sealed override bool EvaluateFromJObjects(JObject contextJObject, JObject resourceJObject)
        {
            // Select the left and right operands based on direction.
            // As this is a contains peration, we expect left to be an IEnumerable (SelectTokens) and right to be a single token (SelectToken).
            IEnumerable<JToken> collection;
            JToken element;

            if (this.Direction == Direction.ContextToResource)
            {
                collection = contextJObject.SelectTokens(this.ContextJPath);
                element = resourceJObject.SelectToken(this.ResourceJPath);
            }
            else
            {
                collection = resourceJObject.SelectTokens(this.ResourceJPath);
                element = contextJObject.SelectToken(this.ContextJPath);
            }

            // Use Linq Any operation with JToken.DeepEquals to find matches
            return collection.Any(t => JToken.DeepEquals(t, element));
        }
    }
}
