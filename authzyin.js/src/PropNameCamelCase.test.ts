/* eslint-disable @typescript-eslint/explicit-function-return-type */
import { camelCaseJsonPath, camelCaseRequirement } from './PropNameCamelCase';
import { OrRequiremet, OperatorType, IsJsonPathRequirement, Direction, Requirement } from './Requirements';

describe('JsonPath camel case', () => {
    const dataPath = '$.Data.Array[@.length-1].[?(@.IntProperty > 0)]';
    const camelCaseDataPath = '$.data.array[@.length-1].[?(@.intProperty > 0)]';
    const resourcePath = '$..BookTitleA.AuthorNamess[-1:]';
    const camelCaseResourcePath = '$..bookTitleA.authorNamess[-1:]';
    const requirement1 = {
        direction: Direction.ContextToResource,
        operator: OperatorType.Equals,
        dataJPath: dataPath,
        resourceJPath: resourcePath
    };

    const requirement2 = {
        direction: Direction.ContextToResource,
        operator: OperatorType.GreaterThan,
        dataJPath: dataPath,
        resourceJPath: resourcePath
    };

    const requirement3 = {
        direction: Direction.ContextToResource,
        operator: OperatorType.Contains,
        dataJPath: dataPath,
        resourceJPath: resourcePath
    };

    const requirementInvalid = {
        direction: Direction.ContextToResource,
        operator: OperatorType.Invalid,
        dataJPath: dataPath,
        resourceJPath: resourcePath
    };

    const requirementRequiresRole = {
        direction: Direction.ContextToResource,
        operator: OperatorType.RequiresRole,
        allowedRoles: ['somerole'],
        dataJPath: dataPath,
        resourceJPath: resourcePath
    };

    test.each([
        [dataPath, camelCaseDataPath],
        [camelCaseDataPath, camelCaseDataPath],
        [resourcePath, camelCaseResourcePath],
        [camelCaseResourcePath, camelCaseResourcePath],
        ['$.Value', '$.value'],
        ['$..Payment.AllowedPaymentMethods[*]', '$..payment.allowedPaymentMethods[*]'],
        ['$..Book[?(@.Price<10)]', '$..book[?(@.price<10)]'],
        ['', ''],
        ['.A..A...A....A', '.a..a...a....a'],
        ['..........', '..........']
    ])('camelCaseJsonPath camel behavior (%#)', (input: string, expectedOutout: string) => {
        expect(camelCaseJsonPath(input)).toBe(expectedOutout);
    });

    test.each([
        [requirement1, true, camelCaseDataPath, camelCaseResourcePath],
        [requirement2, true, camelCaseDataPath, camelCaseResourcePath],
        [requirement3, true, camelCaseDataPath, camelCaseResourcePath],
        [requirementInvalid, false, dataPath, resourcePath],
        [requirementRequiresRole, false, dataPath, resourcePath]
    ])(
        'camelCaseRequirement behavior (%#)',
        (requirement: Requirement, jsonPathReq: boolean, expectedDataPath, expectedResourcePath) => {
            const requirementCopy = {...requirement} as any;
            expect(IsJsonPathRequirement(requirementCopy)).toBe(jsonPathReq);
            camelCaseRequirement(requirementCopy);
            expect(requirementCopy.dataJPath).toBe(expectedDataPath);
            expect(requirementCopy.resourceJPath).toBe(expectedResourcePath);
        }
    );

    it('camelCaseRequirement w/ OrRequirement(%#)', () => {
        const req1 = {...requirement1};
        const req2 = {...requirement2};
        const req3 = {...requirement3};
        const reqInvalid = {...requirementInvalid};
        const orRequirement = {
            operator: OperatorType.Or,
            children: [req1, req2, req3, reqInvalid]
        } as OrRequiremet;

        camelCaseRequirement(orRequirement);
        expect(req1.dataJPath).toBe(camelCaseDataPath);
        expect(req1.resourceJPath).toBe(camelCaseResourcePath);
        expect(req2.dataJPath).toBe(camelCaseDataPath);
        expect(req2.resourceJPath).toBe(camelCaseResourcePath);
        expect(req3.dataJPath).toBe(camelCaseDataPath);
        expect(req3.resourceJPath).toBe(camelCaseResourcePath);

        expect(IsJsonPathRequirement(reqInvalid)).toBeFalsy();
        camelCaseRequirement(reqInvalid);
        expect(reqInvalid.dataJPath).toBe(dataPath);
        expect(reqInvalid.resourceJPath).toBe(resourcePath);
    });
});
