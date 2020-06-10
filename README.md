# authzyin
asp.net core and TypeScript library enables server & client authorization by defining just once and code less.

![.NET Core](https://github.com/sidecus/authzyin/workflows/.NET%20Core/badge.svg)

# Background
Authorization is both simple and complex. Often we need to have proper authorization to protect server apis, and at the same time, provide similar authorization on the UI to make our experience more user firendly. The common practice is to write authorization logic once on the server (asp.net core in my case), and once on the client (e.g. in React).

If there are only a few authoriztaion policies/requirements, this is relatively easy and no big deal. But as our application grows, the work required to maintain and make sure both sides in sync also grows dramatically, and it's likely no longer a simple task.

# What this library does
I was thinking about whether we can have some libraries to enable the same authorization on both server and client - aka - write once, use in both places. This is my first try over the past few days.

I chose to use [JSON path](https://goessner.net/articles/JsonPath/) to represent requirement rules, so that requirements can be evaluated on the server, and at the same time can be serialized "as is" to the client for the UI to evaluate - in some kind of an automatic way. Both the asp.net core policy and requirements are sent to client so you can use the same policy definition for your client authorization.

Since I am a React fan, there is React hooks to help with authorization scenarios for the client as well.

**Server evaluation** - done with [Newtonsoft.Json](https://www.newtonsoft.com/json). Newtonsoft.Json is limited in JSON path parsing, e.g. array filters like this ```[(@.length-1)]``` won't work. I'll research for a better parser in the future.

**Client evlauation** - done with [JsonPath-Plus](https://www.npmjs.com/package/jsonpath-plus).

Currently supported requirement evaluation operations:
- RquiresRole
- Or condition with children requirements
- Value equals
- Value greater than
- Contains

Each operator can be associated with a **Direction**. For example, you can check whether certain properties in the authorization context contains some values from a resource, or use a Contains with a reverse direction to check whether certain properties from the resource contains a value from the authorization context. This also means we don't need to support less than since it's just the reverse of greater than.

# Run locally
## **VSCode**
Open the root folder, then F5.
## **Console**
```
cd sample
dotnet run
```

Server library code is in the /lib folder. Client library code is now in the /sample/ClientApp/src/authzyin folder. To start, check [Requirement.cs](https://github.com/sidecus/authzyin/blob/master/sample/AuthN/Requirements.cs) and [PlaceComponent.tsx](https://github.com/sidecus/authzyin/blob/master/sample/ClientApp/src/components/PlaceComponent.tsx) from the sample project. I'll provide a tutorial later if people think this is useful.
*Kindly note* - you might have more complex authorization scenarios to handle. You don't have to be limited by this library and can mix this with the native asp.net core requirement/handler behaviors.

Feedback welcome - please just open an issue.

# Happy coding. Peace.

*P.S. Please be mindful if you are using System.Text.Json in your code (default asp.net core 3 contract serializer/deserializer). It doesn't support deep polymorphysm (for good reasons - but I do need it in this library), and right now doesn't support field serialization. Keeping this in mind might help you when trouble shooting weird serialization related issues.*
