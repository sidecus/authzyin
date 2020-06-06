import { ISampleStore } from "./store";

// Use reselect.js for memoization and define all selectors outside of the function components.
// If they are defined within the fuinction component it can potentially defeat the purpose of memoization,

/**
 * signInInfo selector
 * @param store app store
 */
export const getSignInInfo = (store: ISampleStore) => store.signinInfo;

/**
 * AuthZyin context selector
 * @param store app store
 */
export const getAuthZyinContext = (store: ISampleStore) => store.authZyinContext;