ARG ASPNET_IMAGE=mcr.microsoft.com/dotnet/aspnet:9.0-alpine
ARG SDK_IMAGE=mcr.microsoft.com/dotnet/sdk:9.0-alpine
FROM ${ASPNET_IMAGE} AS base
# Enable full globalization support (avoid invariant-only mode)
RUN apk add --no-cache icu-libs icu-data-full tzdata \
    && rm -rf /var/cache/apk/*
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
WORKDIR /app
EXPOSE 8080
EXPOSE 55901
EXPOSE 55902
EXPOSE 55903
EXPOSE 55904
EXPOSE 55905
EXPOSE 55906
EXPOSE 44405
EXPOSE 55980

FROM --platform=$BUILDPLATFORM ${SDK_IMAGE} AS build
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1 \
    NUGET_XMLDOC_MODE=skip
WORKDIR /src
COPY ["Startup/MUnique.OpenMU.Startup.csproj", "Startup/"]
RUN dotnet restore "Startup/MUnique.OpenMU.Startup.csproj"
COPY . .
WORKDIR "/src/Startup"
RUN dotnet build "MUnique.OpenMU.Startup.csproj" -c Release -o /app/build -p:ci=true -v m

FROM build AS publish
RUN dotnet publish "MUnique.OpenMU.Startup.csproj" -c Release -o /app/publish -p:ci=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN mkdir /app/logs
RUN chmod 777 /app/logs

# Default UID can be overridden at build-time with --build-arg APP_UID=<uid>
ARG APP_UID=1000
USER ${APP_UID}
ENTRYPOINT ["dotnet", "MUnique.OpenMU.Startup.dll", "-autostart"]
