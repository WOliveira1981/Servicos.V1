# Padrao Builder: o Node compila o React/Webpack e o Nginx serve somente os
# arquivos estaticos gerados. A imagem final fica separada do ambiente de build.
FROM node:22-alpine AS build
WORKDIR /app
ARG API_BASE_URL=/api
ENV API_BASE_URL=$API_BASE_URL

COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

FROM nginx:1.27-alpine AS runtime
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
