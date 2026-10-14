# syntax=docker/dockerfile:1.7-labs

# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
       clang zlib1g-dev libcurl4-openssl-dev

# copy csproj and restore as distinct layers
COPY --parents **/*.csproj .
COPY *.slnx .
RUN dotnet restore

# copy everything else and build app
COPY . .
RUN dotnet publish src -c Release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime-deps:10.0

RUN apt-get update \
     && apt-get install -y --no-install-recommends curl

WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["./LgkProductions.Discord.Activity"]
