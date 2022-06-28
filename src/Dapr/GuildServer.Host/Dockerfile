FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["Dapr/GuildServer.Host/MUnique.OpenMU.GuildServer.Host.csproj", "Dapr/GuildServer.Host/"]
RUN dotnet restore "Dapr/GuildServer.Host/MUnique.OpenMU.GuildServer.Host.csproj"
COPY . .
WORKDIR "/src/Dapr/GuildServer.Host"
RUN dotnet build "MUnique.OpenMU.GuildServer.Host.csproj" -c Release -o /app/build -p:ci=true

FROM build AS publish
RUN dotnet publish "MUnique.OpenMU.GuildServer.Host.csproj" -c Release -o /app/publish -p:ci=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MUnique.OpenMU.GuildServer.Host.dll"]
