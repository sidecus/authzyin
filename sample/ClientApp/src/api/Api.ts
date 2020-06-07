import { acquireTokenAsync } from './MsalClient';
import { AuthZyinContextApiUrl, AuthZyinContext } from '../authzyin/AuthZyinContext';

const barInfoApi = '/api/bars';
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

/* ====================== Api definition =============================*/

export const callAuthZyinClientContextAsync = async () => {
    return await callApiAsync<SampleClientContext>(AuthZyinContextApiUrl);
}

export const callGetBarInfo = async () => {
    return await callApiAsync<Bar[]>(barInfoApi);
}

export const callEnterBarApiAsync = async (id: number) => {
    const uri = enterBarApi + '/' + id;
    return await callApiAsync<Bar>(uri);
};

export const callBuyDrinkAsync = async () => {
    return await callApiAsync<boolean>(buyDrinkApi);
};

/* ====================== Api contract definition =============================*/
export interface PaymentMethod{
    type: string;
    credit: number;
}

export interface AuthorizationData {
    age: number;
    withDriversLicense: boolean;
    withPassport: boolean;
    paymentMethods: PaymentMethod[];
}

export type SampleClientContext = AuthZyinContext<AuthorizationData>;

/* ====================== Resource definition =============================*/
export interface Bar
{
    id: number;
    name: string;
    acceptedPaymentMethods: string[];
}

