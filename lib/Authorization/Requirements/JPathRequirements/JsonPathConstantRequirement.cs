namespace AuthZyin.Authorization.JPathRequirements
{
    using System;
    using AuthZyin.Authorization.Requirements;

    /// <summary>
    /// Authorization requirement which uses JsonPath for filtering and comparison from custom AuthZData to a constant.
    /// </summary>
    /// <typeparam name="TData">Type of custom data in AuthZyinContext</typeparam>
    /// <typeparam name="TValue">Type of constant</typeparam>
    public class JsonPathConstantRequirement<TData, TValue> : JsonPathRequirement<TData, ValueWrapperResource<TValue>>
        where TData : class
        where TValue: IConvertible
    {
        /// <summary>
        /// Dummy resource object used by JsonPath const requirement evaluation.
        /// </summary>
        private readonly ValueWrapperResource<TValue> valueWrapperResource;

        /// <summary>
        /// This needs a const value so doesn't need a resource.
        /// </summary>
        protected override bool NeedResource => false;

        /// <summary>
        /// Value of the constant to compare with
        /// </summary>
        public TValue ConstValue { get; }

        /// <summary>
        /// Initializes a new instance of JsonPathRequirement which uses Json Path to check requirement satisfaction.
        /// 1. The Direction is always set to ContextToResource.
        /// 2. The resource type is always ConstantWrapperResource<TConst>
        /// 3. Passed in resource object will be ignored during evaluation and replaced by a on the fly instance of ConstantWrapperResource<TConst>
        /// </summary>
        /// <param name="operatorType">Requirement operator type</param>
        /// <param name="dataPath">jsonPath to context object</param>
        /// <param name="value">const to compare with</param>
        public JsonPathConstantRequirement(OperatorType operatorType, string dataPath, TValue value)
            : base(
                operatorType,
                dataPath,
                ValueWrapperResource<TValue>.ValueJsonPath,
                Direction.ContextToResource)
        {
            // We don't allow null as the value, so boxing it to compare.
            if (value as object == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.ConstValue = value;
            this.valueWrapperResource = new ValueWrapperResource<TValue>(value);
        }

        /// <summary>
        // Evaluate current requirement against given user and typed resource.
        /// </summary>
        /// <param name="context">authorization data context</param>
        /// <param name="resource">resource object</param>
        /// <returns>true if allowed</returns>
        protected override bool Evaluate(AuthZyinContext<TData> context, ValueWrapperResource<TValue> resource)
        {
            if (resource != null)
            {
                throw new InvalidOperationException("Unexpected resource passed to JsonPathConstantRequirement evaluation");
            }

            // Call base and replace the resource with the dummy resource wrapping around the const value
            return base.Evaluate(context, this.valueWrapperResource);
        }
    }
}