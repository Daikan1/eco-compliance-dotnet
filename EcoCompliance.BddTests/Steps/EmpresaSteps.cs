using EcoCompliance.BddTests.Support;
using Reqnroll;

namespace EcoCompliance.BddTests.Steps;

[Binding]
public sealed class EmpresaSteps(ApiContext api)
{
    [Given("que existe a empresa {string} cadastrada")]
    public Task ExisteEmpresa(string nome) => api.CreateCompanyAsync(nome);

    [Given("que existem as empresas:")]
    public async Task ExistemEmpresas(Table empresas)
    {
        foreach (var linha in empresas.Rows)
            await api.CreateCompanyAsync(linha["nome"]);
    }

    [When("cadastro a empresa {string}")]
    public Task CadastroEmpresa(string nome) =>
        api.SendAsync(HttpMethod.Post, "/api/companies", new { name = nome });

    [When("cadastro a empresa {string} com CNPJ {string} do setor {string}")]
    public Task CadastroEmpresaCompleta(string nome, string cnpj, string setor) =>
        api.SendAsync(HttpMethod.Post, "/api/companies", new { name = nome, cnpj, sector = setor });

    [When("consulto a empresa {string}")]
    public Task ConsultoEmpresa(string nome) =>
        api.SendAsync(HttpMethod.Get, $"/api/companies/{api.CompanyIds[nome]}");

    [When("consulto a empresa de id {long}")]
    public Task ConsultoEmpresaPorId(long id) => api.SendAsync(HttpMethod.Get, $"/api/companies/{id}");

    [When("listo as empresas com page {int} e pageSize {int}")]
    public Task ListoEmpresas(int page, int pageSize) =>
        api.SendAsync(HttpMethod.Get, $"/api/companies?page={page}&pageSize={pageSize}");
}
