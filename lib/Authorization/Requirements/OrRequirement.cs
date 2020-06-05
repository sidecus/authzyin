namespace AuthZyin.Authorization
{
    using System;

    /// <summary>
    /// Authorization requirement with OR condition among children requirements.
    /// The requirement is met if any of its children requirement is satisfied.
    /// </summary>
    public sealed class OrRequirement: AuthZyinRequirement
    {
        /// <summary>
        /// RequirementType used by client lib
        /// </summary>
        public override string RequirementType => "Or";

        /// <summary>
        /// children requirements
        /// </summary>
        public AuthZyinRequirement[] children { get; }

        /// <summary>
        /// Create a new instance of OrRequirement
        /// </summary>
        /// <param name="requirements">children requirements</param>
        public OrRequirement(params OrRequirement[] requirements)
        {
            this.children = requirements ?? throw new ArgumentNullException(nameof(requirements));
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