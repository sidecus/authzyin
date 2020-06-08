import * as React from 'react';
import { useSelector } from 'react-redux';
import { AuthContext } from './AuthContext';
import { signInfoSelector, authZyinContextSelector, userInfoSelector } from '../store/selectors';
import { User } from './User';
import { BarList } from './BarList';
import { Grid, Typography } from '@material-ui/core';

export const Home = () => {
    const signInInfo = useSelector(signInfoSelector);
    const userInfo = useSelector(userInfoSelector);
    const clientContext = useSelector(authZyinContextSelector);

    // main rendering based on state
    if (clientContext.userContext) {
        return (
            <div>
                <Typography variant="h2" component="h2">
                    Welcome {userInfo.userName}!
                </Typography>
                <Grid container direction='column' justify='center' alignItems='stretch'>
                    <Grid item xl={12}>
                        <BarList />
                    </Grid>
                    <Grid item xl={12}>
                        <User />
                    </Grid>
                    <Grid item xl={12}>
                        <AuthContext data={clientContext} />
                    </Grid>
                </Grid>
            </div>
        );
    } else if (signInInfo.signInError) {
        return <h3>Error: {signInInfo.signInError}</h3>;
    } else {
        return <></>;
    }
};