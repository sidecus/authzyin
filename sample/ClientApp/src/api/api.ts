import { acquireTokenAsync } from '../auth/MsalClient';

const userApi = "/api/user";
const adminApi = "/api/admin";

export interface AuthNResult {
    forRole: string;
    message: string;
}

const callApiAsync = async <T>(url: string, accessToken: string):Promise<T> => {
    const response = await fetch(url, {
        headers: {
            Authorization: `Bearer ${accessToken}`
        }
    });

    return response.json();
};

export const getUserAsync = async () => {
    const tokenResponse = await acquireTokenAsync();
    const ret = await callApiAsync<AuthNResult>(userApi, tokenResponse.accessToken);
    return ret;
};

export const getAdminAsync = async () => {
    const tokenResponse = await acquireTokenAsync();
    return await callApiAsync<AuthNResult>(adminApi, tokenResponse.accessToken);
};