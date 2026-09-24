# ── Stage 1: Build ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore separado do código-fonte: aproveita o cache de camadas do Docker
COPY ["EcoCompliance.API/EcoCompliance.API.csproj", "EcoCompliance.API/"]
RUN dotnet restore "EcoCompliance.API/EcoCompliance.API.csproj"

COPY . .
WORKDIR "/src/EcoCompliance.API"
RUN dotnet publish "EcoCompliance.API.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

# ── Stage 2: Runtime ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Versão (commit SHA) injetada pelo pipeline — exibida em GET /
ARG APP_VERSION=local

LABEL org.opencontainers.image.title="eco-compliance-api" \
      org.opencontainers.image.description="API ESG — Cidades ESG Inteligentes (FIAP)" \
      org.opencontainers.image.source="https://github.com/Daikan1/eco-compliance-dotnet"

WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_HTTP_PORTS=8080 \
    APP_VERSION=${APP_VERSION}

COPY --from=build /app/publish .

# Executa como usuário sem privilégios (já existe na imagem oficial)
USER $APP_UID

# Credenciais NUNCA vão na imagem — são passadas por variáveis de ambiente:
#   ConnectionStrings__Oracle, Auth__Password, Auth__JwtKey
ENTRYPOINT ["dotnet", "EcoCompliance.API.dll"]
