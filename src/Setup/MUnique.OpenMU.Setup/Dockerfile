FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["Setup/MUnique.OpenMU.Setup/MUnique.OpenMU.Setup.csproj", "Setup/MUnique.OpenMU.Setup/"]
RUN dotnet restore "Setup/MUnique.OpenMU.Setup/MUnique.OpenMU.Setup.csproj"
COPY . .
WORKDIR "/src/Setup/MUnique.OpenMU.Setup"
RUN dotnet build "MUnique.OpenMU.Setup.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MUnique.OpenMU.Setup.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MUnique.OpenMU.Setup.dll"]
