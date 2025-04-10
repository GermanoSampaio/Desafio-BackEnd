# Imagem base para execução da aplicação
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

# Imagem para a construção do projeto
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia o arquivo de projeto e restaura dependências
COPY ["MotoService.API/MotoService.API.csproj", "MotoService.API/"]
RUN dotnet restore "MotoService.API/MotoService.API.csproj"

# Copia o código restante e constrói
COPY . . 
WORKDIR "/src/MotoService.API"
RUN dotnet build "MotoService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publica a aplicação
FROM build AS publish
RUN dotnet publish "MotoService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Fase final: Prepara o container com a aplicação pronta para rodar
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copia o appsettings.json (caso tenha necessidades específicas para configuração)
# COPY MotoService.API/appsettings.json ./appsettings.json

ENTRYPOINT ["dotnet", "MotoService.API.dll"]
