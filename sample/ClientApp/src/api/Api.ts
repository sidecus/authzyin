import { acquireTokenAsync } from './MsalClient';
import { AuthZyinContextApiUrl, AuthZyinContext } from '../authzyin/AuthZyinContext';
import { Resource } from '../authzyin/Resource';

const barInfoApi = '/api/bars';
const enterBarApi = '/api/enterbar';
const buyDrinkApi = '/api/buydrink';

const callHttpGetAsync = async <T>(url: string):Promise<T> => {
    const tokenResponse = await acquireTokenAsync();
    const response = await fetch(url, {
        headers: {
            Authorization: `Bearer ${tokenResponse.accessToken}`
        }
    });

    if (response.ok) {
        return await response.json() as T;
    } else {
        // Throw error with HTTP status code
        throw new Error(response.status.toString());
    }
};

/* ====================== Api definition =============================*/

export const callAuthZyinClientContextAsync = async () => {
    return await callHttpGetAsync<SampleClientContext>(AuthZyinContextApiUrl);
}

export const callGetBarInfo = async () => {
    return await callHttpGetAsync<Bar[]>(barInfoApi);
}

export const callEnterBarApiAsync = async (id: number) => {
    const uri = enterBarApi + '/' + id;
    return await callHttpGetAsync<Bar>(uri);
};

export const callBuyDrinkAsync = async () => {
    return await callHttpGetAsync<boolean>(buyDrinkApi);
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
export interface Bar extends Resource
{
    id: number;
    name: string;
    acceptedPaymentMethods: string[];
}

