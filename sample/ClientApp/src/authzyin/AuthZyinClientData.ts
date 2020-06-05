export const policyApi = '/authzyin/policies';

export interface AuthPolicy {
    name: string;
    requirements: string[];
}

export interface AuthZyinClientData<TCustomData> {
    userId: string;
    userName: string;
    tenantId: string;
    roles: string[];
    policies: AuthPolicy[];
    customData: TCustomData;
}
