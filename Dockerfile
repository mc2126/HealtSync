FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar todo el repositorio
COPY . .

# Buscar el archivo .csproj y navegar a su directorio
RUN find . -name "*.csproj" | head -1 | xargs dirname > /tmp/project_dir.txt
RUN cd $(cat /tmp/project_dir.txt) && \
    dotnet restore && \
    dotnet build -c Release -o /app/build

FROM build AS publish-stage
RUN cd $(cat /tmp/project_dir.txt) && \
    dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish-stage /app/publish .
ENTRYPOINT ["dotnet", "HealtSyncpag.dll"]
