using System.Globalization;
using System.Text.Json;
using EcoCompliance.BddTests.Support;
using Reqnroll;
using Xunit;

namespace EcoCompliance.BddTests.Steps;

/// <summary>Validações genéricas da resposta HTTP: status code, corpo JSON e contrato.</summary>
[Binding]
public sealed class RespostaSteps(ApiContext api)
{
    [Then("o status da resposta deve ser {int}")]
    public void StatusDeveSer(int status) =>
        Assert.True((int)api.Response!.StatusCode == status,
            $"Status esperado {status}, recebido {(int)api.Response.StatusCode}.\nCorpo: {api.Body}");

    [Then("a resposta deve seguir o contrato {string}")]
    public void DeveSeguirContrato(string contrato) => ContractValidator.Validate(contrato, api.Json);

    [Then("o campo {string} da resposta deve ser {string}")]
    public void CampoDeveSerTexto(string caminho, string esperado) =>
        Assert.Equal(esperado, Campo(caminho).GetString());

    [Then("o campo {string} da resposta deve ser {double}")]
    public void CampoDeveSerNumero(string caminho, double esperado) =>
        Assert.Equal(esperado, Campo(caminho).GetDouble(), precision: 6);

    [Then("o campo {string} da resposta deve conter {string}")]
    public void CampoDeveConter(string caminho, string trecho) =>
        Assert.Contains(trecho, Campo(caminho).GetString());

    [Then("o campo {string} da resposta deve estar preenchido")]
    public void CampoDeveEstarPreenchido(string caminho) =>
        Assert.False(string.IsNullOrWhiteSpace(Campo(caminho).GetString()), $"Campo '{caminho}' vazio");

    [Then("a resposta deve conter {int} item/itens")]
    public void DeveConterItens(int quantidade) =>
        Assert.Equal(quantidade, Campo("items").GetArrayLength());

    [Then("o corpo da resposta deve ser {string}")]
    public void CorpoDeveSer(string esperado) => Assert.Equal(esperado, api.Body);

    [Then("o erro de validação deve apontar o campo {string}")]
    public void ErroDeValidacaoDeveApontar(string campo)
    {
        var errors = Campo("errors");
        Assert.True(errors.TryGetProperty(campo, out var mensagens) && mensagens.GetArrayLength() > 0,
            $"Nenhum erro de validação para '{campo}'. Erros: {errors}");
    }

    [Then("a resposta deve exigir autenticação do tipo {string}")]
    public void DeveExigirAutenticacao(string esquema) =>
        Assert.Contains(api.Response!.Headers.WwwAuthenticate, h => h.Scheme == esquema);

    [Then("a resposta deve indicar a localização do recurso criado")]
    public void DeveIndicarLocalizacao()
    {
        var id = Campo("id").GetInt64().ToString(CultureInfo.InvariantCulture);
        Assert.NotNull(api.Response!.Headers.Location);
        Assert.EndsWith($"/api/companies/{id}", api.Response.Headers.Location!.ToString());
    }

    /// <summary>Navega no JSON por caminho simples: "items[0].companyName".</summary>
    private JsonElement Campo(string caminho)
    {
        var atual = api.Json;
        foreach (var parte in caminho.Split('.'))
        {
            var nome = parte;
            int? indice = null;
            var colchete = parte.IndexOf('[');
            if (colchete >= 0)
            {
                nome = parte[..colchete];
                indice = int.Parse(parte[(colchete + 1)..^1], CultureInfo.InvariantCulture);
            }

            Assert.True(atual.TryGetProperty(nome, out atual), $"Campo '{nome}' ausente na resposta: {api.Body}");
            if (indice is not null) atual = atual[indice.Value];
        }
        return atual;
    }
}
