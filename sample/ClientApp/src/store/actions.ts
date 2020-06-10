import { Dispatch } from 'redux';
import { SignInState, AlertState, Severity } from './store';
import { callEnterPlaceApiAsync, callAuthZyinClientContextAsync, SampleClientContext, Place, callGetPlaces } from '../api/Api';
import { createActionCreator, useMemoizedBoundActionCreators } from 'roth.js';
import { signInAsync } from '../api/MsalClient';

/* action type string enums */

/**
 * Sample app action types
 */
export enum SampleActions {
    SetSignInInfo = 'SetSignInInfo',
    SetAuthZyinContext = 'SetAuthZyinContext',
    SetPlaces = "SetPlaces",
    SetCurrentPlace = "SetCurrentPlace",
    SetSneakIn = "SetSneakIn",
    SetAlert = "SetAlert",
    BuyDrink = 'BuyDrink',
}

/**
 * set sign info action creator
 */
const setSignInInfo = createActionCreator<SignInState>(SampleActions.SetSignInInfo);
export type SetSignInInfoAction = ReturnType<typeof setSignInInfo>

/**
 * set authzyin context action creator
 */
const setAuthZyinContext = createActionCreator<SampleClientContext>(SampleActions.SetAuthZyinContext);
export type SetAuthZyinContextAction = ReturnType<typeof setAuthZyinContext>

/**
 * set places info action creator
 */
const setPlaces = createActionCreator<Place[]>(SampleActions.SetPlaces);
export type SetPlacesAction = ReturnType<typeof setPlaces>


/**
 * set current place action creator
 */
const setCurrentPlace = createActionCreator<number>(SampleActions.SetCurrentPlace);
export type SetCurrentPlaceAction = ReturnType<typeof setCurrentPlace>

/**
 * set sneak in action creator
 */
const setSneakIn = createActionCreator<boolean>(SampleActions.SetSneakIn);
export type SetSneakInAction = ReturnType<typeof setSneakIn>

/**
 * set error
 */
const setAlert = createActionCreator<AlertState>(SampleActions.SetAlert);
export type SetAlertAction = ReturnType<typeof setAlert>


/* ===============================thunk action creators============================ */

/**
 * This is a thunk action creator used to trigger sign in
 */
const signIn = () => {
    return async (dispatch: Dispatch<any>) => {
        let signInInfo = { success: false, signInError: '' };

        try
        {
            const account = await signInAsync();
            signInInfo.success = true;
            console.log(account);
        } catch(error) {
            signInInfo.success = false;
            signInInfo.signInError = error.message;
        }

        // set sign in info
        dispatch(setSignInInfo(signInInfo));

        // if sign in is successful, trigger authorization context loading
        if (signInInfo.success) {
            dispatch(getAuthZyinContext());
        }
    }
};

/**
 * This is a thunk action creator used to get authorization context
 */
const getAuthZyinContext = () => {
    return async (dispatch: Dispatch<any>) => {
        const context = await callAuthZyinClientContextAsync();
        dispatch(setAuthZyinContext(context));
        dispatch(getPlaces());
    }
};

/**
 * This is a thunk action creator used to get authorization context
 */
const getPlaces = () => {
    return async (dispatch: Dispatch<any>) => {
        const places = await callGetPlaces();
        dispatch(setPlaces(places));
    }
};

/**
 * This is a thunk action creator used to call enter a place
 */
const enterPlace = (id: number) => {
    return async (dispatch: Dispatch<any>) => {
        try {
            const ret = await callEnterPlaceApiAsync(id);
            console.log(`Entered place ${id}: server call succeeded`);
            dispatch(setCurrentPlace(ret.id))
            dispatch(setAlert({
                severity: Severity.Info,
                message: `You just walked into: ${ret.name}`,
            }));
        } catch (error) {
            console.error(`Server authorization failed for place ${id}: ${error.message}`);
            dispatch(setAlert({
                severity: Severity.Error,
                message: `You tried to sneak into the place ${id} but got rejected by server`,
            }));
        }
    }
}

/* named dispatchers (bound action creators) */
const namedActionCreators = {
    signIn: signIn,
    getAuthZyinContext: getAuthZyinContext,
    getPlaces: getPlaces,
    setAlert: setAlert,
    setCurrentPlace: setCurrentPlace,
    setSneakIn: setSneakIn,
    enterPlace: enterPlace,
}

/**
 * Custom hooks for dispatchers (bound action creators).
 * You can create one of this for each domain area to logically separate the dispatchers.
 */
export const useSampleAppBoundActionCreators = () => useMemoizedBoundActionCreators(namedActionCreators);
