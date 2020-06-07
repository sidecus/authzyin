import * as React from 'react';
import { useSelector } from 'react-redux';
import { userInfoSelector } from '../store/selectors';

export const User = () => {
    const userInfo = useSelector(userInfoSelector);

    // main rendering based on state
    if (userInfo) {
        return (
            <h1>Welcome {userInfo.userName}!</h1>
        );
    }

    return <></>;
};