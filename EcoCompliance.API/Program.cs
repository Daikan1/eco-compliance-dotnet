using System.Text;
using EcoCompliance.API.Data;
using EcoCompliance.API.Middleware;
using EcoCompliance.API.Repositories;
using EcoCompliance.API.Repositories.Interfaces;
using EcoCompliance.API.Services;
using EcoCompliance.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("Oracle")));

// ── Repositories ──────────────────────────────────────────────────────────────
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IComplianceRepository, ComplianceRepository>();
builder.Services.AddScoped<IEsgIndicatorRepository, EsgIndicatorRepository>();

// ── Services ──────────────────────────────────────────────────────────────────
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IComplianceService, ComplianceService>();
builder.Services.AddScoped<IEsgIndicatorService, EsgIndicatorService>();

// ── JWT Authentication ────────────────────────────────────────────────────────
var jwtKey = builder.Configuration["Auth:JwtKey"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = builder.Configuration["Auth:JwtIssuer"],
            ValidAudience            = builder.Configuration["Auth:JwtAudience"],
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// ── Controllers + Validation ──────────────────────────────────────────────────
builder.Services.AddControllers();

// ── Swagger ───────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "Eco Compliance API (.NET)",
        Version     = "v1",
        Description = "API ESG para governança e compliance ambiental — FIAP"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.Http,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "Informe o token JWT: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ── Health checks (usado pelo pipeline e pelo Azure App Service) ─────────────
builder.Services.AddHealthChecks();

// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

// ── Migrations automáticas (Database__ApplyMigrations=true) ─────────────────
if (app.Configuration.GetValue<bool>("Database:ApplyMigrations"))
{
    using var scope = app.Services.CreateScope();
    try
    {
        scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
        app.Logger.LogInformation("Migrations aplicadas com sucesso");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Falha ao aplicar migrations — a API sobe mesmo assim");
    }
}

app.UseMiddleware<GlobalExceptionMiddleware>();

// Swagger habilitado em todos os ambientes (staging e produção incluídos)
// para permitir a validação e as evidências do deploy.
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapHealthChecks("/health");

// Identifica ambiente e versão (commit) em execução — evidência de deploy
app.MapGet("/", (IWebHostEnvironment env, IConfiguration config) => Results.Ok(new
{
    application = "Eco Compliance API — Cidades ESG Inteligentes",
    environment = env.EnvironmentName,
    version     = config["APP_VERSION"] ?? "local",
    swagger     = "/swagger",
    health      = "/health"
}));

app.Run();

// Partial class needed for WebApplicationFactory in integration/unit tests
public partial class Program { }
