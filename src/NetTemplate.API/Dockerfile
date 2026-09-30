# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/NetTemplate.API/NetTemplate.API.csproj", "src/NetTemplate.API/"]
COPY ["src/NetTemplate.Infrastructure/NetTemplate.Infrastructure.csproj", "src/NetTemplate.Infrastructure/"]
COPY ["src/NetTemplate.Application/NetTemplate.Application.csproj", "src/NetTemplate.Application/"]
COPY ["src/NetTemplate.Core/NetTemplate.Core.csproj", "src/NetTemplate.Core/"]
RUN dotnet restore "./src/NetTemplate.API/NetTemplate.API.csproj"
COPY . .
WORKDIR "/src/src/NetTemplate.API"
RUN dotnet build "./NetTemplate.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NetTemplate.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Setup TimeZone as Argentina
ENV TZ=America/Buenos_Aires

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NetTemplate.API.dll"]