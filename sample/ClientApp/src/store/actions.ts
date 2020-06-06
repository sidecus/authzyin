import { Dispatch } from 'redux';
import { SignInInfo, SampleClientContext } from './store';
import { callEnterBarApiAsync, callAuthZyinClientContextAsync } from '../api/Api';
import { createActionCreator, useMemoizedBoundActionCreators } from 'roth.js';
import { signInAsync } from '../api/MsalClient';

/* action type string enums */

/**
 * Sample app action types
 */
export enum SampleActions {
    SetSignInInfo = 'SetSignInInfo',
    SetAuthZyinContext = 'SetAuthZyinContext',
    EnterBar = 'EnterBar',
    BuyDrink = 'BuyDrink',
}

/**
 * set sign info action creator
 */
const setSignInInfo = createActionCreator<SignInInfo>(SampleActions.SetSignInInfo);
export type SetSignInInfoAction = ReturnType<typeof setSignInInfo>

/**
 * set authzyin context action creator
 */
const setAuthZyinContext = createActionCreator<SampleClientContext>(SampleActions.SetAuthZyinContext);
export type SetAuthZyinContextAction = ReturnType<typeof setAuthZyinContext>

/* thunk action creators */

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
    }
};


/* named dispatchers (bound action creators) */
const namedActionCreators = {
    signIn: signIn,
}

/**
 * Custom hooks for dispatchers (bound action creators).
 * You can create one of this for each domain area to logically separate the dispatchers.
 */
export const useSampleAppBoundActionCreators = () => useMemoizedBoundActionCreators(namedActionCreators);
