import { Dispatch } from 'redux';
import { SignInState, AlertState, Severity } from './store';
import { callEnterBarApiAsync, callAuthZyinClientContextAsync, SampleClientContext, Bar, callGetBarInfo } from '../api/Api';
import { createActionCreator, useMemoizedBoundActionCreators } from 'roth.js';
import { signInAsync } from '../api/MsalClient';

/* action type string enums */

/**
 * Sample app action types
 */
export enum SampleActions {
    SetSignInInfo = 'SetSignInInfo',
    SetAuthZyinContext = 'SetAuthZyinContext',
    SetBars = "SetBarInfo",
    SetCurrentBar = "SetCurrentBar",
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
 * set bar info action creator
 */
const setBars = createActionCreator<Bar[]>(SampleActions.SetBars);
export type SetBarsAction = ReturnType<typeof setBars>


/**
 * set current bar action creator
 */
const setCurrentBar = createActionCreator<number>(SampleActions.SetCurrentBar);
export type SetCurrentBarAction = ReturnType<typeof setCurrentBar>

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
        dispatch(getBars());
    }
};

/**
 * This is a thunk action creator used to get authorization context
 */
const getBars = () => {
    return async (dispatch: Dispatch<any>) => {
        const barInfo = await callGetBarInfo();
        dispatch(setBars(barInfo));
    }
};

/**
 * This is a thunk action creator used to call enter bar
 */
const enterBar = (id: number) => {
    return async (dispatch: Dispatch<any>) => {
        try {
            const ret = await callEnterBarApiAsync(id);
            console.log(`Entered bar ${id}: server call succeeded`);
            dispatch(setCurrentBar(ret.id))
            dispatch(setAlert({
                severity: Severity.Info,
                message: `You just walked int the bar: ${ret.name}`,
            }));
        } catch (error) {
            console.error(`Server authorization failed for bar ${id}: ${error.message}`);
            dispatch(setAlert({
                severity: Severity.Error,
                message: `Entering bar ${id} rejected by server`,
            }));
        }
    }
}

/* named dispatchers (bound action creators) */
const namedActionCreators = {
    signIn: signIn,
    getAuthZyinContext: getAuthZyinContext,
    getBars: getBars,
    enterBar: enterBar,
    setCurrentBar: setCurrentBar,
    setAlert: setAlert,
}

/**
 * Custom hooks for dispatchers (bound action creators).
 * You can create one of this for each domain area to logically separate the dispatchers.
 */
export const useSampleAppBoundActionCreators = () => useMemoizedBoundActionCreators(namedActionCreators);
