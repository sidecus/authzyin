UI components to demo usage of the AuthZyin library.

# /src/authzyin
Contains the client lib to use AuthZyin library. This won't change from project to project. I just haven't got time to move it to a standalone React library yet.
It handles authorization policies and requirements for you automatically from AuthZyin. And provides convient Hooks (```useAuthorize```) to help you with your client authorization.


# /src/api
Contains sample app api and resource data contracts. This will be specific to your app.

# /src/components folder
UI components, using Material UI.

# /src/store
Sample app Redux store definition using react-redux. Also used reselect.js and roth.js for to simplify state boilerplates and management.


You can start from [PlaceComponent](https://github.com/sidecus/authzyin/blob/master/sample/ClientApp/src/components/PlaceComponent.tsx) if you want to jump onto the client authorization part.