FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Mfm.Api/Mfm.Api.csproj", "src/Mfm.Api/"]
COPY ["src/Mfm.Application/Mfm.Application.csproj", "src/Mfm.Application/"]
COPY ["src/Mfm.Domain/Mfm.Domain.csproj", "src/Mfm.Domain/"]
COPY ["src/Mfm.Infrastructure.Data/Mfm.Infrastructure.Data.csproj", "src/Mfm.Infrastructure.Data/"]
RUN dotnet restore "./src/Mfm.Api/Mfm.Api.csproj"
COPY . .
WORKDIR "/src/src/Mfm.Api"
RUN dotnet build "./Mfm.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Mfm.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mfm.Api.dll"]