# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
WORKDIR /app

RUN mkdir -p /app/Data \
    && chown -R $APP_UID:$APP_UID /app/Data 

RUN mkdir -p /app/Logs \
    && chown -R $APP_UID:$APP_UID /app/Logs

USER $APP_UID

EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["API/ICMarkets.Blockcypher.Api/ICMarkets.Blockcypher.Api.csproj", "API/ICMarkets.Blockcypher.Api/"]

COPY ["Application/ICMarkets.Blockcypher.Application.Configuration/ICMarkets.Blockcypher.Application.Configuration.csproj", "Application/ICMarkets.Blockcypher.Application.Configuration/"]
COPY ["Application/ICMarkets.Blockcypher.Application.DataObjects/ICMarkets.Blockcypher.Application.DataObjects.csproj", "Application/ICMarkets.Blockcypher.Application.DataObjects/"]
COPY ["Application/ICMarkets.Blockcypher.Application.DependencyInjection/ICMarkets.Blockcypher.Application.DependencyInjection.csproj", "Application/ICMarkets.Blockcypher.Application.DependencyInjection/"]
COPY ["Application/ICMarkets.Blockcypher.Application.Interfaces/ICMarkets.Blockcypher.Application.Interfaces.csproj", "Application/ICMarkets.Blockcypher.Application.Interfaces/"]
COPY ["Application/ICMarkets.Blockcypher.Application.Services/ICMarkets.Blockcypher.Application.Services.csproj", "Application/ICMarkets.Blockcypher.Application.Services/"]

COPY ["Domain/ICMarkets.Blockcypher.Domain.Entities/ICMarkets.Blockcypher.Domain.Entities.csproj", "Domain/ICMarkets.Blockcypher.Domain.Entities/"]
COPY ["Domain/ICMarkets.Blockcypher.Domain.Types/ICMarkets.Blockcypher.Domain.Types.csproj", "Domain/ICMarkets.Blockcypher.Domain.Types/"]

COPY ["Infrastructure/ICMarkets.Blockcypher.Infrastructure.ExternalApis/ICMarkets.Blockcypher.Infrastructure.ExternalApis.csproj", "Infrastructure/ICMarkets.Blockcypher.Infrastructure.ExternalApis/"]
COPY ["Infrastructure/ICMarkets.Blockcypher.Infrastructure.Authentication/ICMarkets.Blockcypher.Infrastructure.Authentication.csproj", "Infrastructure/ICMarkets.Blockcypher.Infrastructure.Authentication/"]
COPY ["Infrastructure/ICMarkets.Blockcypher.Infrastructure.Configuration/ICMarkets.Blockcypher.Infrastructure.Configuration.csproj", "Infrastructure/ICMarkets.Blockcypher.Infrastructure.Configuration/"]
COPY ["Infrastructure/ICMarkets.Blockcypher.Infrastructure.DependencyInjection/ICMarkets.Blockcypher.Infrastructure.DependencyInjection.csproj", "Infrastructure/ICMarkets.Blockcypher.Infrastructure.DependencyInjection/"]
COPY ["Infrastructure/ICMarkets.Blockcypher.Infrastructure.Persistance/ICMarkets.Blockcypher.Infrastructure.Persistance.csproj", "Infrastructure/ICMarkets.Blockcypher.Infrastructure.Persistance/"]

RUN dotnet restore "./API/ICMarkets.Blockcypher.Api/ICMarkets.Blockcypher.Api.csproj"
COPY . .
WORKDIR "/src/API/ICMarkets.Blockcypher.Api"
RUN dotnet build "./ICMarkets.Blockcypher.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ICMarkets.Blockcypher.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ICMarkets.Blockcypher.Api.dll"]