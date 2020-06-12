import { Place } from "../api/Contract";

/* ====================== State definition =============================*/
export interface SignInState {
    success: boolean;
    signInError: string;
}

export interface PlaceState {
    places: Place[];
    currentPlace: number;
    sneakIn: boolean;
}

export enum Severity {
    Info = 'info',
    Error = 'error',
}

export interface AlertState {
    severity: Severity,
    message: string,
}

