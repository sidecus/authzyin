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

    // Allow if user has one of the allowed roles
    for (let i = 0; i < allowedRoles.length; i++) {
        if (userRoles.indexOf(allowedRoles[i]) >= 0) {
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
    // Allow when one of the children requirements is met
    for (let i = 0; i < orRequirement.children.length; i++) {
        // eslint-disable-next-line @typescript-eslint/no-use-before-define
        const result = evaluateRequirement(
            context,
            orRequirement.children[i],
            resource
        );
        if (result) {
            return true;
        }
    }

    return false;
};

const compareTokens = (
    left: Array<object>,
    right: Array<object>,
    operator: OperatorType
) => {
    if (!left || !right || left.length <= 0 || right.length <= 0) {
        return false;
    }

    if (operator === OperatorType.Equals) {
        // We are expecting "value" comparison with equals
        return (
            left.length === right.length &&
            left.length === 1 &&
            left[0] === right[0]
        );
    } else if (operator === OperatorType.Contains) {
        // We are expecting right operand to be a "value"
        return right.length === 1 && left.some((x: unknown) => x === right[0]);
    } else if (operator === OperatorType.GreaterThan) {
        // We are expecting "value" comparison with greater than
        return (
            left.length === right.length &&
            left.length === 1 &&
            left[0] > right[0]
        );
    } else {
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

    const left =
        jsonPathRequirement.direction === Direction.ContextToResource
            ? dataToken
            : resourceToken;
    const right =
        jsonPathRequirement.direction === Direction.ContextToResource
            ? resourceToken
            : dataToken;

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
