import { JSONPath } from 'jsonpath-plus';
import { AuthZyinContext } from './AuthZyinContext';
import {
    Requirement,
    OrRequiremet,
    RequiresRoleRequiremet,
    IsOrRequirement,
    IsRequiresRoleRequirement,
    IsJsonPathConstantRequirement,
    IsJsonPathRequirement,
    JsonPathRequiremet,
    OperatorType,
    Direction
} from './Requirements';
import { Resource, ConstantWrapperResource } from './Resource';

const evaluateRoleRequirement = <TData extends object>(
    context: AuthZyinContext<TData>,
    roleRequirement: RequiresRoleRequiremet
) => {
    const allowedRoles = roleRequirement.allowedRoles;
    const userRoles = context.userContext.roles;

    for (let i = 0; i < allowedRoles.length; i++) {
        if (userRoles.indexOf(allowedRoles[i]) >= 0) {
            // Allow if user has one of the allowed roles
            return true;
        }
    }

    return false;
};

const evaluateOrRequirement = <TData extends object>(
    context: AuthZyinContext<TData>,
    orRequirement: OrRequiremet,
    resource?: Resource
) => {
    for (let i = 0; i < orRequirement.children.length; i++) {
        // eslint-disable-next-line @typescript-eslint/no-use-before-define
        const result = evaluateRequirement(context, orRequirement.children[i], resource);
        if (result) {
            // Allow when one of the children requirements is met
            return true;
        }
    }

    return false;
};

const compareTokens = (left: Array<object>, right: Array<object>, operator: OperatorType) => {
    if (!left || !right || left.length <= 0 || right.length <= 0) {
        return false;
    }

    switch (operator) {
        case OperatorType.Equals:
        case OperatorType.GreaterThan:
        case OperatorType.GreaterThanOrEqualTo:
            // We are expecting "value" comparisons for these cases
            return (
                left.length === 1 &&
                left.length === right.length &&
                ((operator === OperatorType.Equals && left[0] === right[0]) ||
                    (operator === OperatorType.GreaterThan && left[0] > right[0]) ||
                    (operator === OperatorType.GreaterThanOrEqualTo && left[0] >= right[0]))
            );
        case OperatorType.Contains:
            // We are expecting right operand to be a "value"
            return right.length === 1 && left.some((x: unknown) => x === right[0]);
        default:
            return false;
    }
};

const evaulateJsonPathRequirement = <TData extends object>(
    context: AuthZyinContext<TData>,
    jsonPathRequirement: JsonPathRequiremet,
    resource?: Resource
) => {
    // Create dummy resource first if this is to compare with a constant.
    if (IsJsonPathConstantRequirement(jsonPathRequirement)) {
        resource = {
            value: jsonPathRequirement.constValue
        } as ConstantWrapperResource;
    }

    if (!resource) {
        // resource is invalid or not provided.
        return false;
    }

    const dataToken = JSONPath({
        path: jsonPathRequirement.dataJPath,
        json: context.data
    });
    const resourceToken = JSONPath({
        path: jsonPathRequirement.resourceJPath,
        json: resource
    });

    const left = jsonPathRequirement.direction === Direction.ContextToResource ? dataToken : resourceToken;
    const right = jsonPathRequirement.direction === Direction.ContextToResource ? resourceToken : dataToken;

    return compareTokens(left, right, jsonPathRequirement.operator);
};

export const evaluateRequirement = <TData extends object>(
    context: AuthZyinContext<TData>,
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
};
