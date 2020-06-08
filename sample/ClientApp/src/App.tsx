import * as React from 'react';
import { Home } from './components/Home';
import { createMuiTheme, MuiThemeProvider, makeStyles, Theme } from '@material-ui/core/styles';
import { Paper, Container } from '@material-ui/core';

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

    return (
        <Container>
            <MuiThemeProvider theme={darkTheme}>
                <Paper className={classes.root}>
                    <Home />
                </Paper>
            </MuiThemeProvider>
        </Container>
    );
};
