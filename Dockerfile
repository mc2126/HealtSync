FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar TODO el contenido del repositorio
COPY . .

# Buscar automáticamente el archivo .csproj
RUN find . -name "*.csproj" -exec dirname {} \; | head -1 | xargs -I {} cp {}/*.csproj .

# Restaurar paquetes
RUN dotnet restore

# Compilar
RUN dotnet build -c Release -o /app/build

# Publicar
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HealthSyncpag.dll"]
