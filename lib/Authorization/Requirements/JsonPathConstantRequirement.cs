namespace AuthZyin.Authorization.Requirements
{
    using System;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Authorization requirement which uses JsonPath for filtering and comparison from custom AuthZData to a constant.
    /// </summary>
    /// <typeparam name="TData">Type of custom data in AuthZyinContext</typeparam>
    /// <typeparam name="TConst">Type of constant</typeparam>
    public class JsonPathConstantRequirement<TData, TConst> : JsonPathRequirement<TData, ConstantWrapperResource<TConst>>
        where TData : class
        where TConst: IConvertible
    {
        /// <summary>
        /// This needs a const value so doesn't need a resource.
        /// </summary>
        protected override bool NeedResource => false;

        /// <summary>
        /// Value of the constant to compare with
        /// </summary>
        public TConst ConstValue { get; }

        /// <summary>
        /// Initializes a new instance of JsonPathRequirement which uses Json Path to check requirement satisfaction.
        /// 1. The Direction is always set to ContextToResource.
        /// 2. The resource type is always ConstantWrapperResource<TConst>
        /// 3. Passed in resource object will be ignored during evaluation and replaced by a on the fly instance of ConstantWrapperResource<TConst>
        /// </summary>
        /// <param name="operatorType">Requirement operator type</param>
        /// <param name="dataPath">jsonPath to context object</param>
        /// <param name="constValue">const to compare with</param>
        public JsonPathConstantRequirement(
            OperatorType operatorType,
            string dataPath,
            TConst constValue)
            : base(
                operatorType,
                dataPath,
                ConstantWrapperResource<TConst>.ValueJsonPath,
                Direction.ContextToResource)
        {
            // TODO[sidecus] - what if constValue is null? We need to make sure the client works fine.
            this.ConstValue = constValue;
        }

        /// <summary>
        /// Get a const value resource with the const value. The passed in resource will be replaced by this.
        /// </summary>
        /// <param name="resource">JObject representing resource - always null in this case</param>
        /// <returns>a resource object wrapping the const value in JObject format</returns>
        protected sealed override JObject GetResourceJObject(ConstantWrapperResource<TConst> resource)
        {
            if (resource != null)
            {
                throw new InvalidOperationException("Unexpected resource passed to JsonPathConstantRequirement evaluation");
            }

            return JObject.FromObject(new ConstantWrapperResource<TConst>(this.ConstValue));
        }
    }
}