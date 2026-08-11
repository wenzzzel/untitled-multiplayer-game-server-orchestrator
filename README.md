# untitled-multiplayer-game-server-orchestrator
An orchestrator of online game servers for wenzzzel/untitled-multiplayer-game. The game server is in a private github repo so you'd need to authenticate using a token.

# Run
## Plain Docker
```
docker run --rm \
  -p 8000:8000 \
  -v /var/run/docker.sock:/var/run/docker.sock \
  --group-add "$(stat -c '%g' /var/run/docker.sock)" \
  -e ApiKey="put-api-key-here" \
  -e Ghcr__Username="put-username-here" \
  -e Ghcr__Token="put-token-here" \
  ghcr.io/wenzzzel/untitled-multiplayer-game-server-orchestrator:latest
```
## Docker Compose

```
services:
  orchestrator:
    image: ghcr.io/wenzzzel/untitled-multiplayer-game-server-orchestrator:latest
    container_name: untitled-multiplayer-game-server-orchestrator
    ports:
      - "8000:8000"
    volumes:
      - /var/run/docker.sock:/var/run/docker.sock
    group_add:
      - "${DOCKER_GID}"
    environment:
      ApiKey: ${ApiKey}
      Ghcr__Username: ${Ghcr__Username}
      Ghcr__Token: ${Ghcr__Token}
    restart: unless-stopped
```

It expects four values via environment variables (or an `.env` file next to `compose.yaml`):

```
DOCKER_GID=<GID of the host's docker socket, e.g. run: stat -c '%g' /var/run/docker.sock>
ApiKey=put-api-key-here
Ghcr__Username=put-username-here
Ghcr__Token=put-token-here
```