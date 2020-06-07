/*
* AuthZyin context api URL - this is setup by the AutZyin library automatically for you
*/
export const contextApiUrl = '/authzyin/context';

/*
* AuthZyin client context definition, used by client authorization
*/
export interface ClientContext<TCustomData> {
    userContext: UserContext;
    policies: ClientPolicy[];
    customData: TCustomData;
}

/*
* AuthZyin user context definition (part of client context)
*/
export interface UserContext {
    userId: string;
    userName: string;
    tenantId: string;
    roles: string[];
}

/*
* AuthZyin client policy definition. Serialized from server and will be used in client authorization.
*/
export interface ClientPolicy {
    name: string;
    requirements: Requirement[];
}

/*
* AuthZyin client policy definition. Serialized from server and will be used in client authorization.
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
* AuthZyin client requirement definition. Serialized from server and will be used in client authorization.
*/
interface BaseRequirement {
    operator: RequirementOperatorType;
}

export enum JsonRequirementDirection {
    ContextToResource = 1,
    ResourceToContext = 2,
}

export interface JsonPathRequiremet extends BaseRequirement {
    customDataJPath: string;
    resourceJPath: string;
    direction: JsonRequirementDirection;
    constValue: unknown;    // only available when we are comparing with a const value instead of real resource
}

export interface OrRequiremet extends BaseRequirement {
    children: Requirement[];
}

export type Requirement = JsonPathRequiremet | OrRequiremet;

/*
* Requirement type guards for easy type manipulation.
* https://www.typescriptlang.org/docs/handbook/advanced-types.html#type-guards-and-differentiating-types
*/