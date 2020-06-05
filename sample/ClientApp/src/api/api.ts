import { acquireTokenAsync } from '../auth/MsalClient';

const userApi = '/api/user';
const adminApi = '/api/admin';
const policyApi = '/authzyin/policies';

export interface AuthNResult {
    forRole: string;
    message: string;
}

export interface AuthPolicy {
    name: string;
    requirements: string[];
}

export interface Department {
    regionId: number;
    departmentId: number;
}

export interface Membership {
    admins: Department[];
}

export interface AuthClientData {
    userId: string;
    userName: string;
    tenantId: string;
    roles: string[];
    policies: AuthPolicy[];
    customData: Membership;
}

const callApiAsync = async <T>(url: string):Promise<T> => {
    const tokenResponse = await acquireTokenAsync();
    const response = await fetch(url, {
        headers: {
            Authorization: `Bearer ${tokenResponse.accessToken}`
        }
    });

    return response.json();
};

export const getUserAsync = async () => {
    const ret = await callApiAsync<AuthNResult>(userApi);
    return ret;
};

export const getAdminAsync = async () => {
    return await callApiAsync<AuthNResult>(adminApi);
};

export const getPolicies = async () => {
    return await callApiAsync<AuthClientData>(policyApi);
}