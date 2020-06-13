/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable @typescript-eslint/explicit-function-return-type */
import {
    camelCasePropertyNamesInJsonPath,
    camelCasePropertyNamesInJsonPathForRequirement
} from './AuthZyinProvider';
import {
    OrRequiremet,
    RequirementOperatorType,
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
            operator: RequirementOperatorType.Equals,
            dataJPath: dataPath,
            resourceJPath: resourcePath
        };

        const requirement2 = {
            direction: Direction.ContextToResource,
            operator: RequirementOperatorType.GreaterThan,
            dataJPath: dataPath,
            resourceJPath: resourcePath
        };

        const requirement3 = {
            direction: Direction.ContextToResource,
            operator: RequirementOperatorType.Contains,
            dataJPath: dataPath,
            resourceJPath: resourcePath
        };

        const requirementInvalid = {
            direction: Direction.ContextToResource,
            operator: RequirementOperatorType.Invalid,
            dataJPath: dataPath,
            resourceJPath: resourcePath
        };

        const requirementRequiresRole = {
            direction: Direction.ContextToResource,
            operator: RequirementOperatorType.RequiresRole,
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

    it('camelCasePropertyNamesInJsonPath converts property name to camel case correctly', () => {
        expect(camelCasePropertyNamesInJsonPath(dataPath)).toBe(
            camelCaseDataPath
        );
        expect(camelCasePropertyNamesInJsonPath(resourcePath)).toBe(
            camelCaseResourcePath
        );
    });

    it('camelCasePropertyNamesInJsonPathForRequirement handles JsonPathRequirement correctly', () => {
        const {
            requirement1,
            requirement2,
            requirement3,
            requirementInvalid,
            requirementRequiresRole
        } = getRequirements();

        expect(IsJsonPathRequirement(requirement1)).toBeTruthy();
        camelCasePropertyNamesInJsonPathForRequirement(requirement1);
        expect(requirement1.dataJPath).toBe(camelCaseDataPath);
        expect(requirement1.resourceJPath).toBe(camelCaseResourcePath);

        // non json path requirement should be untouched
        expect(IsJsonPathRequirement(requirementRequiresRole)).toBeFalsy();
        camelCasePropertyNamesInJsonPathForRequirement(requirementRequiresRole);
        expect(requirementRequiresRole.dataJPath).toBe(dataPath);
        expect(requirementRequiresRole.resourceJPath).toBe(resourcePath);
    });

    it('camelCasePropertyNamesInJsonPathForRequirement handles OrRequirement correctly', () => {
        const {
            requirement1,
            requirement2,
            requirement3,
            requirementInvalid
        } = getRequirements();

        const orRequirement = {
            operator: RequirementOperatorType.Or,
            children: [
                requirement1,
                requirement2,
                requirement3,
                requirementInvalid
            ]
        } as OrRequiremet;

        camelCasePropertyNamesInJsonPathForRequirement(orRequirement);
        expect(requirement1.dataJPath).toBe(camelCaseDataPath);
        expect(requirement1.resourceJPath).toBe(camelCaseResourcePath);
        expect(requirement2.dataJPath).toBe(camelCaseDataPath);
        expect(requirement2.resourceJPath).toBe(camelCaseResourcePath);
        expect(requirement3.dataJPath).toBe(camelCaseDataPath);
        expect(requirement3.resourceJPath).toBe(camelCaseResourcePath);

        expect(IsJsonPathRequirement(requirementInvalid)).toBeFalsy();
        camelCasePropertyNamesInJsonPathForRequirement(requirementInvalid);
        expect(requirementInvalid.dataJPath).toBe(dataPath);
        expect(requirementInvalid.resourceJPath).toBe(resourcePath);
    });
});
