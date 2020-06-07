namespace AuthZyin.Authorization.Requirements
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Authorization requirement which uses JsonPath for filtering and comparison.
    /// Server evaluation is done via Newtonsoft.Json.
    /// Client evaluation is done via JsonPath plus - https://github.com/s3u/JSONPath;
    /// </summary>
    /// <typeparam name="TContextCustomData">Type of custom data in AuthZyinContext</typeparam>
    /// <typeparam name="TResource">Type of Resource</typeparam>
    public class JsonPathRequirement<TContextCustomData, TResource> : Requirement<TContextCustomData, TResource>
        where TContextCustomData: class
        where TResource: Resource
    {
        /// <summary>
        /// Evaluator map
        /// </summary>
        /// <typeparam name="RequirementOperatorType">operator type</typeparam>
        /// <typeparam name="RequirementEvaluator">evaluator</typeparam>
        /// <returns></returns>
        private static readonly Dictionary<RequirementOperatorType, RequirementEvaluator> EvaluatorMap = new Dictionary<RequirementOperatorType, RequirementEvaluator>()
        {
            { RequirementOperatorType.Equals,       new EqualsEvaluator() },
            { RequirementOperatorType.GreaterThan,  new GreaterThanEvaluator() },
            { RequirementOperatorType.Contains,     new ContainsEvaluator() },
        };

        /// <summary>
        /// Operator type for this requirement
        /// </summary>
        protected RequirementOperatorType operatorType;

        /// <summary>
        /// Gets the operator type for the requirement
        /// </summary>
        public override RequirementOperatorType Operator => this.operatorType;

        /// <summary>
        /// Gets the Jpath to get JToken or IEnumerable<JToken> from context.CustomData object
        /// </summary>
        public string DataJPath { get; }

        /// <summary>
        /// Gets the Jpath to get JToken or IEnumerable<JToken> from resource object.
        /// If we are comparing token from context to a constant value, this will be null.
        /// Check <see cref="JsonPathConstantRequirement"/> out.
        /// </summary>
        public string ResourceJPath { get; }

        /// <summary>
        /// Direction of the operator
        /// </summary>
        public Direction Direction { get; }

        /// <summary>
        /// Initializes a new instance of JsonPathRequirement which uses Json Path to check requirement satisfaction
        /// </summary>
        /// <param name="operatorType">Requirement operator type</param>
        /// <param name="dataPath">jsonPath to context object</param>
        /// <param name="resourcePath">jsonPath to resource object</param>
        /// <param name="direction">operation direction</param>
        public JsonPathRequirement(RequirementOperatorType operatorType, string dataPath, string resourcePath, Direction direction)
        {
            this.ValidateOperatorType(operatorType);

            this.operatorType = operatorType;
            this.DataJPath = dataPath ?? throw new ArgumentNullException(nameof(dataPath));
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
            if (context.CustomData == null)
            {
                throw new ArgumentNullException("JsonPath requirement needs the CustomData to be set in context");
            }

            var customDataJObject = this.GetCustomDataJObject(context);
            var resourceJObj = this.GetResourceJObject(resource);

            if (customDataJObject == null || resourceJObj == null)
            {
                return false;
            }

            var evaluatorContext = new EvaluatorContext
            {
                CustomData = customDataJObject,
                CustomDataPath = this.DataJPath,
                Resource = resourceJObj,
                ResourcePath = this.ResourceJPath,
                Direction = this.Direction,
            };

            // Delegate real evlauation to the evaluators
            return EvaluatorMap[this.operatorType].Evaluate(evaluatorContext);
        }

        /// <summary>
        /// Get the JObject representing the custom data in the AuthZyinContext
        /// </summary>
        /// <param name="context">AuthZyin context object</param>
        /// <returns>JObject for the custom data</returns>
        protected JObject GetCustomDataJObject(AuthZyinContext<TContextCustomData> context)
        {
            return JObject.FromObject(context.CustomData);
        }

        /// <summary>
        /// Get the resource in JObject format. Can be overridden
        /// </summary>
        /// <param name="resource">resource object</param>
        /// <returns>resource object wrapped in JObject format</returns>
        protected virtual JObject GetResourceJObject(TResource resource)
        {
            return JObject.FromObject(resource);
        }

        /// <summary>
        /// Check whether operator type is supported by JsonPathRequirement
        /// </summary>
        /// <param name="operatorType">operator type</param>
        private void ValidateOperatorType(RequirementOperatorType operatorType)
        {
            if (!EvaluatorMap.ContainsKey(operatorType))
            {
                throw new ArgumentOutOfRangeException($"Operator not supported by JsonPathRequirement - {operatorType.ToString()}");
            }
        }
    }
}
