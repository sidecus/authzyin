import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import {sampleStore} from './store/store';
import App from './App';
import registerServiceWorker from './registerServiceWorker';
import { initializeAuthZyinContext } from 'authzyin.js';
import { AuthorizationData } from './api/Contract';

// Initialize authzyin context first
initializeAuthZyinContext<AuthorizationData>();

ReactDOM.render(
    <Provider store={sampleStore}>
        <App />
    </Provider>,
    document.getElementById('root'));

registerServiceWorker();
