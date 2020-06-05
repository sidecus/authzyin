namespace AuthZyin.Authorization
{
    using System;
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// Authorization requirement
    /// </summary>
    public abstract class AuthZyinRequirement: IAuthorizationRequirement
    {
        /// <summary>
        /// RequirementType used by client lib
        /// </summary>
        public virtual string RequirementType => "Unknown";

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
    public abstract class AuthZyinRequirement<TContextCustomData, TResource> : AuthZyinRequirement
        where TContextCustomData: class
        where TResource: AuthZyinResource
    {
        /// <summary>
        // Evaluate current requirement against given user and resource
        /// </summary>
        /// <param name="context">authorization data context</param>
        /// <param name="resource">resource object</param>
        /// <returns>true if allowed</returns>
        public sealed override bool Evaluate(IAuthZyinContext context, object resource)
        {
            var typedContext = context as AuthZyinContext<TContextCustomData> ??
                throw new InvalidOperationException($"IAuthZyinContext type is unexpected. expected: {typeof(AuthZyinContext<TContextCustomData>).Name}, actual: {context.GetType().Name}");
            var typedResource = resource as TResource ??
                throw new InvalidOperationException($"resource type is unexpected. expected: {typeof(TResource).Name}, actual: {resource.GetType().Name}");

            return this.EvaluateWithTypeResource(typedContext, resource as TResource);
        }

        /// <summary>
        // Evaluate current requirement against given user and typed resource
        /// </summary>
        /// <param name="context">authorization data context</param>
        /// <param name="typedResource">resource object</param>
        /// <returns>true if allowed</returns>
        protected abstract bool EvaluateWithTypeResource(AuthZyinContext<TContextCustomData> context, TResource resource);
    }
}