FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ["Dapr/LoginServer.Host/MUnique.OpenMU.LoginServer.Host.csproj", "Dapr/LoginServer.Host/"]
RUN dotnet restore "Dapr/LoginServer.Host/MUnique.OpenMU.LoginServer.Host.csproj"
COPY . .
WORKDIR "/src/Dapr/LoginServer.Host"
RUN dotnet build "MUnique.OpenMU.LoginServer.Host.csproj" -c Release -o /app/build -p:ci=true

FROM build AS publish
RUN dotnet publish "MUnique.OpenMU.LoginServer.Host.csproj" -c Release -o /app/publish -p:ci=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MUnique.OpenMU.LoginServer.Host.dll"]
