FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TeamLeadWebApp/TeamLeadWebApp/TeamLeadWebApp.csproj", "TeamLeadWebApp/TeamLeadWebApp/"]
COPY ["Hackathon/Hackathon/Hackathon.csproj", "Hackathon/Hackathon/"]
RUN dotnet restore "TeamLeadWebApp/TeamLeadWebApp/TeamLeadWebApp.csproj"
COPY . .
WORKDIR "/src/TeamLeadWebApp/TeamLeadWebApp"
RUN dotnet build "TeamLeadWebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TeamLeadWebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TeamLeadWebApp.dll"]
