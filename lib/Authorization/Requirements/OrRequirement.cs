namespace AuthZyin.Authorization.Requirements
{
    using System;

    /// <summary>
    /// Authorization requirement with OR condition among children requirements.
    /// The requirement is met if any of its children requirement is satisfied.
    /// </summary>
    public sealed class OrRequirement: Requirement
    {
        /// <summary>
        /// children requirements
        /// </summary>
        private readonly Requirement[] children;

        /// <summary>
        /// Gets the children requirements for OrRequirement
        /// TODO[sidecus]: Workaround for new System.Text.Json serialization
        /// https://github.com/dotnet/runtime/issues/31742
        /// https://github.com/dotnet/runtime/issues/29937
        /// </summary>
        public object[] Children => this.children;

        /// <summary>
        /// Create a new instance of OrRequirement
        /// </summary>
        /// <param name="requirements">children requirements</param>
        public OrRequirement(params Requirement[] requirements)
        {
            if (requirements == null || requirements.Length == 0)
            {
                throw new ArgumentNullException(nameof(requirements));
            }

            this.Operator = RequirementOperatorType.Or;
            this.children = requirements;
        }

        /// <summary>
        // Evaluate current requirement against given user and resource
        /// </summary>
        /// <param name="context">authorization data context</param>
        /// <param name="resource">resource object</param>
        /// <returns>true if allowed</returns>
        public override bool Evaluate(IAuthZyinContext context, object resource)
        {
            foreach (var child in this.children)
            {
                if (child.Evaluate(context, resource))
                {
                    return true;
                }
            }

            return false;
        }
    }
}