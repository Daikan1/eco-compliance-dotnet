#!/usr/bin/env python3
"""Gera um resumo em Markdown dos cenários BDD a partir do arquivo TRX.

Uso:  python3 scripts/bdd-summary.py TestResults/bdd-results.trx
No GitHub Actions a saída vai para $GITHUB_STEP_SUMMARY (página da execução).
"""
import re
import sys
import xml.etree.ElementTree as ET

NS = {"t": "http://microsoft.com/schemas/VisualStudio/TeamTest/2010"}
ICON = {"Passed": "✅", "Failed": "❌"}

root = ET.parse(sys.argv[1]).getroot()
counters = root.find(".//t:Counters", NS).attrib

# Funcionalidade de cada teste (classe gerada pelo Reqnroll a partir do .feature)
feature_of = {
    d.get("id"): d.find("t:TestMethod", NS).get("className").split(".")[-1].removesuffix("Feature")
    for d in root.findall(".//t:UnitTest", NS)
}

results = sorted(
    root.findall(".//t:UnitTestResult", NS),
    key=lambda r: (feature_of.get(r.get("testId"), ""), r.get("testName")),
)

print("## Testes BDD de API: Reqnroll + Gherkin + JSON Schema\n")
print(f"**{counters['passed']} aprovados**, {counters['failed']} falhas, {counters['total']} cenários executados\n")
print("| Resultado | Funcionalidade | Cenário |")
print("|---|---|---|")
for r in results:
    name = re.sub(r',? ?__pickleIndex: "\d+", exampleTags: \[[^\]]*\]', "", r.get("testName"))
    name = name.replace("()", "").replace("|", "/")
    outcome = r.get("outcome")
    print(f"| {ICON.get(outcome, '⚠️')} {outcome} | {feature_of.get(r.get('testId'), '')} | {name} |")
