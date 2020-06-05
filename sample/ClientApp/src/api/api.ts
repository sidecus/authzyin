import { acquireTokenAsync } from './MsalClient';
import { contextApiUrl, ClientContext } from '../authzyin/ClientContext';

const userApi = '/api/user';
const adminApi = '/api/admin';

const callApiAsync = async <T>(url: string):Promise<T> => {
    const tokenResponse = await acquireTokenAsync();
    const response = await fetch(url, {
        headers: {
            Authorization: `Bearer ${tokenResponse.accessToken}`
        }
    });

    return response.json();
};

interface AuthNResult {
    forRole: string;
    message: string;
}

interface Membership {
    adminOf: {
        regionId: number;
        departmentId: number;
    }[];
}

export type SampleClientContext = ClientContext<Membership>;


export const getUserAsync = async () => {
    const ret = await callApiAsync<AuthNResult>(userApi);
    return ret;
};

export const getAdminAsync = async () => {
    return await callApiAsync<AuthNResult>(adminApi);
};

export const GetAuthZyinClientContextAsync = async () => {
    return await callApiAsync<SampleClientContext>(contextApiUrl);
}