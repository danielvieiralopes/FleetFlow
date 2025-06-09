# Location: src/FleetFlow.Api/Dockerfile
# Final, Production-Ready Version

# --- Stage 1: Build ---
# Uses the .NET 8 SDK image to build the application.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy .csproj files and restore dependencies first to leverage Docker's layer caching.
COPY ["src/FleetFlow.Api/FleetFlow.Api.csproj", "FleetFlow.Api/"]
COPY ["src/FleetFlow.Application/FleetFlow.Application.csproj", "FleetFlow.Application/"]
COPY ["src/FleetFlow.Domain/FleetFlow.Domain.csproj", "FleetFlow.Domain/"]
COPY ["src/FleetFlow.Infrastructure/FleetFlow.Infrastructure.csproj", "FleetFlow.Infrastructure/"]
RUN dotnet restore "FleetFlow.Api/FleetFlow.Api.csproj"

# --- CORREÇÃO: Copia cada projeto para a sua pasta respetiva ---
# Em vez de um "COPY . .", somos explícitos para garantir um ambiente limpo.
COPY src/FleetFlow.Api/ ./FleetFlow.Api/
COPY src/FleetFlow.Application/ ./FleetFlow.Application/
COPY src/FleetFlow.Domain/ ./FleetFlow.Domain/
COPY src/FleetFlow.Infrastructure/ ./FleetFlow.Infrastructure/

# Constrói o projeto.
WORKDIR "/src/FleetFlow.Api"
RUN dotnet build "FleetFlow.Api.csproj" -c Release -o /app/build

# --- Stage 2: Publish ---
# Publica a aplicação, criando uma saída otimizada para produção.
FROM build AS publish
RUN dotnet publish "FleetFlow.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# --- Stage 3: Final ---
# Cria a imagem final, mais pequena, usando apenas o runtime do ASP.NET.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FleetFlow.Api.dll"]
