FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
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

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src
COPY ["Startup/MUnique.OpenMU.Startup.csproj", "Startup/"]
RUN dotnet restore "Startup/MUnique.OpenMU.Startup.csproj"
COPY . .
WORKDIR "/src/Startup"
RUN dotnet build "MUnique.OpenMU.Startup.csproj" -c Release -o /app/build -p:ci=true

FROM build AS publish
RUN dotnet publish "MUnique.OpenMU.Startup.csproj" -c Release -o /app/publish -p:ci=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN mkdir /app/logs
RUN chmod 777 /app/logs

USER $APP_UID
ENTRYPOINT ["dotnet", "MUnique.OpenMU.Startup.dll", "-autostart"]
