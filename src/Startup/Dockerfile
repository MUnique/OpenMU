FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
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

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["Startup/MUnique.OpenMU.Startup.csproj", "Startup/"]
RUN dotnet restore "Startup/MUnique.OpenMU.Startup.csproj"
COPY . .
WORKDIR "/src/Startup"
RUN dotnet build "MUnique.OpenMU.Startup.csproj" -c Release -o /app/build -p:ci=true -v m

FROM build AS publish
RUN dotnet publish "MUnique.OpenMU.Startup.csproj" -c Release -o /app/publish -p:ci=true \
    && mkdir -p /app/publish/Localization \
    && cp -f /src/Web/AdminPanel/Localization/strings.*.json /app/publish/Localization/ 2>/dev/null || true \
    && cp -f /src/Localization/Resources/strings.*.json /app/publish/Localization/ 2>/dev/null || true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN mkdir /app/logs
RUN chmod 777 /app/logs

# Default UID can be overridden at build-time with --build-arg APP_UID=<uid>
ARG APP_UID=1000
USER ${APP_UID}
ENTRYPOINT ["dotnet", "MUnique.OpenMU.Startup.dll", "-autostart"]
