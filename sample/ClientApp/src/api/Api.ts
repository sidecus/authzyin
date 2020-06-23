import { acquireTokenAsync } from './MsalClient';
import { Place } from './Contract';

const placesApi = '/api/places';
const enterPlaceApi = '/api/enterplace';
const buyDrinkApi = '/api/buydrink';

export const getAuthorizationHeadersAsync = async () => {
    const tokenResponse = await acquireTokenAsync();
    return {
        headers: {
            Authorization: `Bearer ${tokenResponse.accessToken}`
        }
    } as RequestInit;
}

const callHttpGetAsync = async <T>(url: string):Promise<T> => {
    const requestInit = await getAuthorizationHeadersAsync();
    const response = await fetch(url, requestInit);

    if (response.ok) {
        return await response.json() as T;
    } else {
        // Throw error with HTTP status code
        throw new Error(response.status.toString());
    }
}

/* ====================== Api definition =============================*/
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