# authzyin
asp.net core authorization library enables both server/client requirement check by defining once and code less

Authorization is both simple and complex.
Often we need to have proper authorization to protect server apis, and at the same time, provide similar authorization on the UI to make our experience more user firendly.
The common practice is to write certain authorization logic twice, once on the server (asp.net core in my case), and once on the client (e.g. in React).

If there are only a few authoriztaion policies/requirements, this is relatively easy and no big deal.
But as our application grows complex, the work required to maintain and make sure they are in sync also grows, and it's no longer a simple task.

I was thinking different options to see whether we can have some libraries to enable the same authorization on both server and client. Aka, write once, use in both places.

This is my first try over the past few days.
I chose to use JsonPath to build requirement validation rules, so taht I can use it on the server, and serialize at the same time to the client for the UI to consume - in some kind of an automatic way.

I haven't got time to move the client library to a seprate project so it's just sitting in sample/ClientApp/src/authzyin folder now.
The server utilities are all in the standalone lib folder.

Currently supported requirement evaluation operations: RquiresRole, Or condition with multiple children requirements, value comparison (equals, greater than etc.).

Any feedback welcome.

Happy coding. Peace.
