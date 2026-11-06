FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app

# OpenMU uses localized texts and IANA time zones.
RUN apk add --no-cache icu-libs tzdata
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
EXPOSE 8080

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["Dapr/PlayerPortal.Host/MUnique.OpenMU.PlayerPortal.Host.csproj", "Dapr/PlayerPortal.Host/"]
RUN dotnet restore "Dapr/PlayerPortal.Host/MUnique.OpenMU.PlayerPortal.Host.csproj"
COPY . .
WORKDIR "/src/Dapr/PlayerPortal.Host"
RUN dotnet publish "MUnique.OpenMU.PlayerPortal.Host.csproj" -c Release -o /app/publish -p:ci=true

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "MUnique.OpenMU.PlayerPortal.Host.dll"]
