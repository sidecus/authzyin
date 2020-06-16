# authzyin.js

> React library for [authzyin](https://github.com/sidecus/authzyin). This handles policy and requirement based authorization on the client.

[![NPM](https://img.shields.io/npm/v/authzyin.js.svg)](https://www.npmjs.com/package/authzyin.js) [![JavaScript Style Guide](https://img.shields.io/badge/code_style-standard-brightgreen.svg)](https://standardjs.com)

## Install
TODO - not published to npm yet. Let me know if this is useful and I'll publish it as a standalone package.
```Shell
npm install --save authzyin.js
```

## Usage
### Use together with the authzyin server library
The client library supports full policy and requirement based authorization, with built in capability to automatically fetch policy definitions exposed by the server library. Policies and requirements are resolved and evaluated automatically for you whenever you authorize.
1. Initialize AuthZyinContext (similar as ```createStore``` in Redux, call this globally) and wrap your main component with ```AuthZyinProvider``` like below. Do this *after authentication* since the api provided by the lib requires authenticated call by default for security reasons. You can use the ```requestInitFn``` callback in the ```options``` parameter to provide authoriztaion headers. Ignore options if you are using cookie auth.
```TSX
    // Initialize context
    initializeAuthZyinContext();

    // Wrap main content with AuthZyinProvider after signing in
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
2. Now you can call the ```useAuthorize``` hook to achieve policy based authorization in your components like below. More in the [sample](https://github.com/sidecus/authzyin/blob/master/authzyin.js/example/src/components/PlaceComponent.tsx):
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
```AuthZyinProvider``` will automatically use the provided context object and stop fetching from the server library api.

```AuthZyinContext``` type is defined at [here](https://github.com/sidecus/authzyin/blob/master/authzyin.js/src/AuthZyinContext.ts). The type parameter T represents the custom data type, against which context related JSON path requirement will be evaluated.

[Test cases for AuthZyinProvider](https://github.com/sidecus/authzyin/blob/master/authzyin.js/src/AuthZyinProvider.test.tsx) illustrates this usage pattern.

## License

MIT © [sidecus](https://github.com/sidecus)
