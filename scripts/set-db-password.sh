#!/usr/bin/env bash
# ─────────────────────────────────────────────────────────────────────────────
# Atualiza a senha do Oracle FIAP no appsettings.Development.json (local)
# e na connection string dos Web Apps de staging e produção na Azure.
# A senha é digitada sem eco e não fica salva em nenhum outro lugar.
#
# Uso:  ./scripts/set-db-password.sh
# ─────────────────────────────────────────────────────────────────────────────
set -euo pipefail
export PYTHONWARNINGS=ignore

RG="${RG:-rg-eco-compliance}"
PREFIX="${PREFIX:-eco-compliance-rm565199}"
DIR="$(cd "$(dirname "$0")/.." && pwd)"
FILE="$DIR/EcoCompliance.API/appsettings.Development.json"

read -rsp "Nova senha Oracle FIAP: " NOVA_SENHA </dev/tty; echo
read -rsp "Confirme a senha:       " CONFIRMA  </dev/tty; echo
[[ -n "$NOVA_SENHA" ]]              || { echo "❌ Senha vazia. Nada foi alterado."; exit 1; }
[[ "$NOVA_SENHA" == "$CONFIRMA" ]] || { echo "❌ As senhas não conferem. Nada foi alterado."; exit 1; }

NOVA_SENHA="$NOVA_SENHA" python3 - "$FILE" <<'EOF'
import json, os, re, sys
f = sys.argv[1]
d = json.load(open(f))
d["ConnectionStrings"]["Oracle"] = re.sub(
    r"Password=[^;]*", lambda m: "Password=" + os.environ["NOVA_SENHA"], d["ConnectionStrings"]["Oracle"])
json.dump(d, open(f, "w"), indent=2)
EOF
unset NOVA_SENHA CONFIRMA
echo "✓ appsettings.Development.json atualizado"

CONN="$(python3 -c 'import json,sys; print(json.load(open(sys.argv[1]))["ConnectionStrings"]["Oracle"])' "$FILE")"
for ENV in staging production; do
  az webapp config appsettings set -g "$RG" -n "$PREFIX-$ENV" \
    --settings "ConnectionStrings__Oracle=$CONN" -o none 2>/dev/null
  echo "✓ $ENV atualizado (o app reinicia sozinho em 1–2 min)"
done
