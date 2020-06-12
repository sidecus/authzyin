# authzyin.js

> React library for [authzyin](https://github.com/sidecus/authzyin). This handles authorization policy and requirement processing on the client.

[![NPM](https://img.shields.io/npm/v/authzyin.js.svg)](https://www.npmjs.com/package/authzyin.js) [![JavaScript Style Guide](https://img.shields.io/badge/code_style-standard-brightgreen.svg)](https://standardjs.com)

## Install - TBD, not published to npm yet.

```bash
npm install --save authzyin.js
```

## Usage

1. Create a AuthZyinContext (similar as createStore in Redux)
```
export const authorizationContext = createAuthZyinContext<AuthorizationData>();
```

2. Connect the AuthZyinProvider with your components
```
    <AuthZyinProvider context={authorizationContext} requestInitFn={getAuthorizationHeadersAsync}>
        <MainContent />
    </AuthZyinProvider>
```
3. Use the useAuthorize hooks in your components
```
    const authorize = useAuthorize();
    
    const authorized = authorize('PolicyName', resource);
```

## License

MIT © [sidecus](https://github.com/sidecus)
