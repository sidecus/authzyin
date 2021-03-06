import { createStore, combineReducers, applyMiddleware } from 'redux';
import { createSlicedReducer } from 'roth.js';
import { composeWithDevTools } from 'redux-devtools-extension';
import thunk from 'redux-thunk';
import {
    SampleActions,
    SetSignInInfoAction,
    SetAuthZyinContextAction,
    SetPlacesAction,
    SetCurrentPlaceAction,
    SetAlertAction,
    SetSneakInAction
} from './actions';
import { SampleAuthZyinContext } from '../api/Contract';
import { SignInState, PlaceState, Severity, AlertState } from './state';

/* ====================== Reducer definition =============================*/

/**
 * SignInInfo reducer
 */
const signInStateReducer = createSlicedReducer({} as SignInState, {
    [SampleActions.SetSignInInfo]: [
        (state: SignInState, action: SetSignInInfoAction) => {
            return {...state, ...action.payload};
        }
    ],
});

/**
 * AuthZyin context reducer
 */
const authZyinContextReducer = createSlicedReducer({} as SampleAuthZyinContext, {
    [SampleActions.SetAuthZyinContext]: [
        (state: SampleAuthZyinContext, action: SetAuthZyinContextAction) => {
            return {...state, ...action.payload};
        }
    ],
});

/**
 * place state reducer
 */
const placeStateReducer = createSlicedReducer({currentPlace: -1, sneakIn: false} as PlaceState, {
    [SampleActions.SetPlaces]: [
        (state: PlaceState, action: SetPlacesAction) => {
            return {...state, places: action.payload, currentPlace: -1};
        }
    ],
    [SampleActions.SetCurrentPlace]: [
        (state: PlaceState, action: SetCurrentPlaceAction) => {
            return {...state, currentPlace: action.payload};
        }
    ],
    [SampleActions.SetSneakIn]: [
        (state: PlaceState, action: SetSneakInAction) => {
            return {...state, sneakIn: action.payload};
        }
    ],
});

/**
 * Alert reducer
 */
const alertReducer = createSlicedReducer({severity: Severity.Info, message: 'Which place do you want to go?'} as AlertState, {
    [SampleActions.SetAlert]: [
        (state: AlertState, action: SetAlertAction) => {
            return {...state, ...action.payload};
        }
    ],
});

/**
 * root reducer
 */
const rootReducer = combineReducers({
    signinInfo: signInStateReducer,
    authZyinContext: authZyinContextReducer,
    places: placeStateReducer,
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