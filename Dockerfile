# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

# UID/GID passed during build (can also default to 1001)
ARG APP_UID=1001
ARG APP_GID=1001

# Create a non-root user and group
RUN groupadd -g $APP_GID appgroup && \
    useradd -m -u $APP_UID -g $APP_GID appuser

# Set up secure directories (as root)
RUN mkdir -p /app /app/plugins /app/dpkeys && \
    chown -R $APP_UID:$APP_GID /app /app/plugins /app/dpkeys

WORKDIR /app
EXPOSE 7236

# Switch to the non-root user
USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/FlowSynx.Pluginregistry/FlowSynx.Pluginregistry.csproj", "src/FlowSynx.Pluginregistry/"]
COPY ["src/FlowSynx.PluginRegistry.Infrastructure/FlowSynx.PluginRegistry.Infrastructure.csproj", "src/FlowSynx.PluginRegistry.Infrastructure/"]
COPY ["src/FlowSynx.PluginRegistry.Application/FlowSynx.PluginRegistry.Application.csproj", "src/FlowSynx.PluginRegistry.Application/"]
COPY ["src/FlowSynx.PluginRegistry.Domain/FlowSynx.PluginRegistry.Domain.csproj", "src/FlowSynx.PluginRegistry.Domain/"]
RUN dotnet restore "./src/FlowSynx.Pluginregistry/FlowSynx.Pluginregistry.csproj"
COPY . .
WORKDIR "/src/src/FlowSynx.Pluginregistry"
RUN dotnet build "./FlowSynx.Pluginregistry.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FlowSynx.Pluginregistry.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FlowSynx.Pluginregistry.dll"]