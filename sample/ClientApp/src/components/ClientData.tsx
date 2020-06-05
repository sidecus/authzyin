import * as React from 'react';
import { AuthClientData } from "../api/api";

interface IClientDataProps {
    data: AuthClientData;
}

export const ClientData = ({data}: IClientDataProps) => {
    return (
        <div>
            <h1>Welcome {data.userContext.userName}!</h1>
            <h4>User id: {data.userContext.userId}</h4>
            <h4>Tenant id: {data.userContext.tenantId}</h4>
            <h4>Roles: {JSON.stringify(data.userContext.roles)}</h4>
            <h4>Custom data: {JSON.stringify(data.customData)}</h4>
            <p></p>
            <h3>Auth policies: {JSON.stringify(data.policies)}</h3>
        </div>
    );
}