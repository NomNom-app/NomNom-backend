FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
ENV PORT=8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NomNomAPI/NomNomAPI.csproj", "NomNomAPI/"]
COPY ["NomNom.Contracts/NomNom.Contracts.csproj", "NomNom.Contracts/"]
RUN dotnet restore "NomNomAPI/NomNomAPI.csproj"
COPY . .
WORKDIR "/src/NomNomAPI"
RUN dotnet build "NomNomAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NomNomAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NomNomAPI.dll"]
