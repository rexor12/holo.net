# syntax=docker/dockerfile:1

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 as build-env
WORKDIR /build

# COPY src/**/*.csproj ./src/

RUN mkdir -p src
COPY Holo.sln .
COPY src ./src
RUN dotnet restore
RUN dotnet publish -c Release /p:HoloPublishFolder=/usr/holo.net/

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 as runtime
WORKDIR /usr/holo.net

COPY --from=build-env /usr/holo.net .

COPY scripts/runtime/wait-for-it.sh .
RUN chmod +x /usr/holo.net/wait-for-it.sh

COPY scripts/runtime/run-migrations.sh .
RUN chmod +x /usr/holo.net/run-migrations.sh

COPY scripts/runtime/run-app.sh .
RUN chmod +x /usr/holo.net/run-app.sh

# STOPSIGNAL SIGINT
# ENTRYPOINT ["/bin/sh", "-c", "./run-migrations.sh && ./run-service-host.sh"]
ENTRYPOINT ["./run-app.sh"]