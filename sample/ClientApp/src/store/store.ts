import { createStore, combineReducers, applyMiddleware } from 'redux';
import { createSlicedReducer } from 'roth.js';
import { composeWithDevTools } from 'redux-devtools-extension';
import thunk from 'redux-thunk';
import { SampleActions, SetSignInInfoAction, SetAuthZyinContextAction } from './actions';
import { ClientContext } from '../authzyin/ClientContext';

export interface SignInInfo {
    success: boolean;
    signInError: string;
}

export interface PaymentMethod{
    type: string;
    credit: number;
}

export interface PersonalData {
    age: number;
    paymentMethods: PaymentMethod[];
}

export type SampleClientContext = ClientContext<PersonalData>;

/**
 * SetSignInfo reducer
 */
const setSignInInfoReducer = (state: SignInInfo, action: SetSignInInfoAction) => {
    return {...state, ...action.payload};
};

/**
 * SetAuthZyinContext reducer
 */
const setAuthZyinContextReducer = (state: SampleClientContext, action: SetAuthZyinContextAction) => {
    return {...state, ...action.payload};
};

/**
 * SignInInfo reducer
 * @param state signin info state
 * @param action signin info action to dispatch
 */
const signInInfoReducer = createSlicedReducer({} as SignInInfo, {
    [SampleActions.SetSignInInfo]: [setSignInInfoReducer],
});

/**
 * AuthZyin context reducer
 * @param state AuthZyin context state
 * @param action set AuthZyin context action to dispatch
 */
const authZyinContextReducer = createSlicedReducer({} as SampleClientContext, {
    [SampleActions.SetAuthZyinContext]: [setAuthZyinContextReducer],
});

/**
 * todo app root reducer
 */
const rootReducer = combineReducers({signinInfo: signInInfoReducer, authZyinContext: authZyinContextReducer});

/**
 * todo app store, created with thunk middleware and redux dev tools
 */
export const sampleStore = createStore(rootReducer, composeWithDevTools(applyMiddleware(thunk)));

/**
 * Todo app store type
 */
export type ISampleStore = ReturnType<typeof rootReducer>;