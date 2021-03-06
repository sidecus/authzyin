#
# build stage
#

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /authzyin

# copy csproj and restore as distinct layers to use build cache
COPY authzyin.sln .
COPY lib/lib.csproj lib/
COPY sample/sample.csproj sample/
COPY test/test.csproj test/
RUN dotnet restore

# copy everything else and build app
# we are skiping SPA in this image. SPA is handled by a different docker image with nginx.
# check sample.csproj for how SkipSPA is defined.
COPY lib/ lib/
COPY sample/ sample/
WORKDIR /authzyin/sample
RUN dotnet publish -c Release /p:SkipSPA=true

#
# runtime stage
#

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
COPY --from=build /authzyin/sample/bin/Release/netcoreapp3.1/publish/ App/
WORKDIR /App
ENTRYPOINT ["dotnet", "sample.dll"]
