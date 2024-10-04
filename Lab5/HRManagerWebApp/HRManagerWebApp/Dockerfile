FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["HRManagerWebApp/HRManagerWebApp.csproj", "HRManagerWebApp/"]
COPY ["Hackathon/Hackathon.csproj", "Hackathon/"]
RUN dotnet restore "HRManagerWebApp/HRManagerWebApp.csproj"
COPY . .
WORKDIR "/src/HRManagerWebApp"
RUN dotnet build "HRManagerWebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "HRManagerWebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HRManagerWebApp.dll"]
