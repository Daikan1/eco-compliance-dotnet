#!/usr/bin/env bash
# ─────────────────────────────────────────────────────────────────────────────
# Cria a infraestrutura na Azure e configura o GitHub para o pipeline CI/CD:
#   • 1 Resource Group + 1 App Service Plan (Linux)
#   • 2 Web Apps for Containers: <prefixo>-staging e <prefixo>-production
#   • App settings de cada ambiente (connection string, JWT, porta...)
#   • GitHub Environments "staging" e "production" com o secret
#     AZURE_WEBAPP_PUBLISH_PROFILE e a variável AZURE_WEBAPP_NAME
#
# Pré-requisitos:  az login   e   gh auth login
# Uso:             ./scripts/azure-setup.sh
# Variáveis opcionais: RG, LOCATION, PLAN, SKU, PREFIX, REPO
# ─────────────────────────────────────────────────────────────────────────────
set -euo pipefail

RG="${RG:-rg-eco-compliance}"
LOCATION="${LOCATION:-eastus2}"       # Azure for Students (FIAP) libera: eastus2, southcentralus, canadacentral, mexicocentral, chilecentral
PLAN="${PLAN:-plan-eco-compliance}"
SKU="${SKU:-B1}"
PREFIX="${PREFIX:-eco-compliance-rm565199}"
REPO="${REPO:-Daikan1/eco-compliance-dotnet}"
# Imagem provisória — o pipeline substitui pela imagem do GHCR no primeiro deploy
PLACEHOLDER_IMAGE="mcr.microsoft.com/dotnet/samples:aspnetapp"

command -v az >/dev/null || { echo "Azure CLI não encontrado: brew install azure-cli"; exit 1; }
command -v gh >/dev/null || { echo "GitHub CLI não encontrado: brew install gh"; exit 1; }

echo "── Credenciais (não ficam salvas em nenhum arquivo) ──"
read -rp  "Usuário Oracle FIAP (ex: RM565199): " ORACLE_USER
read -rsp "Senha Oracle FIAP: " ORACLE_PASS; echo
read -rsp "Senha do admin da API (POST /api/auth/login): " ADMIN_PASS; echo
read -rp  "PAT do GitHub com read:packages (Enter para pular se o pacote GHCR for público): " GHCR_PAT

ORACLE_CONN="Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=oracle.fiap.com.br)(PORT=1521))(CONNECT_DATA=(SID=ORCL)));User Id=${ORACLE_USER};Password=${ORACLE_PASS};"

echo "── Registrando o provedor Microsoft.Web (App Service) ──"
az provider register -n Microsoft.Web --wait

echo "── Resource Group e App Service Plan ──"
az group create -n "$RG" -l "$LOCATION" -o none
az appservice plan create -n "$PLAN" -g "$RG" --is-linux --sku "$SKU" -o none

for ENV in staging production; do
  APP="${PREFIX}-${ENV}"
  case "$ENV" in
    staging)    ASPNET_ENV="Staging";    APPLY_MIGRATIONS="true"  ;;
    production) ASPNET_ENV="Production"; APPLY_MIGRATIONS="false" ;;
  esac

  echo "── Web App: $APP ($ASPNET_ENV) ──"
  az webapp create -g "$RG" -p "$PLAN" -n "$APP" --container-image-name "$PLACEHOLDER_IMAGE" -o none

  # Chave JWT diferente por ambiente
  JWT_KEY="$(openssl rand -base64 48)"

  az webapp config appsettings set -g "$RG" -n "$APP" -o none --settings \
    ASPNETCORE_ENVIRONMENT="$ASPNET_ENV" \
    WEBSITES_PORT=8080 \
    ConnectionStrings__Oracle="$ORACLE_CONN" \
    Database__ApplyMigrations="$APPLY_MIGRATIONS" \
    Auth__Username=admin \
    Auth__Password="$ADMIN_PASS" \
    Auth__JwtKey="$JWT_KEY"

  if [[ -n "$GHCR_PAT" ]]; then
    az webapp config appsettings set -g "$RG" -n "$APP" -o none --settings \
      DOCKER_REGISTRY_SERVER_URL=https://ghcr.io \
      DOCKER_REGISTRY_SERVER_USERNAME="${REPO%%/*}" \
      DOCKER_REGISTRY_SERVER_PASSWORD="$GHCR_PAT"
  fi

  az webapp config set -g "$RG" -n "$APP" --always-on true -o none || true

  # O publish profile usa autenticação básica no SCM — desabilitada por padrão
  az resource update -g "$RG" --namespace Microsoft.Web --resource-type basicPublishingCredentialsPolicies \
    --name scm --parent "sites/$APP" --set properties.allow=true -o none

  PROFILE="$(az webapp deployment list-publishing-profiles -g "$RG" -n "$APP" --xml)"

  echo "── GitHub Environment: $ENV ──"
  if [[ "$ENV" == "production" ]]; then
    # Aprovação manual antes do deploy em produção
    REVIEWER_ID="$(gh api user -q .id)"
    echo "{\"reviewers\":[{\"type\":\"User\",\"id\":${REVIEWER_ID}}]}" \
      | gh api -X PUT "repos/$REPO/environments/$ENV" --input - >/dev/null \
      || { echo "⚠️  Não foi possível exigir aprovação (repo privado sem plano pago?). Criando sem revisor.";
           gh api -X PUT "repos/$REPO/environments/$ENV" >/dev/null; }
  else
    gh api -X PUT "repos/$REPO/environments/$ENV" >/dev/null
  fi

  gh secret   set AZURE_WEBAPP_PUBLISH_PROFILE --env "$ENV" --repo "$REPO" --body "$PROFILE"
  gh variable set AZURE_WEBAPP_NAME            --env "$ENV" --repo "$REPO" --body "$APP"

  echo "✅ $ENV → https://${APP}.azurewebsites.net"
done

echo
echo "Pronto! Faça um push na branch main para disparar o pipeline."
echo "Para apagar tudo depois da avaliação:  az group delete -n $RG --yes"
