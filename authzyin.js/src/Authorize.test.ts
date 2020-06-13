import { authorize } from './Authorize';
import { AuthZyinContext } from './AuthZyinContext';
import { RequirementOperatorType, Direction } from './Requirements';

const roleRequirement = {
    operator: RequirementOperatorType.RequiresRole,
    allowedRoles: ['wrongRole', 'rightRole']
};

const ageGreaterThan21Requirement = {
    operator: RequirementOperatorType.GreaterThan,
    direction: Direction.ContextToResource,
    dataJPath: '$.age',
    constValue: 21,
    resourceJPath: '$.value'
};

const hasPaymentMethodRequirement = {
    operator: RequirementOperatorType.Contains,
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

describe('authorize behaves correctly', () => {
    it('authorize passes on candrink', () => {
        expect(authorize(context, 'candrink', resource)).toBeTruthy();
    });

    it('authorize fails on canpay', () => {
        expect(authorize(context, 'canpay', resource)).toBeFalsy();
    });
});

describe('authorize returns false if no context or policies defined', () => {
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

    it('authorize fails on canpay with no matching policy', () => {
        expect(authorize(contextWithNoMatchingPolicy, 'canpay', resource)).toBeFalsy();
    });
});
