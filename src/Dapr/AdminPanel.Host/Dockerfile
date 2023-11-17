FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ["Dapr/AdminPanel.Host/MUnique.OpenMU.AdminPanel.Host.csproj", "Dapr/AdminPanel.Host/"]
RUN dotnet restore "Dapr/AdminPanel.Host/MUnique.OpenMU.AdminPanel.Host.csproj"
COPY . .
WORKDIR "/src/Dapr/AdminPanel.Host"
RUN dotnet build "MUnique.OpenMU.AdminPanel.Host.csproj" -c Release -o /app/build -p:ci=true

FROM build AS publish
RUN dotnet publish "MUnique.OpenMU.AdminPanel.Host.csproj" -c Release -o /app/publish -p:ci=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MUnique.OpenMU.AdminPanel.Host.dll"]
