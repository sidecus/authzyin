# authzyin.js

> React library for [authzyin](https://github.com/sidecus/authzyin). This handles authorization policy and requirement processing on the client.

[![NPM](https://img.shields.io/npm/v/authzyin.js.svg)](https://www.npmjs.com/package/authzyin.js) [![JavaScript Style Guide](https://img.shields.io/badge/code_style-standard-brightgreen.svg)](https://standardjs.com)

## Install
TODO - not published to npm yet.
```bash
npm install --save authzyin.js
```

## Usage

1. Initialize AuthZyinContext (similar as createStore in Redux, call this globally e.g. in index.tsx)
```
    initializeAuthZyinContext();
```
[Example index.tsx](https://github.com/sidecus/authzyin/blob/master/authzyin.js/example/src/index.tsx)

2. Connect the AuthZyinProvider with your components. Do this **after authentication** since the authzyin context api provided by the lib requires authenticated users.
```
    <AuthZyinProvider options={{ requestInitFn: getAuthorizationHeadersAsync }}>
        <MainContent />
    </AuthZyinProvider>
```
[Example App.tsx](https://github.com/sidecus/authzyin/blob/master/authzyin.js/example/src/App.tsx)

3. Use the useAuthorize hooks in your components
```
    const authorize = useAuthorize();
    const authorized = authorize('PolicyName', resource);
```
[Example PlaceComponent using this](https://github.com/sidecus/authzyin/blob/master/authzyin.js/example/src/components/PlaceComponent.tsx)

## License

MIT © [sidecus](https://github.com/sidecus)
