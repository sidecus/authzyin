namespace sample.AuthN
{
    using AuthZyin.Authorization;

    public class AdminOfRequirement: ContainsRequirement<Membership, Department>
    {
        private static readonly string MembershipJsonPath = "$.AdminOf[*]";
        private static readonly string DepartmentJsonPath = "$";

        /// <summary>
        /// Initializes a new instance of AdminOfRequirement
        /// </summary>
        public AdminOfRequirement() : base(MembershipJsonPath, DepartmentJsonPath, Direction.ContextToResource)
        {
        }
    }
}