export const policyApi = '/authzyin/policies';

/* Definitions here maps to types defined in AuthZyin.Authorization.Client */
/* We can also consider auto ts generation from C# */

export interface AuthZyinClientPolicy {
    name: string;
    requirements: string[];
}

export interface AuthZyinUserContext {
    userId: string;
    userName: string;
    tenantId: string;
    roles: string[];
}

export interface AuthZyinClientData<TCustomData> {
    userContext: AuthZyinUserContext;
    policies: AuthZyinClientPolicy[];
    customData: TCustomData;
}
