FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Portfolio.Api/Portfolio.Api.csproj", "src/Portfolio.Api/"]
RUN dotnet restore "src/Portfolio.Api/Portfolio.Api.csproj"

COPY . .
RUN dotnet publish "src/Portfolio.Api/Portfolio.Api.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "Portfolio.Api.dll"]
