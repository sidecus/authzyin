import { createSelector } from 'reselect';
import { ISampleStore } from './store';

// Use reselect.js for memoization and define all selectors outside of the function components.
// If they are defined within the fuinction component it can potentially defeat the purpose of memoization,

/**
 * signInInfo selector
 */
export const signInfoSelector = (store: ISampleStore) => store.signinInfo;

/**
 * AuthZyin context selector
 */
export const authZyinContextSelector = (store: ISampleStore) => store.authZyinContext;


/**
 * user info selector
 */
export const userInfoSelector = createSelector(
    [authZyinContextSelector],
    x => x.userContext
);

/**
 * Bar info selector
 */
export const barStateSelector = (store: ISampleStore) => store.barState;

/**
 * Bars selector
 */
export const barsSelector = createSelector(
    [barStateSelector],
    x => x.bars
);

/**
 * Current bars selector
 */
export const currentBarSelector = createSelector(
    [barStateSelector],
    x => x.currentBar
);