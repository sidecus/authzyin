# authzyin
asp.net core and React library enables server & client authorization by defining just once and code less.

![.NET Core](https://github.com/sidecus/authzyin/workflows/.NET%20Core/badge.svg)
![npm](https://github.com/sidecus/authzyin/workflows/npm/badge.svg)

## Motivation
Authorization is both simple and complex. Often we need to have proper authorization to protect server apis, and at the same time, provide similar authorization on the UI to make our experience more user firendly. The common practice is to write authorization logic once on the server (asp.net core in my case), and once on the client (e.g. in React).

If there are only a few authoriztaion policies/requirements, this is relatively easy and no big deal. But as our application grows, the work required to maintain and make sure both sides in sync also grows dramatically, and it's likely no longer a simple task.

I was thinking about whether we can have some libraries to enable the same authorization on both server and client, aka write once, use in both places leveraging the good policy based patterns from asp.net core. This repo is my first try over the past few days. It contains server library and client library to make this simple for you.

*Kindly note*: the server & client libraries can be used together, or standalone. When using together the libraries make the authorization much easier, but it's not a must.

## How to use this library
### Server:
Similar policy/requirement authorization pattern from asp.net core without having to use AuthroziationHandlers, with JSON path based requirement definition and evlauation, as well as built in api to expose the policy/requirement definitions so that client can consume direclty. The only thing additional you need to do compared is to register a scoped ```IAuthZyinContext``` service into the DI container.
1. Define your requirements and policies, like in the [sample project](https://github.com/sidecus/authzyin/blob/master/sample/AuthN/Requirements.cs).
2. Use the AddAuthZyinAuthorization extension method to enable AuthZyin authorization and register your IAuthZyinContext context in Startup.cs.
```CSharp
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
```CSharp
    // Authorize based on policy and resource
    var authResult = await this.authorizationService.AuthorizeAsync(user, bar, "CanEnterBar");
```
**You don't need to worry about serializing and exposing api for this data to be shared with the client**. The library handles that for you automatically once you finishe #1 & #2.
You might also have have more complex authorization scenarios to handle, and you can mix this with standard asp.net core requirement/handler behaviors - as long as those requirements are not inherited from ```AuthZyin.Authorization.Requirement``` they won't be processed by this lib. If you'd like, you can also define your policies/requirements in a json config file and load it during your app startup. It depends on perosnal preference so not included as part of this library.

### Client:
Full policy and requirement based authorization. Built in capability to automatically load policy definitions exposed by the server library. Policies and requirements are resolved and evaluated automatically for you whenever you authorize. *The client library can be used standalone without having to use the server library*.
**More details** about the client lib can be found [here](https://github.com/sidecus/authzyin/tree/master/authzyin.js).
1. Initialize AuthZyinContext (similar as ```createStore``` in Redux, call this globally) and wrap your main component with AuthZyinProvider like below.
```TSX
    initializeAuthZyinContext();

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
Do this after authentication since the api provided by the lib requires authenticated api call by default for security reasons. You can use the optional ```requestInitFn``` callback parameter in the options to provide your authoriztaion headers if needed.

2. Now you can use the useAuthorize hooks in your components like below or as the [sample](https://github.com/sidecus/authzyin/blob/master/authzyin.js/example/src/components/PlaceComponent.tsx):
```TSX
    const authorize = useAuthorize();
    const authorized = authorize('CanEnterBar' /*policy*/, bar /*resource*);
```

## How to run locally
### VSCode
1. Build client lib first
```bash
    cd authzyin.js
    npm install
    npm run build # or 'npm start' if you need HMR
```
2. Use VSCode to open the project root folder, then F5.

### Console
1. Start client lib
```bash
    cd authzyin.js
    npm install
    npm run build # or 'npm start' if you need HMR
```
2. Start the sample asp.net core project
```bash
    cd sample
    dotnet run
```

## A bit more technical details
I chose to use [JSON path](https://goessner.net/articles/JsonPath/) to represent requirement rules, so that requirements can be evaluated on the server, and at the same time can be serialized "as is" to the client for the UI to evaluate - in some kind of an automatic way. Both the asp.net core policy and requirements are sent to client so you can use the same policy definition for your client authorization.

Since I am a React fan, there is React hooks to help with authorization scenarios for the client as well.

**Server evaluation** - done with [Newtonsoft.Json](https://www.newtonsoft.com/json). Newtonsoft.Json is limited in JSON path parsing, e.g. array filters like this ```[(@.length-1)]``` won't work. I'll research for a better parser in the future.

**Client evlauation** - done with [JsonPath-Plus](https://www.npmjs.com/package/jsonpath-plus).

Currently supported requirement evaluation operations:
- RquiresRole
- Or condition with children requirements
- Value equals
- Value greater than
- Value greater than or equal to
- Contains

Each operator can be associated with a **Direction**. For example, you can check whether certain properties in the authorization context contains some values from a resource, or use a Contains with a reverse direction to check whether certain properties from the resource contains a value from the authorization context. This also means we don't need to support less than since it's just the reverse of greater than.

## Code structure
- [/lib](https://github.com/sidecus/authzyin/tree/master/lib): server library
- [/authzyin.js/src](https://github.com/sidecus/authzyin/tree/master/authzyin.js/src): client library
- [/sample](https://github.com/sidecus/authzyin/tree/master/sample): sample project
- [/authzyin.js/example](https://github.com/sidecus/authzyin/tree/master/authzyin.js/example): React SPA used by the sample project

Please feel free to open an issue if there is any question. And let me know if you find this useful.

*P.S. Please be mindful if you are using System.Text.Json in your code (default asp.net core 3 contract serializer/deserializer). It doesn't support deep polymorphysm (for good reasons - but I do need it in this library), and right now doesn't support field serialization. Keeping this in mind might help you when trouble shooting weird serialization related issues.*

# Happy coding. Peace.
MIT © [sidecus](https://github.com/sidecus)
