FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5194

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["JuniorWebApp/JuniorWebApp.csproj", "JuniorWebApp/"]
COPY ["../Hackathon/Hackathon/Hackathon.csproj", "../Hackathon/Hackathon/"]
RUN dotnet restore "JuniorWebApp/JuniorWebApp.csproj"
COPY . .
WORKDIR "/src/JuniorWebApp"
RUN dotnet build "JuniorWebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "JuniorWebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JuniorWebApp.dll"]
