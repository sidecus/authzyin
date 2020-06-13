import { Resource } from './Resource';
import { AuthZyinContext } from './AuthZyinContext';
import { evaluateRequirement } from './RequirementEvaluator';
import { useAuthZyinContext } from './AuthZyinProvider';

/**
 * authorize method which finds the policy and evaluates all requirements in it
 * @param context - AuthZyin context object containing user context, policy definitions and custom authorization data
 * @param policy - the policy to authorize against
 * @param resource - resource, optional depending  on the policy and requirement
 */
export const authorize = <TData extends object = object>(
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
            return false; // requirement failed, no need to continue
        }
    }

    return true;
};

/**
 * Authorization hooks, returns a convenient authorize method you can use in your component.
 * An always false func is returned when context is not initialized.
 */
export const useAuthorize = <TData extends object = object>() => {
    const context = useAuthZyinContext<TData>();
    return (policy: string, resource?: Resource) => {
        return context && context.userContext && authorize(context, policy, resource);
    };
};
