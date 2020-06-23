# authzyin
Asp.Net Core library and React library enabling easy server & client policy based authorization (defining just once).

![.NET Core](https://github.com/sidecus/authzyin/workflows/.NET%20Core/badge.svg)

## Motivation
Authorization is both simple and complex. Often we need to have proper authorization on the server to protect apis, and at the same time, provide similar authorization on the client UI to make it more user firendly. The common practice is to repeat some authorization logic twice, once on the server, and once on the client.

If there are only a few authoriztaion policies/requirements, this is relatively easy and no big deal. But as our application grows, the work required to maintain and make sure both sides in sync also grows dramatically, and it'll likely no longer be a simple task.

I was thinking about whether we can have some libraries to enable the same authorization on both server and client, aka write once, use in both places leveraging the good policy based patterns from asp.net core. This repo is my first try over the past few days. It contains server library and client library to make this simple for you.

*Kindly note*: the server & client libraries can be used together, or standalone. When using together the libraries make the authorization much easier, but it's not a must.

## How to use this library
### Server library (this project):
1. Define your requirements and policies, like in the [sample project](https://github.com/sidecus/authzyin/blob/master/sample/AuthN/Requirements.cs).
2. Use the ```AddAuthZyinAuthorization``` extension method to enable AuthZyin authorization and register your own ```IAuthZyinContext``` implementation as a scoped service in Startup.cs.
```C#
    services.AddAuthZyinAuthorization(options =>
    {
        // add policies
        options.AddPolicy("isCustomer", Policies.IsCustomer);
        options.AddPolicy("CanEnterBar", Policies.CanEnterBar);
        ...
    });

    // Add scoped context, used for authorization on both server and client
    services.AddScoped<IAuthZyinContext, SampleAuthZyinContext>();
```
3. Now you can use the policies in the standard asp.net core way (as param to ```AuthorizeAttribute``` for those not requiring resources, or as part of ```AuthorizeAsync```).
```C#
    // Authorize based on policy and resource
    var authResult = await this.authorizationService.AuthorizeAsync(user, bar, "CanEnterBar");
```

This library provides similar policy/requirement authorization pattern from asp.net core without having to use AuthroziationHandlers, with JSON path based requirement definition and evlauation. It also provides a built in api to expose the policy/requirement definitions so that client can consume direclty. Compared to standard asp.net core pattern the only additional thing you need to do is to register a scoped ```IAuthZyinContext``` service into the DI container to construct the context instance which will used as part of the requirement evaluation.
**You don't need to worry about serializing and exposing api for this data to be shared with the client**. The library handles that for you automatically once you finish 1 & 2 above. If you'd like, you can also define your policies/requirements in a json config file and load it during your app startup. It depends on perosnal preference so not included as part of this library. I am using asp.net core 3.1 and if you are on asp.net core 2.x, you can reuse majority of the code but will have to write your own extension method similar as ```AddAuthZyinAuthorization```.

### Client (authzyin.js):
The client library is a standalone library to enable policy based authorization in React. It's in a standalone repo now with published NPM package - [authzyin.js](https://github.com/sidecus/authzyin.js).
When used together with the server library it makes things a lot easier. You can still use it by itself if you just want to take advantage of policy based authorization capability in React.

It's simple to use: initialize ```AuthZyinContext``` (similar as ```createStore``` in Redux, call this globally), wrap your main component with ```AuthZyinProvider```, and authorize.
```TSX
    /**
     * Initialize context - do this once before your main component rendering.
     * This creates a react context object to hold the authorization context.
     */
    initializeAuthZyinContext();

    /**
     * Wrap main content with AuthZyinProvider (after signing in).
     * AuthZyinProvider will automatically try to load the context from the api exposed by the server library.
     * Pass in a requestInitFn to customize your authentication - e.g. JWT Bearer token authorization header.
     */
    export const App = () => {
        if (signedIn) {
            return (
                <AuthZyinProvider options={{ requestInitFn: getAuthorizationHeadersAsync }}>
                    <MainContent />
                </AuthZyinProvider>
            );
        }
    }

    /**
     * Use the useAuthorize hook to get an authorize function and use policy based authorization in your function components
     */
    const authorize = useAuthorize();
    // pure policy + user based
    const IsCustomer = authorize('IsCustomer');
    // policy + resource + user based
    const barAuthorized = authorize('CanEnterBar' /*policy*/, bar /*resource*);```
```

## How to run the sample project locally
### Prerequisites
Install following pre-reqs:
- [.net core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1)
- [node.js](https://nodejs.org/en/)
- [npm](https://www.npmjs.com/get-npm)
- [yarn](https://classic.yarnpkg.com/en/docs/install)

### Run using VS Code
```
    Open project root folder with VS Code, then F5.
```
**Or**
### Run using Terminal
```Shell
    cd sample
    dotnet run
```
After that simply visit https://localhost:5001. Please **allow popup** since the sample app uses msal.js popup based log in. The first run will take some time since it needs to restore node modules for both the client lib and the SPA app. 

## More technical details
### Requirement evaluation
I chose to use [JSON path](https://goessner.net/articles/JsonPath/) to represent requirement rules, so that requirements can be evaluated on the server, and at the same time can be serialized "as is" to the client for the UI to evaluate - in some kind of an automatic way. Both the asp.net core policy and requirements are sent to client so you can use the same policy definition for your client authorization.

**Server evaluation** - done with [Newtonsoft.Json](https://www.newtonsoft.com/json). Newtonsoft.Json is limited in JSON path parsing, e.g. array filters like this ```[(@.length-1)]``` won't work. I'll research for a better parser in the future.
**Client evlauation** - done with [JsonPath-Plus](https://www.npmjs.com/package/jsonpath-plus).

I was also thinking about supporting serialized Expression Trees so that we can build more powerful requirements with Linq queries but that's much more effort. Will leave it for future considerations.

Currently supported requirement evaluation operators:
- RquiresRole
- Or (with children requirements)
- Equals
- GreaterThan
- GreaterThanOrEqualTo
- Contains

The last 4 operators can be used with **Json Path to a resource object**, or **against a constant value**. Each of them can also be associated with a **Direction**. For example, you can check whether certain collection property in the ```AuthZyinContext.data``` contains some value from a resource, or use a Contains with reverse direction to check whether certain property from the resource object contains a value from the ```AuthZyinContext.data```. This also means we don't need to support LessThan operator since it's just the reverse of GreaterThan.
### Accessing AuthZyinContext
```useAuthorize``` hook can be used in any children components. And it requires access to the ```AuthZyinContext``` object. This is done via [React Context](https://reactjs.org/docs/context.html). ```AuthZyinProvider``` is a wrapper around the React Context. This is a similar pattern as Redux store. The hook and the provider component together hide the React Context details as well as context initialization and value setting from server api to make it much easier to consume.
### Code structure
- [/lib](https://github.com/sidecus/authzyin/tree/master/lib): server library
- [/sample](https://github.com/sidecus/authzyin/tree/master/sample): sample dotnet core project usign the both the server and the client library

Please feel free to open an issue if there is any question. And of course, please help star the project if you find it useful.

# Happy coding. Peace.
MIT © [sidecus](https://github.com/sidecus)
