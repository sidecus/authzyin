import * as React from 'react';
import { Typography, Card, CardContent, CardHeader } from '@material-ui/core';
import { useAuthZyinContext } from 'authzyin.js';
import { AuthorizationData } from '../api/Contract';

export const User = () => {
    const clientContext = useAuthZyinContext<AuthorizationData>();

    // main rendering based on state
    if (clientContext) {
        return (
            <div>
                <Card variant="outlined">
                    <CardHeader title='User information' />
                    <CardContent>
                        <Typography variant="body1" component="div">
                            Age: {clientContext.data.age}
                        </Typography>
                        <Typography variant="body1" component="div">
                            Has driver's license: {String(clientContext.data.withDriversLicense)}
                        </Typography>
                        <Typography variant="body1" component="div">
                            Has passport: {String(clientContext.data.withPassport)}
                            </Typography>
                        <Typography variant="body1" component="div">
                            PaymentMethods: {JSON.stringify(clientContext.data.paymentMethods.map(x => x.type))}
                        </Typography>
                    </CardContent>
                </Card>
            </div>
        );
    }

    return <></>;
};