import { createStore, combineReducers, applyMiddleware } from 'redux';
import { createSlicedReducer } from 'roth.js';
import { composeWithDevTools } from 'redux-devtools-extension';
import thunk from 'redux-thunk';
import { SampleActions, SetSignInInfoAction, SetAuthZyinContextAction, SetBarsAction, SetCurrentBarAction, SetAlertAction } from './actions';
import { SampleClientContext, Bar } from '../api/Api';

/* ====================== State definition =============================*/
export interface SignInState {
    success: boolean;
    signInError: string;
}

export interface BarState {
    bars: Bar[];
    currentBar: number;
    barError: string;
}

export enum Severity {
    Info = 'info',
    Error = 'error',
}

export interface AlertState {
    severity: Severity,
    message: string,
}

/* ====================== Reducer definition =============================*/

/**
 * SetSignInfo reducer
 */
const setSignInInfoReducer = (state: SignInState, action: SetSignInInfoAction) => {
    return {...state, ...action.payload};
};

/**
 * SetAuthZyinContext reducer
 */
const setAuthZyinContextReducer = (state: SampleClientContext, action: SetAuthZyinContextAction) => {
    return {...state, ...action.payload};
};

/**
 * set bar info reducer
 */
const setBarsReducer = (state: BarState, action: SetBarsAction) => {
    return {...state, bars: action.payload, currentBar: -1};
}

/**
 * set current bar reducer
 */
const setCurrentBarReducer = (state: BarState, action: SetCurrentBarAction) => {
    return {...state, currentBar: action.payload};
}

/**
 * set alert reducer
 */
const setAlertReducer = (state: AlertState, action: SetAlertAction) => {
    return {...state, ...action.payload};
}

/**
 * SignInInfo reducer
 */
const signInStateReducer = createSlicedReducer({} as SignInState, {
    [SampleActions.SetSignInInfo]: [setSignInInfoReducer],
});

/**
 * AuthZyin context reducer
 */
const authZyinContextReducer = createSlicedReducer({} as SampleClientContext, {
    [SampleActions.SetAuthZyinContext]: [setAuthZyinContextReducer],
});

/**
 * bar info reducer,handles two actions
 */
const barStateReducer = createSlicedReducer({currentBar: -1} as BarState, {
    [SampleActions.SetBars]: [setBarsReducer],
    [SampleActions.SetCurrentBar]: [setCurrentBarReducer],
});

/**
 * Alert reducer
 */
const alertReducer = createSlicedReducer({severity: Severity.Info, message: 'Which bar do you want to go?'} as AlertState, {
    [SampleActions.SetAlert]: [setAlertReducer],
});

/**
 * root reducer
 */
const rootReducer = combineReducers({
    signinInfo: signInStateReducer,
    authZyinContext: authZyinContextReducer,
    barInfo: barStateReducer,
    alertInfo: alertReducer,
});

/**
 * store, created with thunk middleware and redux dev tools
 */
export const sampleStore = createStore(rootReducer, composeWithDevTools(applyMiddleware(thunk)));

/**
 * app store type
 */
export type ISampleStore = ReturnType<typeof rootReducer>;