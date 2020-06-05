import { acquireTokenAsync } from './MsalClient';
import { policyApi, AuthZyinClientData } from '../authzyin/AuthZyinClientData';

const userApi = '/api/user';
const adminApi = '/api/admin';

export interface AuthNResult {
    forRole: string;
    message: string;
}

interface Membership {
    adminOf: {
        regionId: number;
        departmentId: number;
    }[];
}

export type AuthClientData = AuthZyinClientData<Membership>;

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

export const getAuthClientDataAsync = async () => {
    return await callApiAsync<AuthClientData>(policyApi);
}