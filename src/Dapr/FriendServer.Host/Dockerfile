FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
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
ENTRYPOINT ["dotnet", "MUnique.OpenMU.FriendServer.Host.dll"]
