# authzyin.js

> React library for [authzyin](https://github.com/sidecus/authzyin). This handles policy and requirement based authorization on the client.

[![NPM](https://img.shields.io/npm/v/authzyin.js.svg)](https://www.npmjs.com/package/authzyin.js) [![JavaScript Style Guide](https://img.shields.io/badge/code_style-standard-brightgreen.svg)](https://standardjs.com)

## Install
TODO - not published to npm yet. Let me know if this is useful and I'll publish it as a standalone package.
```bash
npm install --save authzyin.js
```

## Usage
### Use together with the authzyin server library
Full policy and requirement based authorization. Built in capability to automatically load policy definitions exposed by the server library. Policies and requirements are resolved and evaluated automatically for you whenever you authorize.
1. Initialize AuthZyinContext (similar as ```createStore``` in Redux, call this globally) and wrap your main component with AuthZyinProvider like below.
```TSX
    // initialize the authzyin context (like redux createStore)
    initializeAuthZyinContext();

    // Wrap your main content with AuthZyinProvider (like redux Provider)
    export const App = () => {
        if (signedIn) {
            return (
                <AuthZyinProvider options={{ requestInitFn: getAuthorizationHeadersAsync }}>
                    <MainContent />
                </AuthZyinProvider>
            );
        }
    }
```
```AuthZyinProvider``` will automatically invoke the authzyin context api to retrieve context/policy/requirement definitions from the built in api provided by authzyin server library. Do this after authentication since the api requires authenticated calls by default for security reasons. You can use the optional ```requestInitFn``` callback parameter in the options to provide your authoriztaion headers if needed.

2. Now you can use the useAuthorize hooks in your components like below:
```TSX
    const authorize = useAuthorize();
    const authorized = authorize('CanEnterBar' /*policy*/, bar /*resource*);
```
Here is the sample code: [App.tsx](https://github.com/sidecus/authzyin/blob/master/authzyin.js/example/src/App.tsx) and [PlaceComponent.tsx](https://github.com/sidecus/authzyin/blob/master/authzyin.js/example/src/components/PlaceComponent.tsx).

### Use as a standalone library
The client library can be used as a standalone library without having to use the server part. To do so, you'll need to create your own ```AuthZyinContext<T>``` object instance. Then set it as a parameter to the ```initializeAuthZyinContext()``` call in step #1 above, like below:
```TSX
    const authZyinContext = /*create my own context*/;
    initializeAuthZyinContext(authZyinContext);
```
AuthZyinProvider will automatically use this context object and stop loading it from the server library api.

AuthZyinContext type is defined at [here](https://github.com/sidecus/authzyin/blob/master/authzyin.js/src/AuthZyinContext.ts). The type parameter T represents the custom data type, against which all JSON path requirement will be evaluated.

[Test cases for AuthZyinProvider](https://github.com/sidecus/authzyin/blob/master/authzyin.js/src/AuthZyinProvider.test.tsx) illustrates this usage pattern.

## License

MIT © [sidecus](https://github.com/sidecus)
