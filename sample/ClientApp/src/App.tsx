import * as React from 'react';
import { useSelector } from 'react-redux';
import { createMuiTheme, MuiThemeProvider, makeStyles } from '@material-ui/core/styles';
import { Paper, Container } from '@material-ui/core';
import { initializeAuthZyinContext, AuthZyinProvider } from 'authzyin.js';
import { getAuthorizationHeadersAsync } from './api/Api';
import { useAppBoundActions } from './store/actions';
import { signInfoSelector } from './store/selectors';
import { Home } from './components/Home';

// dark theme
export const darkTheme = createMuiTheme({
    palette: {
        type: 'dark',
    },
});

const useStyles = makeStyles(() => ({
    root: {
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'center',
        alignItems: 'center',
    }
}));

// Initialize authzyin context globally
initializeAuthZyinContext();

export default () => {
    const classes = useStyles();
    const signInInfo = useSelector(signInfoSelector);
    const { signIn } = useAppBoundActions();

    // Effect to trigger log in during page load
    React.useEffect(() => {
        signIn();
    }, [signIn]);

    if (signInInfo.success) {
        return (
            // Sign in succeeded, render main content wrapped with AuthZyinProvider
            <AuthZyinProvider options={{ requestInitFn: getAuthorizationHeadersAsync }}>
                <Container>
                    <MuiThemeProvider theme={darkTheme}>
                        <Paper className={classes.root}>
                            <Home />
                        </Paper>
                    </MuiThemeProvider>
                </Container>
            </AuthZyinProvider>
        );
    } else if (signInInfo.signInError) {
        return <h3>Error: {signInInfo.signInError}</h3>;
    } else {
        return <h3>Signing in</h3>;
    }
};
