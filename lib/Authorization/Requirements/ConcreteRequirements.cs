namespace AuthZyin.Authorization.Requirements
{
    using System;
    using AuthZyin.Authorization.JPathRequirements;

    /// <summary>
    /// Requirement to test a property in TData equals to a property in TResource
    /// </summary>
    /// <typeparam name="TData">Data type in the AuthZyinContext</typeparam>
    /// <typeparam name="TResource">Resource type</typeparam>
    public class EqualsRequirement<TData, TResource> : JsonPathRequirement<TData, TResource>
        where TData: class
        where TResource: Resource
    {
        public EqualsRequirement(string dataPath, string resourcePath)
            : base(OperatorType.Equals, dataPath, resourcePath, Direction.ContextToResource) {}
    }

    /// <summary>
    /// Requirement to test a property in TData equals to a const value
    /// </summary>
    /// <typeparam name="TData">Data type in the AuthZyinContext</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    public class EqualsValueRequirement<TData, TValue> : JsonPathConstantRequirement<TData, TValue>
        where TData: class
        where TValue: IConvertible
    {
        public EqualsValueRequirement(string dataPath, TValue value)
            : base(OperatorType.Equals, dataPath, value) {}
    }

    /// <summary>
    /// Requirement checks greater than for properties from TData and TResource based on direction
    /// </summary>
    /// <typeparam name="TData">Data type in the AuthZyinContext</typeparam>
    /// <typeparam name="TResource">Resource type</typeparam>
    public class GreaterThanRequirement<TData, TResource> : JsonPathRequirement<TData, TResource>
        where TData: class
        where TResource: Resource
    {
        public GreaterThanRequirement(string dataPath, string resourcePath, Direction direction)
            : base(OperatorType.GreaterThan, dataPath, resourcePath, direction) {}
    }

    /// <summary>
    /// Requirement checks a property in TData is greater than a const value
    /// </summary>
    /// <typeparam name="TData">Data type in the AuthZyinContext</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    public class GreaterThanValueRequirement<TData, TValue> : JsonPathConstantRequirement<TData, TValue>
        where TData: class
        where TValue: IConvertible
    {
        public GreaterThanValueRequirement(string dataPath, TValue value)
            : base(OperatorType.GreaterThan, dataPath, value) {}
    }

    /// <summary>
    /// Requirement checks greater than or equal to for properties from TData and TResource based on direction
    /// </summary>
    /// <typeparam name="TData">Data type in the AuthZyinContext</typeparam>
    /// <typeparam name="TResource">Resource type</typeparam>
    public class GreaterThanOrEqualToRequirement<TData, TResource> : JsonPathRequirement<TData, TResource>
        where TData: class
        where TResource: Resource
    {
        public GreaterThanOrEqualToRequirement(string dataPath, string resourcePath, Direction direction)
            : base(OperatorType.GreaterThanOrEqualTo, dataPath, resourcePath, direction) {}
    }

    /// <summary>
    /// Requirement checks a property in TData is greater than or equal to a const value
    /// </summary>
    /// <typeparam name="TData">Data type in the AuthZyinContext</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    public class GreaterThanOrEqualToValueRequirement<TData, TValue> : JsonPathConstantRequirement<TData, TValue>
        where TData: class
        where TValue: IConvertible
    {
        public GreaterThanOrEqualToValueRequirement(string dataPath, TValue value)
            : base(OperatorType.GreaterThanOrEqualTo, dataPath, value) {}
    }

    /// <summary>
    /// Requirement checks a collection contains a property from TData and TResource based on the direction
    /// </summary>
    /// <typeparam name="TData">Data type in the AuthZyinContext</typeparam>
    /// <typeparam name="TResource">Resource type</typeparam>
    public class ContainsRequirement<TData, TResource> : JsonPathRequirement<TData, TResource>
        where TData: class
        where TResource: Resource
    {
        public ContainsRequirement(string dataPath, string resourcePath, Direction direction)
            : base(OperatorType.Contains, dataPath, resourcePath, direction) {}
    }

    /// <summary>
    /// Requirement checks a collection from TData contains const value
    /// </summary>
    /// <typeparam name="TData">Data type in the AuthZyinContext</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    public class ContainsValueRequirement<TData, TValue> : JsonPathConstantRequirement<TData, TValue>
        where TData: class
        where TValue: IConvertible
    {
        public ContainsValueRequirement(string dataPath, TValue value)
            : base(OperatorType.Contains, dataPath, value) {}
    }
}