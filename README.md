# Eco Compliance API (.NET)

API REST para **governança e compliance ambiental (ESG)** desenvolvida em ASP.NET Core 8, com banco de dados Oracle, autenticação JWT e documentação via Swagger.

Projeto acadêmico — FIAP 2026.

---

## O que faz

A API permite que empresas registrem e acompanhem seus indicadores de sustentabilidade (ESG — Environmental, Social & Governance) e auditem o cumprimento de obrigações de compliance ambiental.

**Entidades principais:**

| Entidade | Descrição |
|---|---|
| `Company` | Empresa cadastrada (nome, CNPJ, setor) |
| `ComplianceRecord` | Registro de compliance vinculado a uma empresa |
| `EsgIndicator` | Indicador ESG registrado (emissões, resíduos, etc.) |

---

## Tecnologias

- [.NET 8](https://dotnet.microsoft.com/) — ASP.NET Core Web API
- [Oracle Database](https://www.oracle.com/database/) + [Oracle.EntityFrameworkCore 8.23.60](https://www.nuget.org/packages/Oracle.EntityFrameworkCore)
- [Entity Framework Core 8](https://docs.microsoft.com/ef/core/) — ORM + Migrations
- [JWT Bearer Authentication](https://jwt.io/) — Microsoft.AspNetCore.Authentication.JwtBearer
- [Swagger / Swashbuckle 6.9](https://swagger.io/) — Documentação interativa
- [Docker](https://www.docker.com/) — Containerização
- xUnit — Testes unitários

---

## Endpoints

| Método | Rota | Auth | Descrição |
|---|---|---|---|
| `POST` | `/api/auth/login` | — | Autentica e retorna JWT |
| `GET` | `/api/companies` | — | Lista empresas (paginado) |
| `GET` | `/api/companies/{id}` | — | Retorna empresa por ID |
| `POST` | `/api/companies` | JWT | Cadastra empresa |
| `GET` | `/api/compliance` | — | Lista registros de compliance |
| `GET` | `/api/compliance/company/{id}` | — | Compliance por empresa |
| `POST` | `/api/compliance` | JWT | Cria registro de compliance |
| `PUT` | `/api/compliance/{id}` | JWT | Atualiza registro |
| `DELETE` | `/api/compliance/{id}` | JWT | Remove registro |
| `GET` | `/api/esg-indicators` | — | Lista indicadores ESG |
| `POST` | `/api/esg-indicators` | JWT | Registra indicador ESG |

---

## Como rodar localmente

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- Acesso a um banco Oracle (ex: Oracle FIAP, Oracle XE, Oracle Cloud)

### 1. Clone o repositório

```bash
git clone https://github.com/SEU_USUARIO/eco-compliance-dotnet.git
cd eco-compliance-dotnet
```

### 2. Configure as credenciais

Crie o arquivo `EcoCompliance.API/appsettings.Development.json` (ele está no `.gitignore` e nunca é commitado):

```json
{
  "ConnectionStrings": {
    "Oracle": "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=oracle.fiap.com.br)(PORT=1521))(CONNECT_DATA=(SID=ORCL)));User Id=SEU_USUARIO_ORACLE;Password=SUA_SENHA_ORACLE;"
  }
}
```

> O `appsettings.json` contém placeholders. Preencha **apenas** no arquivo `Development.json` local.

Edite também `appsettings.json` para definir a senha do admin e a chave JWT:

```json
{
  "Auth": {
    "Username": "admin",
    "Password": "SUA_SENHA_ADMIN_AQUI",
    "JwtKey": "SUA_CHAVE_JWT_SECRETA_MINIMO_32_CHARS_AQUI"
  }
}
```

### 3. Execute as migrations

```bash
cd EcoCompliance.API
dotnet ef database update
```

> Alternativamente, rode o script `migration_manual.sql` diretamente no banco.

### 4. Inicie a API

```bash
dotnet run --project EcoCompliance.API
```

Acesse o Swagger em: `http://localhost:5000/swagger`

---

## Como rodar com Docker

```bash
docker build -t eco-compliance-api .

docker run -p 8080:8080 \
  -e "ConnectionStrings__Oracle=Data Source=...;User Id=SEU_USUARIO;Password=SUA_SENHA;" \
  -e "Auth__Password=SUA_SENHA_ADMIN" \
  -e "Auth__JwtKey=SUA_CHAVE_JWT_SECRETA_MINIMO_32_CHARS" \
  eco-compliance-api
```

---

## Como autenticar no Swagger

1. Acesse `/swagger`
2. Chame `POST /api/auth/login` com:
   ```json
   { "username": "admin", "password": "SUA_SENHA_ADMIN_AQUI" }
   ```
3. Copie o token retornado
4. Clique em **Authorize** (cadeado) e cole: `Bearer <token>`

---

## Testes

```bash
dotnet test
```

---

## Coleção Postman

Importe o arquivo `eco-compliance.postman_collection.json` no Postman. O token JWT é salvo automaticamente após o login e usado nos demais requests.

---

## Estrutura do projeto

```
eco-compliance-dotnet/
├── EcoCompliance.API/
│   ├── Controllers/        # AuthController, CompanyController, ComplianceController, EsgIndicatorController
│   ├── Data/               # AppDbContext, Migrations
│   ├── Middleware/         # GlobalExceptionMiddleware
│   ├── Models/             # Company, ComplianceRecord, EsgIndicator
│   ├── Repositories/       # Interfaces + implementações
│   ├── Services/           # Regras de negócio
│   ├── ViewModels/         # Request/Response DTOs
│   ├── appsettings.json    # Configuração (sem credenciais reais)
│   └── Program.cs
├── EcoCompliance.Tests/    # Testes xUnit
├── Dockerfile
├── migration_manual.sql
└── eco-compliance.postman_collection.json
```

---

## Autor

**Luiz Alberto Silva de Santana** — RM565199  
FIAP — 2026
