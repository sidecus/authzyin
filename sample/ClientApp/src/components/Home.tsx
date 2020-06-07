import * as React from 'react';
import { useSelector } from 'react-redux';
import { SampleContext } from './SampleContext';
import { useSampleAppBoundActionCreators } from '../store/actions';
import { signInfoSelector, authZyinContextSelector } from '../store/selectors';
import { User } from './User';
import { BarList } from './BarList';

export const Home = () => {
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
            <div>
                <User />
                <BarList />
                <SampleContext data={clientContext} />
            </div>
        );
    } else if (signInInfo.signInError) {
        return <h3>Error: {signInInfo.signInError}</h3>;
    } else {
        return <></>;
    }
};