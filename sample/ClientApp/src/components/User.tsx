import * as React from 'react';
import { authZyinContextSelector } from '../store/selectors';
import { Typography, Card, CardContent, CardHeader } from '@material-ui/core';
import { useSelector } from 'react-redux';

export const User = () => {
    const authorizationInfo = useSelector(authZyinContextSelector);

    // main rendering based on state
    if (authorizationInfo) {
        return (
            <div>
                <Card variant="outlined">
                    <CardHeader title='User information' />
                    <CardContent>
                        <Typography variant="body1" component="div">
                            Age: {authorizationInfo.data.age}
                        </Typography>
                        <Typography variant="body1" component="div">
                            Has driver's license: {String(authorizationInfo.data.withDriversLicense)}
                        </Typography>
                        <Typography variant="body1" component="div">
                            Has passport: {String(authorizationInfo.data.withPassport)}
                            </Typography>
                        <Typography variant="body1" component="div">
                            PaymentMethods: {JSON.stringify(authorizationInfo.data.paymentMethods.map(x => x.type))}
                        </Typography>
                    </CardContent>
                </Card>
            </div>
        );
    }

    return <></>;
};