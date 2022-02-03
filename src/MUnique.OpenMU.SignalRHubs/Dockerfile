#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MUnique.OpenMU.SignalRHubs/MUnique.OpenMU.SignalRHubs.csproj", "MUnique.OpenMU.SignalRHubs/"]
RUN dotnet restore "MUnique.OpenMU.SignalRHubs/MUnique.OpenMU.SignalRHubs.csproj"
COPY . .
WORKDIR "/src/MUnique.OpenMU.SignalRHubs"
RUN dotnet build "MUnique.OpenMU.SignalRHubs.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MUnique.OpenMU.SignalRHubs.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MUnique.OpenMU.SignalRHubs.dll"]