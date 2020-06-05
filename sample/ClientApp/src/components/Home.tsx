import * as React from 'react';
import { connect } from 'react-redux';
import { logInAsync } from '../api/MsalClient';
import { getAuthClientDataAsync, AuthClientData } from '../api/api';
import { ClientData } from './ClientData';

interface LoginState {
    loginSuccess: boolean,
    loginError: string,
};

const Home = () => {
    const [loginState, setLoginState] = React.useState<LoginState>({ loginSuccess: false, loginError: '', });
    const [authClientDataState, setAuthClientDataState] = React.useState<AuthClientData>();

    // Effect to trigger log in during page load
    React.useEffect(() => {
        const logIn = async () => {
            let newState = { loginSuccess: false, loginError: '' };
            try
            {
                const account = await logInAsync();
                console.log(account);
                newState.loginSuccess = true;
                newState.loginError = '';
            } catch(error) {
                newState.loginSuccess = false;
                newState.loginError = error.message;
            }

            setLoginState(newState);
        };

        logIn();
      }, []);

    // Effect to call api after logging in
    React.useEffect(() => {
        const fetchAuthClientData = async () => {
            if (loginState.loginSuccess) {
                const authClientData = await getAuthClientDataAsync();
                setAuthClientDataState(authClientData);
            }
        };

        fetchAuthClientData();
    },
    [loginState.loginSuccess, setAuthClientDataState]);

    // main rendering based on state
    if (loginState.loginSuccess && authClientDataState) {
        return <ClientData data={authClientDataState} />;
    } else if (loginState.loginError) {
        return <h3>Error: {loginState.loginError}</h3>;
    }

    return <></>;
};

export default connect()(Home);
