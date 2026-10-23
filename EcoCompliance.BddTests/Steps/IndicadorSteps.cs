using System.Net;
using EcoCompliance.BddTests.Support;
using Reqnroll;
using Xunit;

namespace EcoCompliance.BddTests.Steps;

[Binding]
public sealed class IndicadorSteps(ApiContext api)
{
    private static readonly DateTime DataMedicao = new(2026, 9, 30, 12, 0, 0, DateTimeKind.Utc);

    [Given("que existe o indicador {string} com valor {double} {string} para a empresa {string}")]
    public async Task ExisteIndicador(string tipo, double valor, string unidade, string empresa)
    {
        await RegistroIndicador(tipo, valor, unidade, empresa);
        Assert.Equal(HttpStatusCode.Created, api.Response!.StatusCode);
    }

    [When("registro o indicador {string} com valor {double} {string} para a empresa {string}")]
    public Task RegistroIndicador(string tipo, double valor, string unidade, string empresa) =>
        Registrar(tipo, valor, unidade, api.CompanyIds[empresa]);

    [When("registro o indicador {string} com valor {double} {string} para a empresa de id {long}")]
    public Task RegistroIndicadorEmpresaId(string tipo, double valor, string unidade, long companyId) =>
        Registrar(tipo, valor, unidade, companyId);

    [When("listo os indicadores ESG")]
    public Task ListoIndicadores() => api.SendAsync(HttpMethod.Get, "/api/esg-indicators");

    private Task Registrar(string tipo, double valor, string unidade, long companyId) =>
        api.SendAsync(HttpMethod.Post, "/api/esg-indicators",
            new { companyId, indicatorType = tipo, value = valor, unit = unidade, measuredAt = DataMedicao });
}
