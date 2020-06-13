import * as React from 'react';
import { useState, useEffect, PropsWithChildren } from 'react';
import { AuthZyinContext, ClientPolicy } from './AuthZyinContext';
import {
    Requirement,
    IsOrRequirement,
    IsJsonPathRequirement
} from './Requirements';

/**
 * AuthZyin context api URL - this is setup by the AutZyin library automatically for you
 */
const contextApiUrl = '/authzyin/context';

/**
 * Global reference of the authorization React context object
 */
// eslint-disable-next-line @typescript-eslint/no-explicit-any
export let authZyinReactContext: React.Context<any>;

/*
 * Function to initialize the authorization React context - similar as createStore from redux.
 * Call this in your app startup (e.g. index.tsx)
 */
export const initializeAuthZyinContext = <TData extends object>() => {
    if (authZyinReactContext) {
        throw new Error('Authorization context is already initialized.');
    }

    authZyinReactContext = React.createContext<AuthZyinContext<TData>>(
        // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
        undefined!
    );
};

/*
 * Read AuthZyinContext object (e.g. accessing basic user info or policy info)
 */
export const useAuthZyinContext = <TData extends object>() => {
    const reactContext = authZyinReactContext as React.Context<
        AuthZyinContext<TData>
    >;
    if (!reactContext) {
        throw new Error(
            'AuthZyin authorization React context is not setup. Have you called createAuthZyinContext?'
        );
    }

    const context = React.useContext(reactContext);
    return context;
};

/**
 * Options interface for AuthZyinProvider
 */
export interface AuthZyinProviderOptions {
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
    jsonPathPropToCamelCase: boolean;
}

const defaultOptions: Partial<AuthZyinProviderOptions> = {
    rootUrl: '/',
    requestInitFn: () => Promise.resolve({}),
    jsonPathPropToCamelCase: true
};

/**
 * Convert property names in a json path to camel case. e.g., "$.SomeData.Value" to "$.someData.value"
 * @param path json path
 */
export const camelCasePropertyNamesInJsonPath = (path: string) => {
    // TODO[sidecus] - more performant way to ignore case in JPath
    let newPath = '';
    let isPreviousCharADot = false;
    for (let i = 0; i < path.length; i++) {
        newPath += isPreviousCharADot ? path[i].toLowerCase() : path[i];
        isPreviousCharADot = path[i] === '.';
    }

    return newPath;
};

export const camelCasePropertyNamesInJsonPathForRequirement = (
    requirement: Requirement
) => {
    if (IsOrRequirement(requirement) && requirement.children) {
        for (let i = 0; i < requirement.children.length; i++) {
            camelCasePropertyNamesInJsonPathForRequirement(
                requirement.children[i]
            );
        }
    } else if (IsJsonPathRequirement(requirement)) {
        requirement.dataJPath = camelCasePropertyNamesInJsonPath(
            requirement.dataJPath
        );
        requirement.resourceJPath = camelCasePropertyNamesInJsonPath(
            requirement.resourceJPath
        );
    }
};

export const camelCasePropertyNamesInJsonPathForPolicy = (
    policy: ClientPolicy
) => {
    if (policy && policy.requirements && policy.requirements.length > 0) {
        for (let i = 0; i < policy.requirements.length; i++) {
            camelCasePropertyNamesInJsonPathForRequirement(
                policy.requirements[i]
            );
        }
    }
};

export const camelCasePropertyNamesInJsonPathForContext = <
    TData extends object
>(
    context: AuthZyinContext<TData>
) => {
    if (context && context.policies) {
        for (let i = 0; i < context.policies.length; i++) {
            camelCasePropertyNamesInJsonPathForPolicy(context.policies[i]);
        }
    }
};

/*
 * HOC component which sets the authorization React context - similar as Provider from redux
 */
export const AuthZyinProvider = <TData extends object>(
    props: PropsWithChildren<{ options: Partial<AuthZyinProviderOptions> }>
): JSX.Element => {
    if (!authZyinReactContext) {
        throw new Error(
            'AuthZyin react context is not initialized. Call initializeAuthZyinContext during app start up first.'
        );
    }

    const [context, setContext] = useState<AuthZyinContext<TData>>(
        {} as AuthZyinContext<TData>
    );
    const options = {
        ...defaultOptions,
        ...props.options
    } as AuthZyinProviderOptions;

    // Use effect to trigger the api call to load AuthZyinContext data from server
    useEffect(() => {
        const loadContext = async (): Promise<void> => {
            const url = options.rootUrl.replace(/\/$/, '') + contextApiUrl;
            const request = await options.requestInitFn();
            request.method = 'GET';
            request.body = undefined;

            // Call the context api to get the context data
            const response = await fetch(url, request);
            if (response.ok) {
                const newContext = (await response.json()) as AuthZyinContext<
                    TData
                >;
                if (options.jsonPathPropToCamelCase) {
                    // Convert property names in JSON path to camel case
                    camelCasePropertyNamesInJsonPathForContext(newContext);
                }
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
    );
};
