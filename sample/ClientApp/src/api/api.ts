import { acquireTokenAsync } from './MsalClient';
import { AuthZyinContextApiUrl } from '../authzyin/AuthZyinContext';
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

export const callEnterBarApiAsync = async (id: number) => {
    const uri = enterBarApi + '/' + id;
    return await callApiAsync<boolean>(uri);
};

export const callBuyDrinkAsync = async () => {
    return await callApiAsync<boolean>(buyDrinkApi);
};

export const callAuthZyinClientContextAsync = async () => {
    return await callApiAsync<SampleClientContext>(AuthZyinContextApiUrl);
}