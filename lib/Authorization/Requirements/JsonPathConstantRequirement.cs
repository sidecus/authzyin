namespace AuthZyin.Authorization.Requirements
{
    using System;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Authorization requirement which uses JsonPath for filtering and comparison from custom AuthZData to a constant.
    /// Server evaluation is done via Newtonsoft.Json.
    /// Client evaluation is done via JsonPath plus - https://github.com/s3u/JSONPath;
    /// </summary>
    /// <typeparam name="TContextCustomData">Type of custom data in AuthZyinContext</typeparam>
    /// <typeparam name="TConst">Type of constant</typeparam>
    public class JsonPathConstantRequirement<TContextCustomData, TConst> : JsonPathRequirement<TContextCustomData, ConstantWrapperResource<TConst>>
        where TContextCustomData : class
    {
        /// <summary>
        /// JPath to select the const value out of the dummy resource wrapping the constant
        /// </summary>
        private static readonly string ConstResourceValueJPath = "$.Value";

        /// <summary>
        /// This needs a const value so doesn't need a resource.
        /// </summary>
        protected override bool NeedResource => false;

        /// <summary>
        /// Value of the constant to compare with
        /// </summary>
        public TConst ConstValue { get; }

        /// <summary>
        /// Initializes a new instance of JsonPathRequirement which uses Json Path to check requirement satisfaction
        /// </summary>
        /// <param name="operatorType">Requirement operator type</param>
        /// <param name="dataPath">jsonPath to context object</param>
        /// <param name="constValue">const to compare with</param>
        /// <param name="direction">operation direction</param>
        public JsonPathConstantRequirement(
            RequirementOperatorType operatorType,
            string dataPath,
            TConst constValue,
            Direction direction)
            : base(operatorType, dataPath, ConstResourceValueJPath, direction)
        {
            this.ConstValue = constValue;
        }

        /// <summary>
        /// Get a const value resource with the const value. The passed in resource will be replaced by this.
        /// </summary>
        /// <param name="resource">JObject representing resource - always null in this case</param>
        /// <returns>a resource object wrapping the const value in JObject format</returns>
        protected override JObject GetResourceJObject(ConstantWrapperResource<TConst> resource)
        {
            if (resource != null)
            {
                throw new InvalidOperationException("Unexpected resource passed to JsonPathConstantRequirement evaluation");
            }

            return JObject.FromObject(new ConstantWrapperResource<TConst>(this.ConstValue));
        }
    }
}