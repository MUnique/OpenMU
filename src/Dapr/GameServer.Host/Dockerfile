FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ["Dapr/GameServer.Host/MUnique.OpenMU.GameServer.Host.csproj", "Dapr/GameServer.Host/"]
RUN dotnet restore "Dapr/GameServer.Host/MUnique.OpenMU.GameServer.Host.csproj"
COPY . .
WORKDIR "/src/Dapr/GameServer.Host"
RUN dotnet build "MUnique.OpenMU.GameServer.Host.csproj" -c Release -o /app/build  -p:ci=true

FROM build AS publish
RUN dotnet publish "MUnique.OpenMU.GameServer.Host.csproj" -c Release -o /app/publish  -p:ci=true

FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MUnique.OpenMU.GameServer.Host.dll"]
