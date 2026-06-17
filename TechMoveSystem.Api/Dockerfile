FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY TechMoveSystem.Api/TechMoveSystem.Api.csproj TechMoveSystem.Api/
RUN dotnet restore TechMoveSystem.Api/TechMoveSystem.Api.csproj
COPY TechMoveSystem.Api/ TechMoveSystem.Api/
WORKDIR /src/TechMoveSystem.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TechMoveSystem.Api.dll"]
