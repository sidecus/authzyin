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
* AuthZyin client requirement definition. Serialized from server and will be used in client authorization.
*/
interface BaseRequirement {
    type: string;
}

export enum JsonRequirementDirection {
    ContextToResource = 1,
    ResourceToContext = 2,
}

export interface JsonRequiremet extends BaseRequirement {
    direction: JsonRequirementDirection;
    contextJPath: string;
    resourceJPath: string;
}

export interface OrRequiremet extends BaseRequirement {
    children: Requirement[];
}

export type Requirement = JsonRequiremet & OrRequiremet;

/*
* Requirement type guards for easy type manipulation.
* https://www.typescriptlang.org/docs/handbook/advanced-types.html#type-guards-and-differentiating-types
*/