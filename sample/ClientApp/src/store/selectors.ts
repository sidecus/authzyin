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
 * places selector
 */
export const placesStateSelector = (store: ISampleStore) => store.places;

/**
 * Places selector
 */
export const placesSelector = createSelector(
    [placesStateSelector],
    x => x.places
);

/**
 * Current place selector
 */
export const currentPlaceSelector = createSelector(
    [placesStateSelector],
    x => x.currentPlace
);

/**
 * Sneak in selector
 */
export const sneakInSelector = createSelector(
    [placesStateSelector],
    x => x.sneakIn
);

/**
 * Alert selector selector
 */
export const alertSelector = (store: ISampleStore) => store.alertInfo;