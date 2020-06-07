import { createStore, combineReducers, applyMiddleware } from 'redux';
import { createSlicedReducer } from 'roth.js';
import { composeWithDevTools } from 'redux-devtools-extension';
import thunk from 'redux-thunk';
import { SampleActions, SetSignInInfoAction, SetAuthZyinContextAction, SetBarsAction, SetCurrentBarAction } from './actions';
import { SampleClientContext, Bar } from '../api/Api';

/* ====================== State definition =============================*/
export interface SignInState {
    success: boolean;
    signInError: string;
}

export interface BarState {
    bars: Bar[];
    currentBar: number;
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
 * root reducer
 */
const rootReducer = combineReducers({
    signinInfo: signInStateReducer,
    authZyinContext: authZyinContextReducer,
    barState: barStateReducer});

/**
 * store, created with thunk middleware and redux dev tools
 */
export const sampleStore = createStore(rootReducer, composeWithDevTools(applyMiddleware(thunk)));

/**
 * Todo app store type
 */
export type ISampleStore = ReturnType<typeof rootReducer>;