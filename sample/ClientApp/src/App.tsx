import * as React from 'react';
import { useSelector } from 'react-redux';
import { createMuiTheme, MuiThemeProvider, makeStyles, Theme } from '@material-ui/core/styles';
import { Paper, Container } from '@material-ui/core';
import { getAuthorizationHeadersAsync } from './api/Api';
import { AuthZyinProvider } from './authzyin/AuthZyinProvider';
import { useSampleAppBoundActionCreators } from './store/actions';
import { authZyinContext } from './store/authorization';
import { signInfoSelector } from './store/selectors';
import { Home } from './components/Home';

// dark theme
export const darkTheme = createMuiTheme({
    palette: {
        type: 'dark',
    },
});

const useStyles = makeStyles((theme: Theme) => ({
    root: {
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'center',
        alignItems: 'center',
    }
}));

export default () => {
    const classes = useStyles();
    const signInInfo = useSelector(signInfoSelector);
    const { signIn } = useSampleAppBoundActionCreators();

    // Effect to trigger log in during page load
    React.useEffect(() => {
        signIn();
    }, [signIn]);

    if (signInInfo.success) {
        return (
            <AuthZyinProvider context={authZyinContext} requestInitFn={getAuthorizationHeadersAsync}>
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
