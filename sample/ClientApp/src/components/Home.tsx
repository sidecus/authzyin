import * as React from 'react';
import { connect, useSelector } from 'react-redux';
import { SampleContext } from './SampleContext';
import { useSampleAppBoundActionCreators } from '../store/actions';
import { getSignInInfo, getAuthZyinContext } from '../store/selectors';

const Home = () => {
    const { signIn } = useSampleAppBoundActionCreators();
    const signInInfo = useSelector(getSignInInfo);
    const clientContext = useSelector(getAuthZyinContext);

    // Effect to trigger log in during page load
    React.useEffect(() => {
        signIn();
      }, [signIn]);

    // main rendering based on state
    if (signInInfo.success && clientContext.userContext) {
        return <SampleContext data={clientContext} />;
    } else if (signInInfo.signInError) {
        return <h3>Error: {signInInfo.signInError}</h3>;
    } else {
        return <></>;
    }
};

export default connect()(Home);
