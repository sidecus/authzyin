import { useContext } from 'react';
import { Resource } from './Resource';
import { AuthZyinContext } from './AuthZyinContext';
import { evaluateRequirement } from './RequirementEvaluator';
import { authZyinReactContext } from './AuthZyinProvider';

/*
 * authorize method which finds the policy and evaluates all requirements in it
 */
export const authorize = <TData extends object>(
    context: AuthZyinContext<TData>,
    policy: string,
    resource?: Resource
) => {
    if (!context || !context.policies) {
        // Incorrect context
        return false;
    }

    const policyObject = context.policies.filter((p) => p.name === policy)[0];

    if (!policyObject || !policyObject.requirements) {
        // Cannot find policy
        return false;
    }

    const requirements = policyObject.requirements;
    let result = true;

    for (let i = 0; i < requirements.length; i++) {
        result = evaluateRequirement(context, requirements[i], resource);
        if (!result) {
            // current requirement failed, no need to continue
            return false;
        }
    }

    return true;
};

/*
 * Authorization hooks
 */
export const useAuthorize = <TData extends object>() => {
    const reactContextRef = authZyinReactContext as React.Context<
        AuthZyinContext<TData>
    >;
    if (!reactContextRef) {
        throw new Error(
            'AuthZyin authorization React context is not setup. Have you called createAuthorizationContext?'
        );
    }

    const context = useContext(reactContextRef);

    return (policy: string, resource?: Resource) => {
        // Return false when context is not fully loaded yet
        return (
            context &&
            context.userContext &&
            authorize(context, policy, resource)
        );
    };
};
