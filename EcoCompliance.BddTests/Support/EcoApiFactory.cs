using EcoCompliance.API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace EcoCompliance.BddTests.Support;

/// <summary>
/// Sobe a API real em memória (pipeline HTTP completo: rotas, JWT, validação,
/// middleware de erros e serialização JSON), trocando apenas o Oracle por um
/// banco EF Core InMemory isolado por cenário.
/// </summary>
public sealed class EcoApiFactory : WebApplicationFactory<Program>
{
    public const string AdminUser     = "admin";
    public const string AdminPassword = "Senha-Testes-BDD-2026";
    private const string JwtKey       = "chave-jwt-exclusiva-dos-testes-bdd-com-mais-de-32-caracteres";

    private readonly string _databaseName = $"eco-bdd-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("Auth:Username", AdminUser);
        builder.UseSetting("Auth:Password", AdminPassword);
        builder.UseSetting("Auth:JwtKey", JwtKey);
        builder.UseSetting("Database:ApplyMigrations", "false");

        // Erros esperados (400/404) não poluem a saída dos testes
        builder.ConfigureLogging(logging => logging.ClearProviders());

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(_databaseName));
        });
    }
}
