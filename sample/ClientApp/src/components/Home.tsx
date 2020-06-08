import * as React from 'react';
import { useSelector } from 'react-redux';
import { SampleContext } from './SampleContext';
import { useSampleAppBoundActionCreators } from '../store/actions';
import { signInfoSelector, authZyinContextSelector } from '../store/selectors';
import { User } from './User';
import { BarList } from './BarList';
import { AlertBanner } from './Alert';
import { makeStyles } from '@material-ui/core';

const useStyles = makeStyles((theme) => ({
    root: {
      display: 'flex',
      flexDirection: 'column',
      justifyContent: 'center',
    },
  }));

export const Home = () => {
    const classes  = useStyles();
    const { signIn } = useSampleAppBoundActionCreators();
    const signInInfo = useSelector(signInfoSelector);
    const clientContext = useSelector(authZyinContextSelector);

    // Effect to trigger log in during page load
    React.useEffect(() => {
        signIn();
      }, [signIn]);

    // main rendering based on state
    if (signInInfo.success && clientContext.userContext) {
        return (
            <div className = {classes.root}>
                <User />
                <BarList />
                <AlertBanner />
                <SampleContext data={clientContext} />
            </div>
        );
    } else if (signInInfo.signInError) {
        return <h3>Error: {signInInfo.signInError}</h3>;
    } else {
        return <></>;
    }
};