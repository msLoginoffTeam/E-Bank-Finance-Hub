FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ENV ASPNETCORE_URLS=http://+:8088;https://+:8089
USER $APP_UID
WORKDIR /app
EXPOSE 8088
EXPOSE 8089

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Client/UserPreferences/UserPreferences.csproj", "Client/UserPreferences/"]
RUN dotnet restore "Client/UserPreferences/UserPreferences.csproj"
COPY . .
WORKDIR "/src/Client/UserPreferences"
RUN dotnet build "UserPreferences.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "UserPreferences.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserPreferences.dll"]
