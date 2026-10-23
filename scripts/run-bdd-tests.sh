#!/usr/bin/env bash
# ─────────────────────────────────────────────────────────────────────────────
# Executa os testes BDD de API (Reqnroll + Gherkin + JSON Schema).
#
#   ./scripts/run-bdd-tests.sh           # via Docker (só precisa do Docker)
#   ./scripts/run-bdd-tests.sh --local   # via .NET 8 SDK instalado na máquina
#
# Relatórios em ./TestResults: relatorio-bdd.html, bdd-results.trx
# ─────────────────────────────────────────────────────────────────────────────
set -euo pipefail
cd "$(dirname "$0")/.."
mkdir -p TestResults

status=0
if [[ "${1:-}" == "--local" ]]; then
  dotnet test EcoCompliance.BddTests/EcoCompliance.BddTests.csproj -c Release \
    --logger "trx;LogFileName=bdd-results.trx" \
    --logger "console;verbosity=normal" \
    --results-directory TestResults || status=$?
else
  docker build -f Dockerfile.tests -t eco-compliance-bdd .
  docker run --rm -v "$PWD/TestResults:/src/TestResults" eco-compliance-bdd || status=$?
fi

echo
python3 scripts/bdd-summary.py TestResults/bdd-results.trx || true
echo
echo "Relatório HTML: $PWD/TestResults/relatorio-bdd.html"
exit $status
