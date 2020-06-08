import { PublicClientApplication, TokenResponse } from '@azure/msal-browser';

// Config object to be passed to Msal on creation.
// For a full list of msal.js configuration parameters, 
// visit https://azuread.github.io/microsoft-authentication-library-for-js/docs/msal/modules/_authenticationparameters_.html
const msalConfig = {
    auth: {
        clientId: "1be718bf-6d75-4a33-a493-ed7e3df882de",
        authority: "https://login.microsoftonline.com/common",
        redirectUri: "https://localhost:5001/auth.html",
    },
    cache: {
        cacheLocation: "sessionStorage", // This configures where your cache will be stored
        storeAuthStateInCookie: false, // Set this to "true" if you are having issues on IE11 or Edge
    }
};

// Add here scopes for id token to be used at MS Identity Platform endpoints.
const loginParams = {
    scopes: ["openid", "profile", "User.Read"]
};

// Add here scopes for access token to be used at our own api backend
const tokenParams = {
    scopes: ["api://1be718bf-6d75-4a33-a493-ed7e3df882de/AuthZyinSample"],
    forceRefresh: false // set this to "true" if you would like to skip a cached token and go to the server
};

// msal client
export const msalClient = new PublicClientApplication(msalConfig);

// sign in popup
export const signInAsync = async () => {
    await msalClient.loginPopup(loginParams);
    return msalClient.getAccount();
}

// acquire token - this needs to be called each time before an api call is made
export const acquireTokenAsync = async () => {
    let tokenResponse: TokenResponse;
    try {
        // Try acquire silently first
        tokenResponse = await msalClient.acquireTokenSilent(tokenParams);
    } catch (error) {
        // If acquire silent fails, we might need user consent. Try to use popup.
        console.warn(error.message);
        tokenResponse = await msalClient.acquireTokenPopup(tokenParams);
    };

    return tokenResponse;
}
