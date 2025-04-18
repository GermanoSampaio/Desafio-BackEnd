# Imagem base para execu��o da aplica��o
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

# Imagem para a constru��o do projeto
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia o arquivo de projeto e restaura depend�ncias
COPY ["MotoService.API/MotoService.API.csproj", "MotoService.API/"]
RUN dotnet restore "MotoService.API/MotoService.API.csproj"

# Copia o c�digo restante e constr�i
COPY . . 
WORKDIR "/src/MotoService.API"
RUN dotnet build "MotoService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publica a aplica��o
FROM build AS publish
RUN dotnet publish "MotoService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Fase final: Prepara o container com a aplica��o pronta para rodar
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copia o appsettings.json (caso tenha necessidades espec�ficas para configura��o)
# COPY MotoService.API/appsettings.json ./appsettings.json

ENTRYPOINT ["dotnet", "MotoService.API.dll"]
