import { AuthorizationData } from "../api/Contract";
import { createAuthZyinContext } from "authzyin.js";

export const authZyinContext = createAuthZyinContext<AuthorizationData>();
