import * as React from 'react';
import { useState, useEffect, PropsWithChildren } from 'react';
import { AuthZyinContext } from './AuthZyinContext';

/**
 * AuthZyin context api URL - this is setup by the AutZyin library automatically for you
 */
const contextApiUrl = '/authzyin/context';

/**
 * Global reference of the authorization React context object
 */
export let authZyinReactContext: React.Context<any>;

/*
 * Function to initialize the authorization React context - similar as createStore from redux.
 * Call this in your app startup (e.g. index.tsx)
 */
export const initializeAuthZyinContext = <TData extends object>() => {
  if (authZyinReactContext) {
    throw new Error('Authorization context is already initialized.')
  }

  authZyinReactContext = React.createContext<AuthZyinContext<TData>>(undefined!);
}

/*
 * Read AuthZyinContext object (e.g. accessing basic user info or policy info)
 */
export const useAuthZyinContext = <TData extends object>() => {
  const reactContext = authZyinReactContext as React.Context<AuthZyinContext<TData>>;
  if (!reactContext) {
    throw new Error(
      'AuthZyin authorization React context is not setup. Have you called createAuthZyinContext?'
    );
  };

  const context = React.useContext(reactContext);
  return context;
}

/**
 * Options interface for AuthZyinProvider
 */
export interface IAuthZyinProviderOptions {
    /**
     * root URL for the context api call.
     * Defaul: root of current domain. 
     */
    rootUrl: string;

    /**
     * Function to initialize requestInit for the context api fetch call.
     * Use this to set authorization headers.
     */
    requestInitFn: () => Promise<RequestInit>;

    /**
     * Automatically convert property names in requirement JsonPath to camel case.
     * If you are using Pascal case propery names in your C# authorization data definition and your asp.net core configuration
     * converts them to Camel case, you'll want to set this to true.
     * If you are serializing as Pascal case to the client, then set this to false.
     * Defaults to true since this the most common case.
     */
    autoJPathCamelCase: boolean;
};

const defaultOptions: Partial<IAuthZyinProviderOptions> = {
    rootUrl: '/',
    requestInitFn: () => Promise.resolve({}),
    autoJPathCamelCase: true,
};

/*
 * HOC component which sets the authorization React context - similar as Provider from redux
 */
export const AuthZyinProvider = <TData extends object>(
    props: PropsWithChildren<{ options: Partial<IAuthZyinProviderOptions> }>
): JSX.Element => {
    if (!authZyinReactContext) {
        throw new Error(
            'AuthZyin react context is not initialized. Call initializeAuthZyinContext during app start up first.'
        );
    }

    const [context, setContext] = useState<AuthZyinContext<TData>>({} as AuthZyinContext<TData>);
    const options = { ...defaultOptions, ...props.options } as IAuthZyinProviderOptions;

    // Use effect to trigger the api call to load AuthZyinContext data from server
    useEffect(() => {
        const loadContext = async (): Promise<void> => {
            const url = options.rootUrl.replace(/\/$/, "") + contextApiUrl;
            const request = await options.requestInitFn();
            request.method = 'GET';
            request.body = undefined;

            // Call the context api to get the context data
            const response = await fetch(url, request);
            if (response.ok) {
                const newContext = await response.json() as AuthZyinContext<TData>;
                setContext(newContext);
            } else {
                throw new Error(
                    `AuthZyinContext loading error: ${response.status}`
                );
            }
        };

        // load context data from authzyin context api
        loadContext();
    }, [props]);

    return (
        <authZyinReactContext.Provider value={context}>
            {context && context.userContext && props.children}
        </authZyinReactContext.Provider>
    )
}
