
/*
* Requirement operator type
*/
enum RequirementOperatorType
{
    Invalid = -100,

    // For asp.net core built in requirement serialization only
    RequiresRole = -1,

    // Direction agnostic requirements
    Or = 1,
    Equals = 2,

    // Below operators can have direction applied
    GreaterThan = 3,
    Contains = 4,
}

/*
* Operator direction
*/
export enum Direction {
    ContextToResource = 1,
    ResourceToContext = 2,
}

/*
* AuthZyin client requirement definition. Serialized from server and will be used in client authorization.
*/
interface BaseRequirement {
    operator: RequirementOperatorType;
}

export interface OrRequiremet extends BaseRequirement {
    children: Requirement[];
}

export interface RequiresRoleRequiremet extends BaseRequirement {
    allowedRoles: string[];
}

export interface JsonPathRequiremet extends BaseRequirement {
    dataJPath: string;
    resourceJPath: string;
    direction: Direction;
}

export interface JsonPathConstantRequiremet extends JsonPathRequiremet {
    constValue: unknown;    // only available when we are comparing with a const value instead of real resource
}

export type Requirement = OrRequiremet | RequiresRoleRequiremet | JsonPathRequiremet;

/*
* Requirement type guards for easy type manipulation.
* https://www.typescriptlang.org/docs/handbook/advanced-types.html#type-guards-and-differentiating-types
*/
export function IsOrRequirement(requirement: Requirement): requirement is OrRequiremet {
    return requirement.operator == RequirementOperatorType.Or;
}

export function IsRequiresRoleRequirement(requirement: Requirement): requirement is RequiresRoleRequiremet {
    return requirement.operator == RequirementOperatorType.RequiresRole;
}

export function IsJsonPathRequirement(requirement: Requirement): requirement is JsonPathRequiremet {
    return requirement.operator != RequirementOperatorType.Invalid &&
           requirement.operator != RequirementOperatorType.Or &&
           requirement.operator != RequirementOperatorType.RequiresRole &&
           (requirement as JsonPathRequiremet).dataJPath !== undefined;
}

export function IsJsonPathConstantRequirement(requirement: Requirement): requirement is JsonPathConstantRequiremet {
    return IsJsonPathRequirement(requirement) &&
            (requirement as JsonPathConstantRequiremet).constValue !== undefined;
}