using System.Net;
using EcoCompliance.BddTests.Support;
using Reqnroll;
using Xunit;

namespace EcoCompliance.BddTests.Steps;

[Binding]
public sealed class ComplianceSteps(ApiContext api)
{
    private static readonly DateTime DataRelatorio = new(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc);

    // Dados do último registro criado, reaproveitados no PUT
    private long _companyId;
    private string _tipo = string.Empty;

    [Given("que existe o compliance {string} com status {string} para a empresa {string}")]
    public async Task ExisteCompliance(string tipo, string status, string empresa)
    {
        await RegistroCompliance(tipo, status, empresa);
        Assert.Equal(HttpStatusCode.Created, api.Response!.StatusCode);
        api.LastComplianceId = api.Json.GetProperty("id").GetInt64();
    }

    [When("registro o compliance {string} com status {string} para a empresa {string}")]
    public Task RegistroCompliance(string tipo, string status, string empresa) =>
        Registrar(tipo, status, api.CompanyIds[empresa]);

    [When("registro o compliance {string} com status {string} para a empresa de id {long}")]
    public Task RegistroComplianceEmpresaId(string tipo, string status, long companyId) =>
        Registrar(tipo, status, companyId);

    [When("atualizo o status desse registro de compliance para {string}")]
    public Task AtualizoStatus(string novoStatus) =>
        api.SendAsync(HttpMethod.Put, $"/api/compliance/{api.LastComplianceId}", Corpo(_companyId, _tipo, novoStatus));

    [When("atualizo o registro de compliance de id {long} para a empresa {string}")]
    public Task AtualizoInexistente(long id, string empresa) =>
        api.SendAsync(HttpMethod.Put, $"/api/compliance/{id}", Corpo(api.CompanyIds[empresa], "Licença Ambiental", "CONFORME"));

    [When("removo esse registro de compliance")]
    public Task RemovoRegistro() => api.SendAsync(HttpMethod.Delete, $"/api/compliance/{api.LastComplianceId}");

    [When("removo o registro de compliance de id {long}")]
    public Task RemovoRegistroPorId(long id) => api.SendAsync(HttpMethod.Delete, $"/api/compliance/{id}");

    [When("envio uma requisição {word} para o registro de compliance criado")]
    public Task EnvioParaRegistroCriado(string metodo) =>
        api.SendAsync(new HttpMethod(metodo), $"/api/compliance/{api.LastComplianceId}",
            metodo == "DELETE" ? null : Corpo(_companyId, _tipo, "NAO_CONFORME"));

    [When("listo os registros de compliance")]
    public Task ListoTodos() => api.SendAsync(HttpMethod.Get, "/api/compliance");

    [When("listo os registros de compliance da empresa {string}")]
    public Task ListoPorEmpresa(string empresa) =>
        api.SendAsync(HttpMethod.Get, $"/api/compliance/company/{api.CompanyIds[empresa]}");

    [When("listo os registros de compliance da empresa de id {long}")]
    public Task ListoPorEmpresaId(long companyId) =>
        api.SendAsync(HttpMethod.Get, $"/api/compliance/company/{companyId}");

    [Then("ao listar os registros de compliance da empresa {string} a resposta deve conter {int} item/itens")]
    public async Task AoListarDeveConter(string empresa, int quantidade)
    {
        await ListoPorEmpresa(empresa);
        Assert.Equal(HttpStatusCode.OK, api.Response!.StatusCode);
        Assert.Equal(quantidade, api.Json.GetProperty("items").GetArrayLength());
    }

    private Task Registrar(string tipo, string status, long companyId)
    {
        _companyId = companyId;
        _tipo = tipo;
        return api.SendAsync(HttpMethod.Post, "/api/compliance", Corpo(companyId, tipo, status));
    }

    private static object Corpo(long companyId, string tipo, string status) =>
        new { companyId, complianceType = tipo, status, reportDate = DataRelatorio };
}
