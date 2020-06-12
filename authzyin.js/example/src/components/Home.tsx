import * as React from 'react';
import { AuthContext } from './AuthContext';
import { User } from './User';
import { PlaceList } from './PlaceList';
import { Grid, Typography } from '@material-ui/core';
import { useAuthZyinContext } from 'authzyin.js';
import { AuthorizationData } from '../api/Contract';

export const Home = () => {
    const context = useAuthZyinContext<AuthorizationData>();

    return (
        <div>
            <Typography variant="h2" component="h2">
                Welcome {context.userContext.userName}!
            </Typography>
            <Grid container direction='column' justify='center' alignItems='stretch'>
                <Grid item xl={12}>
                    <PlaceList />
                </Grid>
                <Grid item xl={12}>
                    <User />
                </Grid>
                <Grid item xl={12}>
                    <AuthContext data={context} />
                </Grid>
            </Grid>
        </div>
    );
};