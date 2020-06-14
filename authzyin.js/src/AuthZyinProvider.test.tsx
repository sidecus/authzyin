import { enableFetchMocks } from 'jest-fetch-mock';
enableFetchMocks()

import fetchMock from "jest-fetch-mock";
import * as React from 'react';
import { initializeAuthZyinContext, useAuthZyinContext, resetAuthZyinContext, AuthZyinProvider, AuthZyinProviderOptions } from './AuthZyinProvider';
import { AuthZyinContext } from './AuthZyinContext';
import { renderHook } from '@testing-library/react-hooks';
import { JsonPathConstantRequiremet, OperatorType, Direction } from './Requirements';

const DummyRequirement = {
    operator: OperatorType.Equals,
    direction: Direction.ResourceToContext,
    dataJPath: '$.DataNested[@.length-1].Value',
    resourceJPath: '$.Value',
    constValue: 3
};
const DummyPolicy = {
    name: 'dummyPolicy',
    requirements: [DummyRequirement]
};
const DummyUserContext = {
    userId: '1',
    userName: 'test',
    tenantId: '2',
    roles: []
};
const DummyContext = {
    name: 'dummy context',
    userContext: DummyUserContext,
    policies: [DummyPolicy],
    data: {}
} as AuthZyinContext;
const DummyContextJsonString = JSON.stringify(DummyContext);

// Generate a dummy context instance with new requirement instances
const getNewDummyContextInstance = () => {
    return {
        ...DummyContext,
        policies: [{
            ...DummyPolicy,
            requirements: [{
                ...DummyRequirement,
            }]
        }]
    };
};

// Get a function component with given default value and options, and wrapping the children with AuthZyinProvider
export const getAuthZyinContextWrapper = (
    contextValue: AuthZyinContext | undefined,
    options: Partial<AuthZyinProviderOptions>
) => ({children}: any) => {
    resetAuthZyinContext();
    initializeAuthZyinContext(contextValue);

    return (
        <AuthZyinProvider options={options}>{children}</AuthZyinProvider>
    );
};

describe('initializeAuthZyinContext/useAuthZyinContext', () => {
    // initializeAuthZyinContext and useAuthZyinContext uses the passed in devaule value when not specifying AuthZyinProvider
    test.each([
        [undefined, undefined],
        [DummyContext, DummyContext],
    ])(
        'initialize & use hooks work as expected w/o AuthZyinProvider (%#)',
        (contextValue, expectedResult) => {
            // initialize for the first time leads to right result in useAuthZyinContext
            resetAuthZyinContext();
            initializeAuthZyinContext(contextValue);

            const { result } = renderHook(useAuthZyinContext);
            if (expectedResult === undefined) {
                expect(result.current).toBeUndefined();
            } else {
                expect(result.current).toBe(expectedResult);
            }

            // initialize again should cause error.
            expect(() => initializeAuthZyinContext(contextValue)).toThrowError();
        }
    );

    // AuthZyinProvider uses the devalue value if provided by initializeAuthZyinContext
    test.each([
        [true, '$.dataNested[@.length-1].value', '$.value'],
        [false, DummyRequirement.dataJPath, DummyRequirement.resourceJPath],
    ])(
        'AuthZyinProvider with provided context and options behaves correctly (%#)',
        (camelCasePathes: boolean, expectedDataPath: string, expectedResourcePath: string) => {
            const contextValue = getNewDummyContextInstance();
            const wrapper = getAuthZyinContextWrapper(contextValue, {jsonPathPropToCamelCase: camelCasePathes});
            const { result } = renderHook(useAuthZyinContext, { wrapper });
            expect(!result.error).toBeTruthy();
            expect(result.current).toBe(contextValue);
            const resultRequirement = result.current.policies[0].requirements[0] as JsonPathConstantRequiremet;
            expect(resultRequirement.dataJPath).toBe(expectedDataPath);
            expect(resultRequirement.resourceJPath).toBe(expectedResourcePath);
            expect(resultRequirement.constValue).toBe(DummyRequirement.constValue);
        }
    );

    // AuthZyinProvider makes proper api call to get context if a devault value is not provided.
    test.each([
        [ { jsonPathPropToCamelCase: true }, '$.dataNested[@.length-1].value', '$.value'],
        [ { url: 'https://someserver.com/abc', jsonPathPropToCamelCase: false }, DummyRequirement.dataJPath, DummyRequirement.resourceJPath],
    ])(
        'AuthZyinProvider makes proper api call (%#)',
        async (options: Partial<AuthZyinProviderOptions>, expectedDataPath: string, expectedResourcePath: string) => {
            const urlPredicator = !options.url ? /^\/authzyin\/context$/ : options.url;
            fetchMock.mockIf(urlPredicator, () => {
                return Promise.resolve(DummyContextJsonString);
            });

            const wrapper = getAuthZyinContextWrapper(undefined, options);

            const { result, waitForNextUpdate } = renderHook(useAuthZyinContext, { wrapper });
            await waitForNextUpdate();

            expect(!result.error).toBeTruthy();
            expect(result.current.policies.length).toBe(1);
            expect(result.current.policies[0].name).toBe(DummyPolicy.name);
            expect(result.current.policies[0].requirements.length).toBe(DummyPolicy.requirements.length);
            const resultRequirement = result.current.policies[0].requirements[0] as JsonPathConstantRequiremet;
            expect(resultRequirement.operator).toBe(DummyRequirement.operator);
            expect(resultRequirement.direction).toBe(DummyRequirement.direction);
            expect(resultRequirement.dataJPath).toBe(expectedDataPath);
            expect(resultRequirement.resourceJPath).toBe(expectedResourcePath);
            expect(resultRequirement.constValue).toBe(DummyRequirement.constValue);
        }
    );
});