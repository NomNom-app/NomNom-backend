FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["./NomNomAPI.csproj", "./"]
RUN dotnet restore "./NomNomAPI.csproj"

COPY . .
WORKDIR "/src/"
RUN dotnet build "NomNomAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NomNomAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NomNomAPI.dll"]
