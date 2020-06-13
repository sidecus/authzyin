/* eslint-disable @typescript-eslint/explicit-function-return-type */
import {
    camelCaseJsonPath,
    camelCaseRequirement
} from './PropNameCamelCase';
import {
    OrRequiremet,
    OperatorType,
    IsJsonPathRequirement,
    Direction
} from './Requirements';

describe('JsonPath camel case', () => {
    const dataPath = '$.Data.Array[@.length-1].[?(@.IntProperty > 0)]';
    const camelCaseDataPath = '$.data.array[@.length-1].[?(@.intProperty > 0)]';
    const resourcePath = '$..BookTitleA.AuthorNamess[-1:]';
    const camelCaseResourcePath = '$..bookTitleA.authorNamess[-1:]';

    const getRequirements = () => {
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

        return {
            requirement1,
            requirement2,
            requirement3,
            requirementInvalid,
            requirementRequiresRole
        };
    };

    it('camelCaseJsonPath converts property name to camel case correctly', () => {
        expect(camelCaseJsonPath(dataPath)).toBe(
            camelCaseDataPath
        );
        expect(camelCaseJsonPath(resourcePath)).toBe(
            camelCaseResourcePath
        );
    });

    it('camelCaseRequirement handles JsonPathRequirement correctly', () => {
        const {
            requirement1,
            requirementRequiresRole
        } = getRequirements();

        expect(IsJsonPathRequirement(requirement1)).toBeTruthy();
        camelCaseRequirement(requirement1);
        expect(requirement1.dataJPath).toBe(camelCaseDataPath);
        expect(requirement1.resourceJPath).toBe(camelCaseResourcePath);

        // non json path requirement should be untouched
        expect(IsJsonPathRequirement(requirementRequiresRole)).toBeFalsy();
        camelCaseRequirement(requirementRequiresRole);
        expect(requirementRequiresRole.dataJPath).toBe(dataPath);
        expect(requirementRequiresRole.resourceJPath).toBe(resourcePath);
    });

    it('camelCaseRequirement handles OrRequirement correctly', () => {
        const {
            requirement1,
            requirement2,
            requirement3,
            requirementInvalid
        } = getRequirements();

        const orRequirement = {
            operator: OperatorType.Or,
            children: [
                requirement1,
                requirement2,
                requirement3,
                requirementInvalid
            ]
        } as OrRequiremet;

        camelCaseRequirement(orRequirement);
        expect(requirement1.dataJPath).toBe(camelCaseDataPath);
        expect(requirement1.resourceJPath).toBe(camelCaseResourcePath);
        expect(requirement2.dataJPath).toBe(camelCaseDataPath);
        expect(requirement2.resourceJPath).toBe(camelCaseResourcePath);
        expect(requirement3.dataJPath).toBe(camelCaseDataPath);
        expect(requirement3.resourceJPath).toBe(camelCaseResourcePath);

        expect(IsJsonPathRequirement(requirementInvalid)).toBeFalsy();
        camelCaseRequirement(requirementInvalid);
        expect(requirementInvalid.dataJPath).toBe(dataPath);
        expect(requirementInvalid.resourceJPath).toBe(resourcePath);
    });
});
