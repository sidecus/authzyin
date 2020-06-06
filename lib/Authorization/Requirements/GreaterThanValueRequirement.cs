namespace AuthZyin.Authorization
{
    using System;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// GreaterThanValueRequirement which compares properties from context with a constant
    /// </summary>
    /// <typeparam name="TContextCustomData">Type of custom data in AuthZyinContext</typeparam>
    public sealed class GreaterThanValueRequirement<TContextCustomData> : Requirement<TContextCustomData, AuthZyinResource>
        where TContextCustomData: class
    {
        /// <summary>
        /// RequirementType used by client lib
        /// </summary>
        public sealed override string RequirementType => "GreaterThanValue";

        /// <summary>
        /// JPath to context object to get a token
        /// TODO[zhezhu]: This is copied from JsonPathBaseRequirement. Need to rethink about the class hierarchy
        /// </summary>
        public string ContextJPath { get; }

        /// <summary>
        /// Const Minimum value to compare against
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Initializes a new instance of JsonPathGreaterThanConstRequirement which uses Json Path to check requirement satisfaction
        /// </summary>
        /// <param name="contextPath">jsonPath to context object</param>
        /// <param name="value">Value to compare to</param> 
        public GreaterThanValueRequirement(string contextPath, int value)
        {
            this.ContextJPath = contextPath ?? throw new ArgumentNullException(nameof(contextPath));
            this.Value = value;
        }

        /// <summary>
        /// Evaluate the context against the given value
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="resource">resource</param>
        /// <returns>true if the token noted by ContextJPath is greater than the minimum value</returns>
        protected override bool Evaluate(AuthZyinContext<TContextCustomData> context, AuthZyinResource resource)
        {
            var jObj = JObject.FromObject(context);
            var token = jObj.SelectToken(this.ContextJPath);

            if (token?.Type != JTokenType.Integer)
            {
                return false;
            }

            return (int)token > this.Value;
        }
    }
}
