FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["MotoFleetManagement.sln", "./"]
COPY ["src/Mfm.Api/Mfm.Api.csproj", "src/Mfm.Api/"]
COPY ["src/Mfm.Application/Mfm.Application.csproj", "src/Mfm.Application/"]
COPY ["src/Mfm.Domain/Mfm.Domain.csproj", "src/Mfm.Domain/"]
COPY ["src/Mfm.Infrastructure.Data/Mfm.Infrastructure.Data.csproj", "src/Mfm.Infrastructure.Data/"]

RUN dotnet restore "MotoFleetManagement.sln"

COPY . .

WORKDIR "/src/src/Mfm.Api"
RUN dotnet build "Mfm.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mfm.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .

E
ENTRYPOINT ["dotnet", "Mfm.Api.dll"]
