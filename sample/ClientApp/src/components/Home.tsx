import * as React from 'react';
import { connect } from 'react-redux';
import { logInAsync } from '../api/MsalClient';
import { GetAuthZyinClientContextAsync, SampleClientContext } from '../api/api';
import { SampleContext } from './SampleContext';

interface LoginState {
    loginSuccess: boolean,
    loginError: string,
};

const Home = () => {
    const [loginState, setLoginState] = React.useState<LoginState>({ loginSuccess: false, loginError: '', });
    const [authZClientContext, setAuthZClientContext] = React.useState<SampleClientContext>();

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
        const fetchAuthZClientContext = async () => {
            if (loginState.loginSuccess) {
                const clientContext = await GetAuthZyinClientContextAsync();
                setAuthZClientContext(clientContext);
            }
        };

        fetchAuthZClientContext();
    },
    [loginState.loginSuccess, setAuthZClientContext]);

    // main rendering based on state
    if (loginState.loginSuccess && authZClientContext) {
        return <SampleContext data={authZClientContext} />;
    } else if (loginState.loginError) {
        return <h3>Error: {loginState.loginError}</h3>;
    } else {
        return <></>;
    }
};

export default connect()(Home);
