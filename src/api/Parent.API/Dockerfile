# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["src/api/Parent.API/Parent.API.csproj", "src/api/Parent.API/"]
COPY ["src/modules/parent/Parent.Infrastructure/Parent.Infrastructure.csproj", "src/modules/parent/Parent.Infrastructure/"]
COPY ["src/common/Common.Infrastructure/Common.Infrastructure.csproj", "src/common/Common.Infrastructure/"]
COPY ["src/common/Common.Application/Common.Application.csproj", "src/common/Common.Application/"]
COPY ["src/common/Common.Domain/Common.Domain.csproj", "src/common/Common.Domain/"]
COPY ["src/modules/parent/Parent.Application/Parent.Application.csproj", "src/modules/parent/Parent.Application/"]
COPY ["src/modules/parent/Parent.Domain/Parent.Domain.csproj", "src/modules/parent/Parent.Domain/"]
COPY ["src/modules/parent/Parent.IntegrationEvents/Parent.IntegrationEvents.csproj", "src/modules/parent/Parent.IntegrationEvents/"]
COPY ["src/modules/parent/Parent.Presentation/Parent.Presentation.csproj", "src/modules/parent/Parent.Presentation/"]
COPY ["src/common/Common.Presentation/Common.Presentation.csproj", "src/common/Common.Presentation/"]
COPY ["src/modules/store/Store.IntegrationEvents/Store.IntegrationEvents.csproj", "src/modules/store/Store.IntegrationEvents/"]
RUN dotnet restore "./src/api/Parent.API/Parent.API.csproj"
COPY . .
WORKDIR "/src/src/api/Parent.API"
RUN dotnet build "./Parent.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Parent.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Parent.API.dll"]