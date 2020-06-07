import { Resource } from "./Resource";
import { AuthZyinContext } from "./AuthZyinContext";
import { useSelector } from "react-redux";
import { evaluateRequirement } from "./RequirementEvaluator";

const authorize = <TCustomData>(context: AuthZyinContext<TCustomData>, policy: string, resource?: Resource) =>
{
    const policyObject = context.policies.filter(p => p.name === policy)[0];
    const requirements = policyObject.requirements;
    let result = true;

    for (let i = 0; i < requirements.length; i ++) {
        result = evaluateRequirement(context, requirements[i], resource);
        if (!result) {
            // current requirement failed, no need to continue
            return false;
        }
    }

    return true;
}

/*
* Authorization hooks
*/
export const useAuthorize = <TState, TCustomData>(selector: (state: TState) => AuthZyinContext<TCustomData>) => {
    const context = useSelector(selector);

    return (policy: string, resource?: Resource) => {
        console.log(`Client authorization - policy: ${policy} w/ resource: ${JSON.stringify(resource)}`)

        const result = authorize(context, policy, resource);

        if (result) {
            console.log('Client authorization succeeded.');
        } else {
            console.error('Client authorization failed.');
        }

        return result;
    }
}