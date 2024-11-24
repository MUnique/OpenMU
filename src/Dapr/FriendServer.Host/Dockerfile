FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src
COPY ["Dapr/FriendServer.Host/MUnique.OpenMU.FriendServer.Host.csproj", "Dapr/FriendServer.Host/"]
RUN dotnet restore "Dapr/FriendServer.Host/MUnique.OpenMU.FriendServer.Host.csproj"
COPY . .
WORKDIR "/src/Dapr/FriendServer.Host"
RUN dotnet build "MUnique.OpenMU.FriendServer.Host.csproj" -c Release -o /app/build -p:ci=true

FROM build AS publish
RUN dotnet publish "MUnique.OpenMU.FriendServer.Host.csproj" -c Release -o /app/publish -p:ci=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "MUnique.OpenMU.FriendServer.Host.dll"]
