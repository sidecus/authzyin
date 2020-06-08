import { Requirement } from "./Requirements";

/*
* AuthZyin context api URL - this is setup by the AutZyin library automatically for you
*/
export const AuthZyinContextApiUrl = '/authzyin/context';

/*
* AuthZyin client context definition, used by client authorization
*/
export interface AuthZyinContext<TCustomData extends object> {
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