FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 44405

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ["Dapr/ConnectServer.Host/MUnique.OpenMU.ConnectServer.Host.csproj", "Dapr/ConnectServer.Host/"]
RUN dotnet restore "Dapr/ConnectServer.Host/MUnique.OpenMU.ConnectServer.Host.csproj"
COPY . .
WORKDIR "/src/Dapr/ConnectServer.Host"
RUN dotnet build "MUnique.OpenMU.ConnectServer.Host.csproj" -c Release -o /app/build -p:ci=true

FROM build AS publish
RUN dotnet publish "MUnique.OpenMU.ConnectServer.Host.csproj" -c Release -o /app/publish -p:ci=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MUnique.OpenMU.ConnectServer.Host.dll"]
