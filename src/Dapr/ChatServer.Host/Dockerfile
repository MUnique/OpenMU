FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80

RUN addgroup -S dotnet && adduser -S dotnet -G dotnet \
    && apk add --no-cache dumb-init

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["Dapr/ChatServer.Host/MUnique.OpenMU.ChatServer.Host.csproj", "Dapr/ChatServer.Host/"]
RUN dotnet restore "Dapr/ChatServer.Host/MUnique.OpenMU.ChatServer.Host.csproj"
COPY . .
WORKDIR "/src/Dapr/ChatServer.Host"
RUN dotnet build "MUnique.OpenMU.ChatServer.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MUnique.OpenMU.ChatServer.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
RUN chown dotnet /app
COPY --chown=dotnet:dotnet-group --from=publish /app/publish .

USER dotnet
CMD ["dumb-init", "dotnet", "MUnique.OpenMU.ChatServer.Host.dll"]
