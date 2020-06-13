import { evaluateRequirement } from './RequirementEvaluator';
import { OperatorType, Direction } from './Requirements';
import { AuthZyinContext } from './AuthZyinContext';

describe('evaluateRequirement', () => {
    const data = {
        age: 30,
        paymentMethods: ['visa', 'creditcard'],
        stringValue: 'random@#$@%#$%'
    };

    const context = {
        userContext: {
            roles: ['someOtherRole', 'rightRole']
        },
        data: data
    } as AuthZyinContext<typeof data>;

    const resource = {
        allowedPaymentMethod: 'visa',
        randomeStringValue: 'random@#$@%#$%',
        resourceData: {
            intValue: 30,
            smallerIntValue: 29,
            biggeIntValue: 31,
            biggerStringValue: data.stringValue + ' ',
            intArray: [30, 35, 36, 38, 39]
        }
    };

    it('evalueteRoleRequirement behaves correctly', () => {
        const roleRequirement = {
            operator: OperatorType.RequiresRole,
            allowedRoles: ['wrongRole', 'rightRole']
        };

        const invalidRoleRequirement = {
            operator: OperatorType.RequiresRole,
            allowedRoles: ['invalidRole1', 'invalidRole2']
        };

        expect(evaluateRequirement(context, roleRequirement)).toBeTruthy();
        expect(
            evaluateRequirement(context, invalidRoleRequirement)
        ).toBeFalsy();
    });

    it('equalsRequirement behaves correctly', () => {
        const intEqualsRequirement = {
            operator: OperatorType.Equals,
            direction: Direction.ContextToResource,
            dataJPath: '$.age',
            resourceJPath: '$.resourceData.intValue'
        };

        const stringEqualsRequirement = {
            operator: OperatorType.Equals,
            direction: Direction.ContextToResource,
            dataJPath: '$.paymentMethods[0]',
            resourceJPath: '$.allowedPaymentMethod'
        };

        expect(
            evaluateRequirement(context, intEqualsRequirement, resource)
        ).toBeTruthy();

        expect(
            evaluateRequirement(context, {
                operator: OperatorType.Equals,
                direction: Direction.ContextToResource,
                dataJPath: '$.age',
                resourceJPath: '$.resourceData.smallerIntValue'
            })
        ).toBeFalsy();

        expect(
            evaluateRequirement(context, stringEqualsRequirement, resource)
        ).toBeTruthy();
        expect(
            evaluateRequirement(
                context,
                {
                    operator: OperatorType.Equals,
                    direction: Direction.ContextToResource,
                    dataJPath: '$.paymentMethods[0]',
                    resourceJPath: '$.resourceData.randomeStringValue'
                },
                resource
            )
        ).toBeFalsy();
    });

    it('greater than behaves correctly', () => {
        const positiveGreaterThanRequirement = {
            operator: OperatorType.GreaterThan,
            direction: Direction.ResourceToContext,
            dataJPath: '$.age',
            resourceJPath: '$.resourceData.biggeIntValue'
        };

        const negativeGreaterThanRequirement = {
            operator: OperatorType.GreaterThan,
            direction: Direction.ContextToResource,
            dataJPath: '$.age',
            resourceJPath: '$.resourceData.biggeIntValue'
        };

        expect(
            evaluateRequirement(
                context,
                positiveGreaterThanRequirement,
                resource
            )
        ).toBeTruthy();
        expect(
            evaluateRequirement(
                context,
                negativeGreaterThanRequirement,
                resource
            )
        ).toBeFalsy();
    });

    // Table driven test case for greater than or equal to
    test.each([
        [Direction.ContextToResource, '$.age', '$.resourceData.biggeIntValue', false],
        [Direction.ResourceToContext, '$.age', '$.resourceData.biggeIntValue', true],
        [Direction.ContextToResource, '$.age', '$.resourceData.intValue', true],
        [Direction.ResourceToContext, '$.age', '$.resourceData.intValue', true],
        [Direction.ContextToResource, '$.age', '$.resourceData.smallerIntValue', true],
        [Direction.ResourceToContext, '$.age', '$.resourceData.smallerIntValue', false],
        [Direction.ContextToResource, '$.stringValue', '$.resourceData.biggerStringValue', false],
        [Direction.ResourceToContext, '$.stringValue', '$.resourceData.biggerStringValue', true]
    ])('GreaterThanOrEqualTo(%#)', (direction, dataPath, resourcePath, expected) => {
        const requirement = {
            operator: OperatorType.GreaterThanOrEqualTo,
            direction: direction,
            dataJPath: dataPath,
            resourceJPath: resourcePath
        };

        expect(evaluateRequirement(context, requirement, resource)).toBe(expected);
    });

    it('contains behaves correctly', () => {
        const positiveContainsThanRequirement = {
            operator: OperatorType.Contains,
            direction: Direction.ContextToResource,
            dataJPath: '$.paymentMethods[*]',
            resourceJPath: '$.allowedPaymentMethod'
        };

        const negativeContainsThanRequirement = {
            operator: OperatorType.Contains,
            direction: Direction.ResourceToContext,
            dataJPath: '$.age',
            resourceJPath: '$.resourceData.intArray[-2:]'
        };

        expect(
            evaluateRequirement(
                context,
                positiveContainsThanRequirement,
                resource
            )
        ).toBeTruthy();

        expect(
            evaluateRequirement(
                context,
                negativeContainsThanRequirement,
                resource
            )
        ).toBeFalsy();

        // array object selector should not trigger contains
        expect(
            evaluateRequirement(
                context,
                {
                    operator: OperatorType.Contains,
                    direction: Direction.ContextToResource,
                    dataJPath: '$.paymentMethods',
                    resourceJPath: '$.allowedPaymentMethod'
                },
                resource
            )
        ).toBeFalsy();
    });

    it('constjsonpathrequirement behaves correctly', () => {
        const positiveIntConstEqualsRequirement = {
            operator: OperatorType.Equals,
            direction: Direction.ContextToResource,
            dataJPath: '$.age',
            constValue: 30,
            resourceJPath: '$.value'
        };

        expect(
            evaluateRequirement(context, positiveIntConstEqualsRequirement)
        ).toBeTruthy();

        const positiveStringEqualsRequirement = {
            operator: OperatorType.Equals,
            direction: Direction.ContextToResource,
            dataJPath: '$.paymentMethods[0]',
            constValue: 'visa',
            resourceJPath: '$.value'
        };

        expect(
            evaluateRequirement(context, positiveStringEqualsRequirement)
        ).toBeTruthy();

        const positiveIntGreaterThanRequirement = {
            operator: OperatorType.GreaterThan,
            direction: Direction.ContextToResource,
            dataJPath: '$.age',
            constValue: 10,
            resourceJPath: '$.value'
        };

        expect(
            evaluateRequirement(context, positiveIntGreaterThanRequirement)
        ).toBeTruthy();

        const negativeConstContainsRequirement = {
            operator: OperatorType.Contains,
            direction: Direction.ResourceToContext,
            dataJPath: '$.paymentMethods',
            constValue: '32#%$^$%^',
            resourceJPath: '$.value'
        };

        expect(
            evaluateRequirement(context, negativeConstContainsRequirement)
        ).toBeFalsy();
    });
});
