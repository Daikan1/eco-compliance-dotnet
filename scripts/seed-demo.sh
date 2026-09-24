#!/usr/bin/env bash
# ─────────────────────────────────────────────────────────────────────────────
# Cadastra uma empresa de demonstração em staging e em produção via API
# (login JWT + POST /api/companies) e lista o resultado.
#
# Uso:  ./scripts/seed-demo.sh
# ─────────────────────────────────────────────────────────────────────────────
set -euo pipefail

PREFIX="${PREFIX:-eco-compliance-rm565199}"

read -rsp "Senha do admin da API: " ADMIN_PASS </dev/tty; echo
[[ -n "$ADMIN_PASS" ]] || { echo "❌ Senha vazia."; exit 1; }

for ENV in staging production; do
  BASE="https://${PREFIX}-${ENV}.azurewebsites.net"
  echo "── $ENV ($BASE)"

  LOGIN_BODY="$(ADMIN_PASS="$ADMIN_PASS" python3 -c 'import json,os; print(json.dumps({"username":"admin","password":os.environ["ADMIN_PASS"]}))')"
  TOKEN="$(curl -s -m 60 -X POST "$BASE/api/auth/login" -H 'Content-Type: application/json' -d "$LOGIN_BODY" \
           | python3 -c 'import sys,json; print(json.load(sys.stdin).get("token",""))' 2>/dev/null || true)"
  [[ -n "$TOKEN" ]] || { echo "❌ Login falhou em $ENV (senha de admin incorreta?)"; continue; }
  echo "✓ login OK"

  case "$ENV" in
    staging)    BODY='{"name":"Prefeitura de São Paulo","cnpj":"46395000000139","sector":"Governo"}' ;;
    production) BODY='{"name":"Secretaria do Verde e Meio Ambiente","cnpj":"46392130000118","sector":"Meio Ambiente"}' ;;
  esac

  curl -s -m 60 -o /dev/null -w "✓ POST /api/companies → HTTP %{http_code}\n" \
    -X POST "$BASE/api/companies" -H "Authorization: Bearer $TOKEN" -H 'Content-Type: application/json' -d "$BODY"
done
unset ADMIN_PASS TOKEN LOGIN_BODY

echo
echo "── GET /api/companies (staging)"
curl -s "https://${PREFIX}-staging.azurewebsites.net/api/companies"; echo
