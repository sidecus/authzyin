# authzyin
asp.net core authorization library enables both server/client requirement check by defining once and code less

Authorization is both simple and complex.
Often we need to have proper authorization to protect server apis, and at the same time, provide similar authorization on the UI to make our experience more user firendly.
The common practice is to write authorization logic once on the server (asp.net core in my case), and once on the client (e.g. in React).

If there are only a few authoriztaion policies/requirements, this is relatively easy and no big deal.
But as our application grows, the work required to maintain and make sure both sides in sync also grows dramatically, and it's likely no longer a simple task.

I was thinking about whether we can have some libraries to enable the same authorization on both server and client - aka - write once, use in both places. This is my first try
over the past few days.

I chose to use [JSON path](https://goessner.net/articles/JsonPath/) to represent requirement rules, so that requirements can be evaluated on the server,
and at the same time can be serialized "as is" to the client for the UI to evaluate - in some kind of an automatic way.
Both the asp.net core policy and requirements are sent to client so you can use the same policy definition for your client authorization.
Since I am a React fan, there is React hooks to help with authorization scenarios for the client as well.

Server evaluation is done with [Newtonsoft.Json](https://www.newtonsoft.com/json). The new System.Text.Json is still limited in functionality and unfortunately doesn't work here.

Client evlauation is done with [JsonPath-Plus](https://www.npmjs.com/package/jsonpath-plus).

Currently supported requirement evaluation operations:
- RquiresRole
- Or condition with children requirements
- Value equals
- Value greater than
- Contains

Each operator can be associated with a "direction". For example, you can check whether certain properties in the authorization context contains some values from a resource,
or use a reverse contains to check whether certain properties from the resource contains a value from the authorization context.

To start, check ```Requirement.cs``` and ```Home.tsx``` from the sample project. I'll provide a tutorial later if people think this is useful.
*Kindly note* - you might have more complex authorization scenarios to handle. You don't have to be limited by this library and can mix this with the native asp.net core requirement/handler behaviors.

JSON path is probably not the best option but definitely the easiest one to implement. I was also thinking about leveraging .net expression trees so that we can use LINQ like queries to have
more powerful evaluation behaviors. That's non trivial so I'll save it for later.
I haven't got time to move the client library to a seprate project so it's just sitting in sample/ClientApp/src/authzyin folder now. The server utilities are all in the standalone lib folder.

Any feedback welcome - feel free open an issue.

Happy coding. Peace.
