FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["HealthSyncpag.csproj", "."]
RUN dotnet restore "./HealthSyncpag.csproj"
COPY . .
RUN dotnet build "HealthSyncpag.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HealthSyncpag.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HealthSyncpag.dll"]
