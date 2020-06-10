import { acquireTokenAsync } from './MsalClient';
import { AuthZyinContextApiUrl, AuthZyinContext } from '../authzyin/AuthZyinContext';
import { Resource } from '../authzyin/Resource';

const placesApi = '/api/places';
const enterPlaceApi = '/api/enterplace';
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
}

/* ====================== Api definition =============================*/

export const callAuthZyinClientContextAsync = async () => {
    return await callHttpGetAsync<SampleClientContext>(AuthZyinContextApiUrl);
}

export const callGetPlaces = async () => {
    return await callHttpGetAsync<Place[]>(placesApi);
}

export const callEnterPlaceApiAsync = async (id: number) => {
    const uri = enterPlaceApi + '/' + id;
    return await callHttpGetAsync<Place>(uri);
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
export interface Place extends Resource
{
    id: number;
    name: string;
    acceptedPaymentMethods: string[];
    policy: string;
}

export interface Bar extends Place
{
    hasHappyHour: boolean;
}

export interface AgeLimitedPlace extends Place
{
    minAge: number;
    maxAge: number;
}

/* ==================== Contract type guards =======================*/

export function IsBar(place: Place): place is Bar {
    return (place as Bar).hasHappyHour !== undefined;
}

export function IsAgeLimitedPlace(place: Place): place is AgeLimitedPlace {
    return (place as AgeLimitedPlace).maxAge !== undefined;
}