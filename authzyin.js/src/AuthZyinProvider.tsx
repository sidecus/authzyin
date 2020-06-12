import * as React from 'react';
import { useState, useEffect, PropsWithChildren } from 'react';
import { AuthZyinContext } from './AuthZyinContext';

/*
 * AuthZyin context api URL - this is setup by the AutZyin library automatically for you
 */
const AuthZyinContextApiUrl = '/authzyin/context';

/*
 * HOC component which sets the authorization React context - similar as Provider from redux
 */
export const AuthZyinProvider = <TData extends object>(
    props: PropsWithChildren<{
        context: React.Context<AuthZyinContext<TData>>;
        requestInitFn: () => Promise<RequestInit>;
    }>
): JSX.Element => {
    const [context, setContext] = useState<AuthZyinContext<TData>>(
        {} as AuthZyinContext<TData>
    );

    useEffect(() => {
        const loadContext = async (): Promise<void> => {
            const requestInit = await props.requestInitFn();
            requestInit.method = 'GET';
            requestInit.body = undefined;

            const response = await fetch(AuthZyinContextApiUrl, requestInit);
            if (response.ok) {
                const result = await response.json();
                const newContext = result as AuthZyinContext<TData>;
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
        <props.context.Provider value={context}>
            {context && context.userContext && props.children}
        </props.context.Provider>
    )
}
