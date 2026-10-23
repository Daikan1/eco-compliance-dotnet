using System.Collections.Concurrent;
using System.Text.Json;
using Json.Schema;

namespace EcoCompliance.BddTests.Support;

/// <summary>
/// Testes de contrato: valida o JSON de resposta contra os arquivos
/// JSON Schema (draft 2020-12) em Schemas/.
/// </summary>
public static class ContractValidator
{
    private static readonly ConcurrentDictionary<string, JsonSchema> Cache = new();

    private static readonly EvaluationOptions Options = new()
    {
        OutputFormat = OutputFormat.List,
        RequireFormatValidation = true
    };

    public static void Validate(string contract, JsonElement instance)
    {
        var schema = Cache.GetOrAdd(contract, name =>
            JsonSchema.FromFile(Path.Combine(AppContext.BaseDirectory, "Schemas", $"{name}.schema.json")));

        var result = schema.Evaluate(instance, Options);
        if (result.IsValid) return;

        var errors = (result.Details ?? [])
            .Where(d => d.Errors is { Count: > 0 })
            .SelectMany(d => d.Errors!.Select(e => $"  {d.InstanceLocation}: {e.Value}"));

        throw new Xunit.Sdk.XunitException(
            $"Resposta não segue o contrato '{contract}':\n{string.Join("\n", errors)}\n\nJSON recebido:\n{instance}");
    }
}
