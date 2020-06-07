import { AuthZyinContext } from "./AuthZyinContext";
import { IsOrRequirement, Requirement, OrRequiremet, IsRequiresRoleRequirement, RequiresRoleRequiremet, IsJsonPathConstantRequirement, IsJsonPathRequirement, JsonPathRequiremet } from "./Requirements";
import { Resource } from "./Resource";

export const evaluateRequirement = <TCustomData>(context: AuthZyinContext<TCustomData>, requirement: Requirement, resource?: Resource) => {
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

const evaluateRoleRequirement = <TCustomData>(context: AuthZyinContext<TCustomData>, roleRequirement: RequiresRoleRequiremet) => {
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

const evaluateOrRequirement = <TCustomData>(context: AuthZyinContext<TCustomData>, orRequirement: OrRequiremet, resource?: Resource) => {
    for (let i = 0; i < orRequirement.children.length; i ++) {
        const result = evaluateRequirement(context, orRequirement.children[i], resource);
        if (result) {
            // Allow when one of the children requirements is met
            return true;
        }
    }

    return false;
}

const evaulateJsonPathRequirement = <TCustomData>(context: AuthZyinContext<TCustomData>, jsonPathRequirement: JsonPathRequiremet, resource?: Resource) => {
    // TODO[sidecus]: Evaulate json requirement
    return true;
}