# syntax=docker/dockerfile:1.7

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ServerOrchestrator/ServerOrchestrator.csproj ServerOrchestrator/
RUN dotnet restore ServerOrchestrator/ServerOrchestrator.csproj

COPY ServerOrchestrator/ ServerOrchestrator/
RUN dotnet publish ServerOrchestrator/ServerOrchestrator.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8000

ENTRYPOINT ["dotnet", "ServerOrchestrator.dll"]
