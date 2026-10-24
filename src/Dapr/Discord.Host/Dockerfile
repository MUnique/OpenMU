FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app

# OpenMU uses localized texts, which require the culture data of ICU.
# The alpine images don't contain it and run in the globalization-invariant mode by default.
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
EXPOSE 8080

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["Dapr/Discord.Host/MUnique.OpenMU.Discord.Host.csproj", "Dapr/Discord.Host/"]
RUN dotnet restore "Dapr/Discord.Host/MUnique.OpenMU.Discord.Host.csproj"
COPY . .
WORKDIR "/src/Dapr/Discord.Host"
RUN dotnet build "MUnique.OpenMU.Discord.Host.csproj" -c Release -o /app/build -p:ci=true

FROM build AS publish
RUN dotnet publish "MUnique.OpenMU.Discord.Host.csproj" -c Release -o /app/publish -p:ci=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "MUnique.OpenMU.Discord.Host.dll"]
