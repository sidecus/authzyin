#
# build image
#

FROM node:lts-alpine3.9 as build
WORKDIR /usr/app

# copy package and yarn install as a distinct layer
COPY package.json ./
COPY yarn.lock ./
RUN yarn install

# copy everything else and build
COPY ./ ./
RUN yarn build

#
# runtime image
#

FROM nginx:alpine as runtime
COPY --from=build /usr/app/build /usr/share/nginx/html
