import { JSONPath } from 'jsonpath-plus';
import { AuthZyinContext } from "./AuthZyinContext";
import {
    Requirement,
    OrRequiremet,
    RequiresRoleRequiremet,
    IsOrRequirement,
    IsRequiresRoleRequirement,
    IsJsonPathConstantRequirement,
    IsJsonPathRequirement,
    JsonPathRequiremet,
    RequirementOperatorType,
    Direction
} from "./Requirements";
import { Resource, ConstantWrapperResource } from "./Resource";

export const evaluateRequirement = <TCustomData extends object>(
    context: AuthZyinContext<TCustomData>,
    requirement: Requirement,
    resource?: Resource
) => {
    let result = false;

    // use type guards to evaluate the concrement requirement type
    // More specific types to more generic types
    if (IsOrRequirement(requirement)) {
        result = evaluateOrRequirement(context, requirement, resource);
    } else if (IsRequiresRoleRequirement(requirement)) {
        result = evaluateRoleRequirement(context, requirement); // 
    } else if (IsJsonPathRequirement(requirement)) {
        result = evaulateJsonPathRequirement(context, requirement, resource);
    }

    return result;
}

const evaluateRoleRequirement = <TCustomData extends object>(
    context: AuthZyinContext<TCustomData>,
    roleRequirement: RequiresRoleRequiremet
) => {
    const allowedRoles = roleRequirement.allowedRoles;
    const userRoles = context.userContext.roles;

    for (let i = 0; i < allowedRoles.length; i ++) {
        if (userRoles.indexOf(allowedRoles[i]) >= 0) {
            // Allow if user has one of the allowed roles
            return true;
        }
    }

    return false;
}

const evaluateOrRequirement = <TCustomData extends object>(
    context: AuthZyinContext<TCustomData>,
    orRequirement: OrRequiremet,
    resource?: Resource
) => {
    for (let i = 0; i < orRequirement.children.length; i ++) {
        const result = evaluateRequirement(context, orRequirement.children[i], resource);
        if (result) {
            // Allow when one of the children requirements is met
            return true;
        }
    }

    return false;
}

const evaulateJsonPathRequirement = <TCustomData extends object>(
    context: AuthZyinContext<TCustomData>,
    jsonPathRequirement: JsonPathRequiremet,
    resource?: Resource
) => {
    if (IsJsonPathConstantRequirement(jsonPathRequirement)) {
        // This is to compare with a constant. Ignore resource and create a dummy one
        resource = { value: jsonPathRequirement.constValue } as ConstantWrapperResource;
    }

    if (!resource) {
        // resource is invalid or not provided.
        return false;
    }

    const camelCaseDataJPath = camelCasePropertyNames(jsonPathRequirement.dataJPath);
    const camelCaseResourceJPath = camelCasePropertyNames(jsonPathRequirement.resourceJPath);

    const dataToken = JSONPath({path: camelCaseDataJPath, json: context.customData});
    const resourceToken = JSONPath({path: camelCaseResourceJPath, json: resource});

    const left = ((jsonPathRequirement.direction === Direction.ContextToResource) ? dataToken : resourceToken);
    const right = (jsonPathRequirement.direction === Direction.ContextToResource) ? resourceToken : dataToken;

    return compareTokens(left, right, jsonPathRequirement.operator);
}

const compareTokens = (
    left: Array<object>,
    right: Array<object>,
    operator: RequirementOperatorType
) => {
    if (!left || !right || left.length <= 0 || right.length <= 0) {
        return false;
    }

    if (operator === RequirementOperatorType.Equals) {
        // We are expecting "value" comparison with equals
        return left.length === right.length && left.length === 1 && left[0] === right[0];
    } else if (operator === RequirementOperatorType.Contains) {
        // We are expecting right operand to be a "value"
        return right.length === 1 && left.some((x: unknown) => x === right[0]);
    } else if (operator === RequirementOperatorType.GreaterThan) {
        // We are expecting "value" comparison with greater than
        return left.length === right.length && left.length === 1 && left > right;
    } else {
        return false;
    }
}

const camelCasePropertyNames = (path: string) => {
    // TODO[sidecus] - better way to ignore case in JPath
    let newPath = '';
    let isPreviousCharADot = false;
    for (let i = 0; i < path.length; i ++) {
        newPath += isPreviousCharADot ? path[i].toLowerCase() : path[i];
        isPreviousCharADot = path[i] === '.';
    }

    return newPath;
}