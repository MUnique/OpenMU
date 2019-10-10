FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build

# Requirements for the typescript compiler
RUN apt-get update
RUN apt-get install npm -y
RUN npm i -g typescript

COPY src/ ./app/src
COPY tests/ ./app/tests
COPY doc/ ./app/doc

WORKDIR /app
RUN dotnet publish src/Startup/MUnique.OpenMU.Startup.csproj -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime

EXPOSE 1234
EXPOSE 55901
EXPOSE 55902
EXPOSE 55903
EXPOSE 44405
EXPOSE 55980

WORKDIR /app
COPY --from=build /app/out ./
COPY --from=build /app/bin/Debug/wwwroot/content/js ./wwwroot/content/js
ENTRYPOINT ["dotnet", "MUnique.OpenMU.Startup.dll", "-autostart"]