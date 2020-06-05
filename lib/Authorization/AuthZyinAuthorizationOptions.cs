namespace AuthZyin.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// Interface to retrieve the list of policies configured in authorization
    /// </summary>
    public interface IAuthorizationPolicyList
    {
        /// <summary>
        /// Gets the list of policices configured
        /// </summary>
        IEnumerable<(string name, AuthorizationPolicy policy)> Policies { get; }
    }

    /// <summary>
    /// The AuthZyinAuthorizationOptions - HACKing by inheriting from AuthorizationOptions.
    /// This is a hack since there seems no way to retrieve the configured policies. It's a private member in AuthorizationOptions class.
    /// </summary>
    public class AuthZyinAuthorizationOptions: AuthorizationOptions, IAuthorizationPolicyList
    {
        /// <summary>
        /// We have to save it to our own map so that we can use it later.
        /// </summary>
        private List<(string name, AuthorizationPolicy policy)> PolicyList = new List<(string name, AuthorizationPolicy policy)>();

        /// <summary>
        /// Contains a list of actions to perform, used to create an Action<AuthorizationPolicy>
        /// </summary>
        private List<Action<AuthorizationOptions>> configureActionList = new List<Action<AuthorizationOptions>>();

        /// <summary>
        ///  Get all the configured policies
        /// </summary>
        public IEnumerable<(string name, AuthorizationPolicy policy)> Policies => this.PolicyList;

        //
        // Summary:
        //     Gets or sets the default authorization policy. Defaults to require authenticated
        //     users.
        //
        // Remarks:
        //     The default policy used when evaluating Microsoft.AspNetCore.Authorization.IAuthorizeData
        //     with no policy name specified.
        public new AuthorizationPolicy DefaultPolicy
        {
            get
            {
                return base.DefaultPolicy;
            }

            set
            {
                base.DefaultPolicy = value;
                this.configureActionList.Add(x => x.DefaultPolicy = value);
            }
        }

        //
        // Summary:
        //     Gets or sets the fallback authorization policy used by Microsoft.AspNetCore.Authorization.AuthorizationPolicy.CombineAsync(Microsoft.AspNetCore.Authorization.IAuthorizationPolicyProvider,System.Collections.Generic.IEnumerable{Microsoft.AspNetCore.Authorization.IAuthorizeData})
        //     when no IAuthorizeData have been provided. As a result, the AuthorizationMiddleware
        //     uses the fallback policy if there are no Microsoft.AspNetCore.Authorization.IAuthorizeData
        //     instances for a resource. If a resource has any Microsoft.AspNetCore.Authorization.IAuthorizeData
        //     then they are evaluated instead of the fallback policy. By default the fallback
        //     policy is null, and usually will have no effect unless you have the AuthorizationMiddleware
        //     in your pipeline. It is not used in any way by the default Microsoft.AspNetCore.Authorization.IAuthorizationService.
        public new AuthorizationPolicy FallbackPolicy
        {
            get
            {
                return base.FallbackPolicy;
            }

            set
            {
                base.FallbackPolicy = value;
                this.configureActionList.Add(x => x.FallbackPolicy = value);
            }
        }

        //
        // Summary:
        //     Determines whether authentication handlers should be invoked after a failure.
        //     Defaults to true.
        public new bool InvokeHandlersAfterFailure
        {
            get
            {
                return base.InvokeHandlersAfterFailure;
            }

            set
            {
                base.InvokeHandlersAfterFailure = value;
                this.configureActionList.Add(x => x.InvokeHandlersAfterFailure = value);
            }
        }

        /// <summary>
        /// Returns an action which configures an AuthorizationOptions object with the same configuration actions
        /// </summary>
        /// <param name="options">authorization options</param>
        public void ConfigureAuthorizationOptions(AuthorizationOptions options)
        {
            foreach(var action in this.configureActionList)
            {
                action(options);
            }
        }

        /// <summary>
        /// Add an authorization policy with the provided name.
        /// </summary>
        /// <param name="name">The name of the policy.</param>
        /// <param name="policy">The authorization policy.</param>
        public new void AddPolicy(string name, AuthorizationPolicy policy)
        {
           if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }
            
            this.AddPolicyInternal(name, policy);
        }

        /// <summary>
        /// Add a policy that is built from a delegate with the provided name.
        /// </summary>
        /// <param name="name">The name of the policy.</param>
        /// <param name="configurePolicy">The delegate that will be used to build the policy.</param>
        public new void AddPolicy(string name, Action<AuthorizationPolicyBuilder> configurePolicy)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (configurePolicy == null)
            {
                throw new ArgumentNullException(nameof(configurePolicy));
            }

            var policyBuilder = new AuthorizationPolicyBuilder();
            configurePolicy(policyBuilder);
            this.AddPolicyInternal(name, policyBuilder.Build());
        }

        /// <summary>
        /// Add policy internal - to both our own list and the base class dictionary
        /// </summary>
        /// <param name="name">policy name</param>
        /// <param name="policy">policy object</param>
        private void AddPolicyInternal(string name, AuthorizationPolicy policy)
        {
            this.PolicyList.Add((name, policy));

            base.AddPolicy(name, policy);
            this.configureActionList.Add(x => x.AddPolicy(name, policy));
        }
    }
}