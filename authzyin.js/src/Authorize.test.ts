import { authorize, useAuthorize } from './Authorize';
import { AuthZyinContext } from './AuthZyinContext';
import { OperatorType, Direction } from './Requirements';
import { getAuthZyinContextWrapper } from './AuthZyinProvider.test';
import { renderHook } from '@testing-library/react-hooks';

const roleRequirement = {
    operator: OperatorType.RequiresRole,
    allowedRoles: ['wrongRole', 'rightRole']
};

const ageGreaterThan21Requirement = {
    operator: OperatorType.GreaterThan,
    direction: Direction.ContextToResource,
    dataJPath: '$.age',
    constValue: 21,
    resourceJPath: '$.value'
};

const hasPaymentMethodRequirement = {
    operator: OperatorType.Contains,
    direction: Direction.ContextToResource,
    dataJPath: '$.paymentMethods[*]',
    resourceJPath: '$.allowedPaymentMethod'
};

const policies = [
    { name: 'candrink', requirements: [ roleRequirement, ageGreaterThan21Requirement ] },
    { name: 'canpay', requirements: [ roleRequirement, ageGreaterThan21Requirement, hasPaymentMethodRequirement ] },
]

const data = {
    age: 30,
    paymentMethods: ['cash']
};

const context = {
    userContext: {
        roles: ['someOtherRole', 'rightRole']
    },
    policies: policies,
    data: data
} as AuthZyinContext<typeof data>;

const resource = {
    allowedPaymentMethod: 'visa',
    randomeStringValue: 'random@#$@%#$%',
    resourceData: {
        intValue: 30,
        smallerIntValue: 29,
        biggeIntValue: 31,
        intArray: [30, 35, 36, 38, 39]
    }
};

const contextWithoutPolicies = {
    userContext: {
        roles: ['someOtherRole', 'rightRole']
    },
    data: data
} as AuthZyinContext<typeof data>;

it('authorize fails on candrink with no policies', () => {
    expect(authorize(contextWithoutPolicies, 'candrink', resource)).toBeFalsy();
});

const contextWithNoMatchingPolicy = {
    userContext: {
        roles: ['someOtherRole', 'rightRole']
    },
    policies: [{name: 'random'}],
    data: data
} as AuthZyinContext<typeof data>;

// Test authorize behavior based on different inputs
describe('authorize function behaves correctly', () => {
    test.each([
        [context,                       'candrink', true],
        [context,                       'canpay',   false],
        [context,                       'canpay',   false],
        [contextWithoutPolicies,        'candrink', false],
        [contextWithNoMatchingPolicy,   'candrink', false],
    ])('authorize behavior (%#)', (context: AuthZyinContext, policy: string, expectedResult: boolean) =>
    {
        expect(authorize(context, policy, resource)).toBe(expectedResult);
    });
});

// Make sure the useAuthorize hook works properly
describe('useAuthorize hook works as expected', () => {
    test.each([
        [context,                       'candrink', true],
        [context,                       'canpay',   false],
        [context,                       'canpay',   false],
        [contextWithoutPolicies,        'candrink', false],
        [contextWithNoMatchingPolicy,   'candrink', false],
    ])(
        'useAuthorize behavior (%#)',
        async (context: AuthZyinContext, policy: string, expectedResult: boolean) =>
        {
            const wrapper = getAuthZyinContextWrapper(context, {});
            const { result } = renderHook(useAuthorize, { wrapper });
            const authorizeFunc = result.current;
            expect(authorizeFunc(policy, resource)).toBe(expectedResult);
        }
    );
});
