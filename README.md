# untitled-multiplayer-game-server-orchestrator
An orchestrator of online game servers for wenzzzel/untitled-multiplayer-game

# Run
## Plain Docker
`
docker run --rm \                                               ✔  13s  system  
  -p 8000:8000 \
  -v /var/run/docker.sock:/var/run/docker.sock \
  --group-add "$(stat -c '%g' /var/run/docker.sock)" \
  -e ApiKey="put-api-key-here" \
  -e Ghcr__Username="put-username-here" \
  -e Ghcr__Token="put-token-here" \
  server-orchestrator
`
## Docker Compose