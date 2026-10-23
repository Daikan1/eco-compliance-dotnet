using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace EcoCompliance.BddTests.Support;

/// <summary>
/// Estado compartilhado pelos steps de um cenário. O Reqnroll cria uma
/// instância por cenário e a descarta no final, então cada cenário tem a
/// própria API e o próprio banco.
/// </summary>
public sealed class ApiContext : IDisposable
{
    private readonly EcoApiFactory _factory = new();

    public ApiContext() => Client = _factory.CreateClient();

    public HttpClient Client { get; }
    public HttpResponseMessage? Response { get; private set; }
    public string Body { get; private set; } = string.Empty;
    public string? Token { get; set; }

    /// <summary>Ids criados no cenário, pelo nome usado no Gherkin.</summary>
    public Dictionary<string, long> CompanyIds { get; } = new();
    public long? LastComplianceId { get; set; }

    public JsonElement Json => JsonDocument.Parse(Body).RootElement;

    public Task SendAsync(HttpMethod method, string path, object? payload = null) =>
        SendRawAsync(method, path, payload is null ? null : JsonSerializer.Serialize(payload));

    public async Task SendRawAsync(HttpMethod method, string path, string? jsonBody)
    {
        using var request = new HttpRequestMessage(method, path);
        if (jsonBody is not null)
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        if (Token is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);

        Response = await Client.SendAsync(request);
        Body = await Response.Content.ReadAsStringAsync();
    }

    /// <summary>Login como administrador e guarda o token para as próximas requisições.</summary>
    public async Task AuthenticateAsync()
    {
        var response = await Client.PostAsJsonAsync("/api/auth/login",
            new { username = EcoApiFactory.AdminUser, password = EcoApiFactory.AdminPassword });
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Token = json.GetProperty("token").GetString();
    }

    /// <summary>Cria uma empresa via API (pré-condição de cenários) e retorna o id.</summary>
    public async Task<long> CreateCompanyAsync(string name, string? cnpj = null, string? sector = null)
    {
        var previousToken = Token;
        if (Token is null) await AuthenticateAsync();

        var response = await Client.SendAsync(new HttpRequestMessage(HttpMethod.Post, "/api/companies")
        {
            Content = JsonContent.Create(new { name, cnpj, sector }),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", Token) }
        });
        response.EnsureSuccessStatusCode();
        var id = (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetInt64();

        Token = previousToken;
        CompanyIds[name] = id;
        return id;
    }

    public void Dispose()
    {
        Response?.Dispose();
        Client.Dispose();
        _factory.Dispose();
    }
}
