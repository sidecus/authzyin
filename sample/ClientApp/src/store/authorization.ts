import { AuthorizationData } from "../api/Contract";
import { createAuthZyinContext } from "../authzyin/Authorize";

export const authZyinContext = createAuthZyinContext<AuthorizationData>();
