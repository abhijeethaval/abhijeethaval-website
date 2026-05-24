#!/bin/sh
set -e

# Default API_UPSTREAM if not supplied
if [ -z "$API_UPSTREAM" ]; then
  export API_UPSTREAM="http://abhijeetsite-api"
fi

# Replace ${API_UPSTREAM} in template and write to final nginx.conf.
# Pass '$API_UPSTREAM' to envsubst to ensure it only substitutes that variable and leaves other Nginx variables intact.
envsubst '$API_UPSTREAM' < /etc/nginx/nginx.conf.template > /etc/nginx/nginx.conf

# Start Nginx
exec nginx -g "daemon off;"
