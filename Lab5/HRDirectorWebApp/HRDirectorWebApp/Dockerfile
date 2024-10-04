FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
ENV DOTNET_URLS=http://+:5208

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["HRDirectorWebApp/HRDirectorWebApp.csproj", "HRDirectorWebApp/"]
COPY ["../Hackathon/Hackathon/Hackathon.csproj", "../Hackathon/Hackathon/"]
RUN dotnet restore "HRDirectorWebApp/HRDirectorWebApp.csproj"
COPY . .
WORKDIR "/src/HRDirectorWebApp"
RUN dotnet build "HRDirectorWebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "HRDirectorWebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HRDirectorWebApp.dll"]
