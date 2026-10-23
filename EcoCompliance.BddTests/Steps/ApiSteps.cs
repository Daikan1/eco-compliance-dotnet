using EcoCompliance.BddTests.Support;
using Reqnroll;

namespace EcoCompliance.BddTests.Steps;

/// <summary>Monitoramento, autenticação e requisições livres.</summary>
[Binding]
public sealed class ApiSteps(ApiContext api)
{
    [Given("que estou autenticado como administrador")]
    public Task EstouAutenticado() => api.AuthenticateAsync();

    [Given("que não estou autenticado")]
    public void NaoEstouAutenticado() => api.Token = null;

    [When("consulto a identificação da API")]
    public Task ConsultoIdentificacao() => api.SendAsync(HttpMethod.Get, "/");

    [When("consulto o health check")]
    public Task ConsultoHealthCheck() => api.SendAsync(HttpMethod.Get, "/health");

    [When("faço login com as credenciais do administrador")]
    public Task LoginAdministrador() =>
        api.SendAsync(HttpMethod.Post, "/api/auth/login",
            new { username = EcoApiFactory.AdminUser, password = EcoApiFactory.AdminPassword });

    [When("faço login com usuário {string} e senha {string}")]
    public Task Login(string usuario, string senha) =>
        api.SendAsync(HttpMethod.Post, "/api/auth/login", new { username = usuario, password = senha });

    [When("envio uma requisição {word} para {string} com o corpo:")]
    public Task EnvioRequisicao(string metodo, string caminho, string corpo) =>
        api.SendRawAsync(new HttpMethod(metodo), caminho, corpo);
}
