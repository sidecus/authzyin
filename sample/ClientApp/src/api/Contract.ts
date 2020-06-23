import { AuthZyinContext } from 'authzyin.js';
import { Resource } from 'authzyin.js';

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

export type SampleAuthZyinContext = AuthZyinContext<AuthorizationData>;

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