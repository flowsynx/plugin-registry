# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 7236


# This stage is used to build the service project
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

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FlowSynx.Pluginregistry.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FlowSynx.Pluginregistry.dll"]