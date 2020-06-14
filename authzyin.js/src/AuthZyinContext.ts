import { Requirement } from './Requirements';

/**
 * AuthZyin client context definition, used by client authorization.
 * This is the client contract for ClientContext class from the server.
 */
export interface AuthZyinContext<TData extends object = object> {
    userContext: UserContext;
    policies: ClientPolicy[];
    data: TData;
}

/**
 * AuthZyin generic user context definition (part of AuthZyinContext)
 */
export interface UserContext {
    userId: string;
    userName: string;
    tenantId: string;
    roles: string[];
}

/**
 * AuthZyin client policy definition. Serialized from server and will be used in client authorization.
 */
export interface ClientPolicy {
    name: string;
    requirements: Requirement[];
}
