FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
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
# The container runs as a non-root user, but /app belongs to root. The data protection
# key ring, which protects the admin panel sessions, needs a writable directory.
RUN mkdir -p /app/data-protection-keys && chmod 777 /app/data-protection-keys
USER $APP_UID
ENTRYPOINT ["dotnet", "MUnique.OpenMU.AdminPanel.Host.dll"]
