import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import App from './App';

it('renders without crashing', () => {
    const storeFake = (state: any) => ({
        default: () => {},
        subscribe: () => {},
        dispatch: () => {},
        getState: () => ({ ...state })
    });
    const store = storeFake({}) as any;

    ReactDOM.render(
        <Provider store={store}>
            <App/>
        </Provider>, document.createElement('div'));
});
