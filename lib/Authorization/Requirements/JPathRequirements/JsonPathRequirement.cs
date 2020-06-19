namespace AuthZyin.Authorization.JPathRequirements
{
    using System;
    using System.Collections.Generic;
    using AuthZyin.Authorization.Requirements;

    /// <summary>
    /// Authorization requirement which uses JsonPath for filtering and comparison.
    /// Server evaluation is done via Newtonsoft.Json.
    /// Client evaluation is done via JsonPath plus - https://github.com/s3u/JSONPath;
    /// </summary>
    /// <typeparam name="TData">Type of custom data in AuthZyinContext</typeparam>
    /// <typeparam name="TResource">Type of Resource</typeparam>
    public class JsonPathRequirement<TData, TResource> : Requirement<TData, TResource>
        where TData: class
        where TResource: Resource
    {
        /// <summary>
        /// Evaluator map
        /// </summary>
        /// <typeparam name="RequirementOperatorType">operator type</typeparam>
        /// <typeparam name="RequirementEvaluator">evaluator</typeparam>
        private static readonly Dictionary<OperatorType, RequirementEvaluator> EvaluatorMap = new Dictionary<OperatorType, RequirementEvaluator>()
        {
            { OperatorType.Equals,                  new EqualsEvaluator() },
            { OperatorType.GreaterThan,             new GreaterThanEvaluator() },
            { OperatorType.GreaterThanOrEqualTo,    new GreaterThanOrEqualToEvaluator() },
            { OperatorType.Contains,                new ContainsEvaluator() },
        };

        /// <summary>
        /// Gets the Jpath to get JToken or IEnumerable<JToken> from context.Data object
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
        public JsonPathRequirement(OperatorType operatorType, string dataPath, string resourcePath, Direction direction)
        {
            this.DataJPath = dataPath ?? throw new ArgumentNullException(nameof(dataPath));
            this.ResourceJPath = resourcePath ?? throw new ArgumentNullException(nameof(resourcePath));

            if (!EvaluatorMap.ContainsKey(operatorType))
            {
                throw new ArgumentOutOfRangeException($"Operator not supported by JsonPathRequirement - {operatorType.ToString()}");
            }

            this.Operator = operatorType;

            if (direction != Direction.ContextToResource && direction != Direction.ResourceToContext)
            {
                throw new ArgumentOutOfRangeException(nameof(direction));
            }

            this.Direction = direction;
        }

        /// <summary>
        // Evaluate current requirement against given user and typed resource.
        /// </summary>
        /// <param name="context">authorization data context</param>
        /// <param name="resource">resource object</param>
        /// <returns>true if allowed</returns>
        protected override bool Evaluate(AuthZyinContext<TData> context, TResource resource)
        {
            if (context?.Data == null)
            {
                throw new ArgumentNullException("JsonPath requirement needs the data to be set in context");
            }

            if (resource == null)
            {
                throw new ArgumentNullException("JsonPath based requirement needs the resource object");
            }

            var dataJObject = context.GetDataAsJObject();
            var resourceJObj = resource.GetResourceAsJObject();

            if (dataJObject == null || resourceJObj == null)
            {
                return false;
            }

            // Create evluation context and delegate real evlauation to the evaluators
            var evaluatorContext = new EvaluatorContext(
                dataJObject,
                this.DataJPath,
                resourceJObj,
                this.ResourceJPath,
                this.Direction);
            return EvaluatorMap[this.Operator].Evaluate(evaluatorContext);
        }
    }
}
