FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app

RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

EXPOSE 8080
EXPOSE 55901
EXPOSE 55902
EXPOSE 55903
EXPOSE 55904
EXPOSE 55905
EXPOSE 55906
EXPOSE 44405
EXPOSE 55980

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY Directory.Packages.props .
COPY Directory.Build.props .

COPY Startup/*.csproj Startup/
COPY Persistence/**/*.csproj Persistence/

RUN dotnet restore "Startup/MUnique.OpenMU.Startup.csproj"

COPY . .

WORKDIR /src/Startup

RUN dotnet build "MUnique.OpenMU.Startup.csproj" \
    -c Release \
    -o /app/build \
    -p:ci=true

FROM build AS publish
RUN dotnet publish "MUnique.OpenMU.Startup.csproj" \
    -c Release \
    -o /app/publish \
    -p:ci=true

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

# The container runs as a non-root user, but /app belongs to root. Directories the
# application writes to have to be created and made writable beforehand - the data
# protection key ring among them, which protects the admin panel sessions.
RUN mkdir -p /app/logs /app/data-protection-keys && chmod 777 /app/logs /app/data-protection-keys

ARG APP_UID=1000
USER ${APP_UID}

ENTRYPOINT ["dotnet", "MUnique.OpenMU.Startup.dll", "-autostart"]