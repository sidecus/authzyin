import { AuthZyinContext, ClientPolicy } from './AuthZyinContext';
import { Requirement, IsOrRequirement, IsJsonPathRequirement } from './Requirements';

/**
 * Convert property names in a json path to camel case.
 * e.g., "$.SomeData.Value" to "$.someData.value"
 * @param path json path
 */
export const camelCaseJsonPath = (path: string) => {
    // TODO[sidecus] - more performant way to ignore case in JPath
    let newPath = '';
    let isPreviousCharADot = false;
    for (let i = 0; i < path.length; i++) {
        newPath += isPreviousCharADot ? path[i].toLowerCase() : path[i];
        isPreviousCharADot = path[i] === '.';
    }

    return newPath;
};

export const camelCaseRequirement = (requirement: Requirement) => {
    if (IsOrRequirement(requirement) && requirement.children) {
        requirement.children.forEach((r) => camelCaseRequirement(r));
    } else if (IsJsonPathRequirement(requirement)) {
        requirement.dataJPath = camelCaseJsonPath(requirement.dataJPath);
        requirement.resourceJPath = camelCaseJsonPath(requirement.resourceJPath);
    }
};

export const camelCasePolicy = (policy: ClientPolicy) => {
    if (policy && policy.requirements && policy.requirements.length > 0) {
        policy.requirements.forEach((r) => camelCaseRequirement(r));
    }
};

/**
 * Update all property names referenced by JSONPath in requirements
 * used by the context to be camel case.
 * @param context AuthzyinContext
 */
export const camelCaseContext = (context: AuthZyinContext<object>) => {
    if (context && context.policies) {
        context.policies.forEach((p) => camelCasePolicy(p));
    }
};
