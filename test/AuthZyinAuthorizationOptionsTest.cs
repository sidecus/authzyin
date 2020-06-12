namespace test
{
    using System.Linq;
    using Xunit;
    using Microsoft.AspNetCore.Authorization;
    using AuthZyin.Authorization;
    using System.Collections.Generic;
    using System;

    public class AuthZyinAuthorizationOptionsTest
    {
        private static readonly AuthorizationPolicy rolePolicy = new AuthorizationPolicyBuilder().RequireRole("somerole").Build();
        private static readonly AuthorizationPolicy namePolicy = new AuthorizationPolicyBuilder().RequireUserName("someName").Build();
        private static readonly Action<AuthorizationPolicyBuilder> claimPolicyConfigure = b => b.RequireClaim("someClaim");
        private static readonly Action<AuthorizationPolicyBuilder> requirementConfigure = b => b.AddRequirements(new TestRequirement(false, true));

        private static readonly Dictionary<string, AuthorizationPolicy> testPolicies = new Dictionary<string, AuthorizationPolicy>()
        {
            { nameof(rolePolicy), rolePolicy },
            { nameof(namePolicy), namePolicy },
        };

        private static readonly Dictionary<string, Action<AuthorizationPolicyBuilder>> testPolicyBuilders = new Dictionary<string, Action<AuthorizationPolicyBuilder>>()
        {
            { nameof(claimPolicyConfigure), claimPolicyConfigure },
            { nameof(requirementConfigure), requirementConfigure },
        };

        [Fact]
        public void AddPolicyThrowsOnInvalidArg()
        {
            var options = new AuthZyinAuthorizationOptions();

            Assert.Throws<ArgumentNullException>(() => options.AddPolicy(null, (AuthorizationPolicy)null));
            Assert.Throws<ArgumentNullException>(() => options.AddPolicy(null, (Action<AuthorizationPolicyBuilder>)null));
            Assert.Throws<ArgumentNullException>(() => options.AddPolicy(nameof(rolePolicy), (AuthorizationPolicy)null));
            Assert.Throws<ArgumentNullException>(() => options.AddPolicy(nameof(claimPolicyConfigure), (Action<AuthorizationPolicyBuilder>)null));

            // Throws on duplicate policies
            options.AddPolicy(nameof(rolePolicy), rolePolicy);
            Assert.Throws<ArgumentException>(() => options.AddPolicy(nameof(rolePolicy), rolePolicy));
        }

        [Fact]
        public void OptionsAreConfiguredCorrectly()
        {
            var ourOptions = new AuthZyinAuthorizationOptions();
            ourOptions.DefaultPolicy = rolePolicy;
            ourOptions.FallbackPolicy = ourOptions.DefaultPolicy;
            ourOptions.InvokeHandlersAfterFailure = false;
            testPolicies.ToList().ForEach(kvp => ourOptions.AddPolicy(kvp.Key, kvp.Value));
            testPolicyBuilders.ToList().ForEach(kvp => ourOptions.AddPolicy(kvp.Key, kvp.Value));

            // Compare with base
            Assert.True(this.AreOptionsEqual(ourOptions, ourOptions as AuthorizationOptions));

            // Compare to a new AuthorizationOptions object using the "captured" actions
            var newOptions = new AuthorizationOptions();
            ourOptions.CapturedConfigureAction(newOptions);
            Assert.True(this.AreOptionsEqual(ourOptions, newOptions));
        }
        
        private bool AreOptionsEqual(AuthZyinAuthorizationOptions left, AuthorizationOptions right)
        {
            var propertiesAreTheSame = 
                left != null && right != null &&
                left.DefaultPolicy == right.DefaultPolicy &&
                left.FallbackPolicy == right.FallbackPolicy &&
                left.InvokeHandlersAfterFailure == right.InvokeHandlersAfterFailure;

            var policiesAreTheSame = left.Policies.All(x => x.policy == right.GetPolicy(x.name));

            return propertiesAreTheSame && policiesAreTheSame;
        }
    }
}