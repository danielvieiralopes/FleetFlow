
# --- Estágio 1: Build ---
# Usa a imagem completa do SDK do .NET 8 para compilar a aplicação.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos .csproj e restaura as dependências primeiro.
# Isso aproveita o cache do Docker. Se os projetos não mudarem,
# o Docker não precisará baixar as dependências novamente.
COPY ["src/FleetFlow.Api/FleetFlow.Api.csproj", "FleetFlow.Api/"]
COPY ["src/FleetFlow.Application/FleetFlow.Application.csproj", "FleetFlow.Application/"]
COPY ["src/FleetFlow.Domain/FleetFlow.Domain.csproj", "FleetFlow.Domain/"]
COPY ["src/FleetFlow.Infrastructure/FleetFlow.Infrastructure.csproj", "FleetFlow.Infrastructure/"]
RUN dotnet restore "FleetFlow.Api/FleetFlow.Api.csproj"

# Copia todo o resto do código fonte e compila o projeto.
COPY . .
WORKDIR "/src/FleetFlow.Api"
RUN dotnet build "FleetFlow.Api.csproj" -c Release -o /app/build

# --- Estágio 2: Publish ---
# Publica a aplicação, gerando os arquivos otimizados para produção.
FROM build AS publish
RUN dotnet publish "FleetFlow.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# --- Estágio 3: Final ---
# Cria a imagem final, que é muito menor, usando apenas o runtime do .NET.
# Isso melhora a segurança e o desempenho.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FleetFlow.Api.dll"]
