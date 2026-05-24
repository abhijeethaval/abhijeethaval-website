# Stage 1: Build the React application
FROM node:20-alpine AS build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci
COPY . .
RUN npm run build

# Stage 2: Serve using Nginx
FROM nginx:alpine
# Copy built files
COPY --from=build /app/dist /usr/share/nginx/html
# Copy nginx configuration template
COPY nginx.conf.template /etc/nginx/nginx.conf.template
# Copy docker entrypoint script
COPY docker-entrypoint.sh /usr/local/bin/docker-entrypoint.sh
RUN chmod +x /usr/local/bin/docker-entrypoint.sh

EXPOSE 80

ENTRYPOINT ["/usr/local/bin/docker-entrypoint.sh"]
