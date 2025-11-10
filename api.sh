
#!/bin/bash

TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhNTRiMDk0ZS1jNjQ0LTQzNzEtYjYzOC0yNzg4YTJiMGY4MjEiLCJlbWFpbCI6IiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiYTU0YjA5NGUtYzY0NC00MzcxLWI2MzgtMjc4OGEyYjBmODIxIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6ImFnb21lem5pZXRvIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiVXNlciIsImV4cCI6MTc2MjgxMzQyMiwiaXNzIjoiSnVkZ2VBcGkiLCJhdWQiOiJKdWRnZUFwaUF1ZGllbmNlIn0.-hzbC4Xp3kBNiXBK6Bgv2FYChJ6Kc6VdbmqXviHFzBk"  # tu token actual
BASE_URL="http://localhost:5500/api"
# BASE_URL="http://localhost:5500/api"

# Comando: ./api.sh get users
#           ./api.sh post login '{"user":"ale","pass":"1234"}'
#           ./api.sh put data '{"campo":"nuevo"}'
#           ./api.sh delete users/3

METHOD=$1
ENDPOINT=$2
BODY=$3

curl -s -X ${METHOD^^} "$BASE_URL/$ENDPOINT" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "$BODY" | jq
