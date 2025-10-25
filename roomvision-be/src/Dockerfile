FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["roomvision.presentation/roomvision.presentation.csproj", "roomvision.presentation/"]
COPY ["roomvision.infrastructure/roomvision.infrastructure.csproj", "roomvision.infrastructure/"]
COPY ["roomvision.domain/roomvision.domain.csproj", "roomvision.domain/"]
COPY ["roomvision.application/roomvision.application.csproj", "roomvision.application/"]

RUN dotnet restore "roomvision.presentation/roomvision.presentation.csproj"

COPY . .

WORKDIR "/src/roomvision.presentation"
RUN dotnet build "roomvision.presentation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "roomvision.presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final

WORKDIR "/app"
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "roomvision.presentation.dll"]