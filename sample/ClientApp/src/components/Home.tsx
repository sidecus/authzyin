import * as React from 'react';
import { connect } from 'react-redux';
import { logInAsync } from '../auth/MsalClient';
import { getUserAsync, AuthNResult } from '../api/api';

interface LoginState {
    loginSuccess: boolean,
    loginError: string,
};

const Home = () => {
    const [loginState, setLoginState] = React.useState<LoginState>({
        loginSuccess: false,
        loginError: '',
    });

    const [userState, setUserSate] = React.useState<AuthNResult>();

    // Effect to trigger log in during page load
    React.useEffect(() => {
        logInAsync()
        .then(() => {
            setLoginState(x => {
                return {
                    ...x,
                    loginSuccess: true,
                    loginError: '',
            }});
        })
        .catch((error) => {
            console.log(error);
            setLoginState(x => {
                return {
                    ...x,
                    loginSuccess: false,
                    loginError: error.message,
            }});
        })
      }, []);

    // Effect to call api after logging in
    React.useEffect(() => {
        const fetchUserData = async () => {
            if (loginState.loginSuccess) {
                const userContent = await getUserAsync();
                setUserSate(userContent);
            }
        };
        fetchUserData();
    }, [loginState.loginSuccess, setUserSate]);

    if (loginState.loginSuccess && userState) {
        return (
            <div>
                <h1>Hello, world!</h1>
                <h4>User Data: {userState.message}</h4>
            </div>
        );
    } else if (loginState.loginError) {
        return <h3>Error: {loginState.loginError}</h3>;
    } else {
        return <></>;
    }

};

export default connect()(Home);
