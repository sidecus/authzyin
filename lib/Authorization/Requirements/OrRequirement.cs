namespace AuthZyin.Authorization
{
    using System;

    /// <summary>
    /// Authorization requirement with OR condition among children requirements.
    /// The requirement is met if any of its children requirement is satisfied.
    /// </summary>
    public sealed class OrRequirement: AbstractRequirement
    {
        /// <summary>
        /// children requirements
        /// </summary>
        private readonly AbstractRequirement[] children;

        /// <summary>
        /// RequirementType used by client lib
        /// </summary>
        public sealed override string Type => "Or";

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
        public OrRequirement(params AbstractRequirement[] requirements)
        {
            this.children = requirements ?? throw new ArgumentNullException(nameof(requirements));
        }

        /// <summary>
        // Evaluate current requirement against given user and resource
        /// </summary>
        /// <param name="context">authorization data context</param>
        /// <param name="resource">resource object</param>
        /// <returns>true if allowed</returns>
        public sealed override bool Evaluate(IAuthZyinContext context, object resource)
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