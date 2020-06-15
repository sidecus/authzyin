import { AuthZyinContext, ClientPolicy } from './AuthZyinContext';
import { Requirement, IsOrRequirement, IsJsonPathRequirement } from './Requirements';

/**
 * Convert property names in a json path to camel case.
 * e.g., "$.SomeData.Value" to "$.someData.value"
 * @param path json path
 */
export const camelCaseJsonPath = (path: string) => {
    if (!path) {
        return path;
    }

    const pathLength = path.length;
    let result = '';
    let nextStart = 0;
    while (nextStart < pathLength) {
        let current = path.indexOf('.', nextStart);
        current = current === -1 ? pathLength : current + 1;
        // copy from the next start position till  this dot
        result += path.substr(nextStart, current - nextStart);
        if (current < pathLength && path[current] !== '.') {
            // camel case next character after this dot (unless it is aslo dot)
            result += path[current].toLowerCase();
            current++;
        }
        nextStart = current;
    }

    return result;
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
