using System;
using System.Collections.Generic;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

const string apiKey = "some-long-random-secret"; // move to config/env var
const string imageName = "ghcr.io/wenzzzel/untitled-multiplayer-game";
const string imageTag = "latest";
const string containerPort = "7777/udp"; // match your server's actual port/protocol

var dockerClient = new DockerClientConfiguration(
    new Uri("unix:///var/run/docker.sock"))
    .CreateClient();

app.MapPost("/lobbies", async (HttpContext ctx) =>
{
    if (!ctx.Request.Headers.TryGetValue("X-Api-Key", out var key) || key != apiKey)
        return Results.Unauthorized();

    var lobbyId = Guid.NewGuid().ToString("N")[..8];

    // Pull the latest image before starting the container. No-op if already up to date.
    await dockerClient.Images.CreateImageAsync(
        new ImagesCreateParameters { FromImage = imageName, Tag = imageTag },
        authConfig: null,
        progress: new Progress<JSONMessage>());

    var createResponse = await dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
    {
        Image = $"{imageName}:{imageTag}",
        Name = $"lobby-{lobbyId}",
        Env = new List<string> { $"LOBBY_ID={lobbyId}" },
        ExposedPorts = new Dictionary<string, EmptyStruct>
        {
            { containerPort, default }
        },
        HostConfig = new HostConfig
        {
            AutoRemove = true, // container deletes itself once the process exits
            PortBindings = new Dictionary<string, IList<PortBinding>>
            {
                { containerPort, new List<PortBinding> { new() { HostPort = "0" } } } // "0" = let Docker pick a free port
            }
        }
    });

    await dockerClient.Containers.StartContainerAsync(createResponse.ID, new ContainerStartParameters());

    // Inspect to find out which host port Docker actually assigned
    var inspect = await dockerClient.Containers.InspectContainerAsync(createResponse.ID);
    var hostPort = inspect.NetworkSettings.Ports[containerPort][0].HostPort;

    return Results.Ok(new { lobbyId, port = hostPort });
});

app.Run("http://0.0.0.0:8000");