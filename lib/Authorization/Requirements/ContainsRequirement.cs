namespace AuthZyin.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// ContainsRequirement which checks whether context or resource contains info from the each other
    /// </summary>
    /// <typeparam name="TContextCustomData">Type of custom data in AuthZyinContext</typeparam>
    /// <typeparam name="TResource">Type of Resource</typeparam>
    public class ContainsRequirement<TContextCustomData, TResource> : JsonPathRequirement<TContextCustomData, TResource>
        where TContextCustomData: class
        where TResource: AuthZyinResource
    {
        /// <summary>
        /// RequirementType used by client lib
        /// </summary>
        public override string RequirementType => "Contains";

        /// <summary>
        /// Initializes a new instance of ContainsRequirement which uses Json Path to check requirement satisfaction
        /// </summary>
        /// <param name="contextPath">jsonPath to context object</param>
        /// <param name="resourcePath">jsonPath to resource object</param>
        /// <param name="direction">contains direction</param>
        public ContainsRequirement(string contextPath, string resourcePath, Direction direction) : base (contextPath, resourcePath, direction)
        {
        }

        /// <summary>
        // Evaluate current requirement against given user and typed resource.
        /// </summary>
        /// <param name="context">authorization data context</param>
        /// <param name="typedResource">resource object</param>
        /// <returns>true if allowed</returns>
        protected sealed override bool EvaluateWithTypeResource(AuthZyinContext<TContextCustomData> context, TResource resource)
        {
            var contextJObject = JObject.FromObject(context);
            var resourceJObj = JObject.FromObject(context);

            IEnumerable<JToken> collection;
            JToken element;

            if (this.Direction == Direction.ContextToResource)
            {
                collection = contextJObject.SelectTokens(this.ContextJPath);
                element = resourceJObj.SelectToken(this.ResourceJPath);
            }
            else
            {
                collection = resourceJObj.SelectTokens(this.ResourceJPath);
                element = contextJObject.SelectToken(this.ContextJPath);
            }

            // Use Linq Any operation with JToken.DeepEquals to find matches
            return collection.Any(t => JToken.DeepEquals(t, element));
        }
    }
}
