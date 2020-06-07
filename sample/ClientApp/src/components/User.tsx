import * as React from 'react';
import { useSelector } from 'react-redux';
import { userInfoSelector } from '../store/selectors';
import { Typography } from '@material-ui/core';

export const User = () => {
    const userInfo = useSelector(userInfoSelector);

    // main rendering based on state
    if (userInfo) {
        return (
            <Typography variant="h2" component="h1">
                Welcome {userInfo.userName}!
            </Typography>
        );
    }

    return <></>;
};