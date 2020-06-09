namespace AuthZyin.Authorization.Requirements
{
    using System;
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// Supported requirement type. Anything less than 0 is for built in asp.net requirements,
    /// and cannot be built using RequirementBuilder.
    /// </summary>
    public enum RequirementOperatorType
    {
        Invalid = -100,

        // For asp.net core built in requirement serialization only
        RequiresRole = -1,

        // Direction agnostic requirements
        Or = 1,
        Equals = 2,

        // Below operators can have direction applied
        GreaterThan = 3,
        Contains = 4,
    }

    /// <summary>
    /// Operation direction. For example, when direction == ContextToResource, the "Contains" operation means info extracted from context
    /// contains info extracted from resource. And vice versa.
    /// </summary>
    public enum Direction
    {
        ContextToResource = 1,
        ResourceToContext = 2,
    }

    /// <summary>
    /// Authorization requirement
    /// </summary>
    public abstract class Requirement: IAuthorizationRequirement
    {
        /// <summary>
        /// Gets the operator type for the requirement
        /// </summary>
        public virtual RequirementOperatorType Operator => RequirementOperatorType.Invalid;

        /// <summary>
        // Evaluate current requirement against given user and resource
        /// </summary>
        /// <param name="context">authorization data context</param>
        /// <param name="resource">resource object</param>
        /// <returns>true if allowed</returns>
        public abstract bool Evaluate(IAuthZyinContext context, object resource);
    }

    /// <summary>
    /// Authorization requirement with context type
    /// </summary>
    /// <typeparam name="TContextCustomData">Type of custom data in AuthZyinContext</typeparam>
    /// <typeparam name="TResource">Type of Resource</typeparam>
    public abstract class Requirement<TContextCustomData, TResource> : Requirement
        where TContextCustomData: class
        where TResource: Resource
    {
        /// <summary>
        /// Gets a value indicationg whether the requirement requires a resource to evaluate on?
        /// Check <see cref="JsonPathConstantRequirement" /> for scenarios where resource is not needed (e.g. declarative requirements).
        /// </summary>
        protected virtual bool NeedResource => true;

        /// <summary>
        // Evaluate current requirement against given user and resource
        /// </summary>
        /// <param name="context">authorization data context</param>
        /// <param name="resource">resource object</param>
        /// <returns>true if allowed</returns>
        public sealed override bool Evaluate(IAuthZyinContext context, object resource)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (this.NeedResource && resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            var typedContext = context as AuthZyinContext<TContextCustomData> ??
                throw new InvalidCastException($"context type is unexpected. expected: {typeof(AuthZyinContext<TContextCustomData>).Name}, actual: {context.GetType().Name}");

            var typedResource = resource as TResource;
            if (this.NeedResource && typedResource == null)
            {
                throw new InvalidCastException($"resource type is unexpected. expected: {typeof(TResource).Name}, actual: {resource.GetType().Name}");
            }

            return this.Evaluate(typedContext, typedResource);
        }

        /// <summary>
        // Evaluate current requirement against given user and typed resource
        /// </summary>
        /// <param name="context">authorization data context</param>
        /// <param name="typedResource">resource object</param>
        /// <returns>true if allowed</returns>
        protected abstract bool Evaluate(AuthZyinContext<TContextCustomData> context, TResource resource);
    }
}