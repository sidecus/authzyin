import { acquireTokenAsync } from './MsalClient';
import { contextApiUrl } from '../authzyin/ClientContext';
import { SampleClientContext } from '../store/store';

const enterBarApi = '/api/enterbar';
const buyDrinkApi = '/api/buydrink';

const callApiAsync = async <T>(url: string):Promise<T> => {
    const tokenResponse = await acquireTokenAsync();
    const response = await fetch(url, {
        headers: {
            Authorization: `Bearer ${tokenResponse.accessToken}`
        }
    });

    return response.json();
};

export const callEnterBarApiAsync = async () => {
    return await callApiAsync<boolean>(enterBarApi);
};

export const callBuyDrinkAsync = async () => {
    return await callApiAsync<boolean>(buyDrinkApi);
};

export const callAuthZyinClientContextAsync = async () => {
    return await callApiAsync<SampleClientContext>(contextApiUrl);
}