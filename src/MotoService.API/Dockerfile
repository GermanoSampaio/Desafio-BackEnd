FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MotoService.API/MotoService.API.csproj", "MotoService.API/"]
RUN dotnet restore "./MotoService.API/MotoService.API.csproj"
COPY . .
WORKDIR "/src/MotoService.API"
RUN dotnet build "./MotoService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MotoService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY MotoService.API/appsettings.json ./appsettings.json

ENTRYPOINT ["dotnet", "MotoService.API.dll"]
