using EcoCompliance.API.Controllers;
using EcoCompliance.API.ViewModels.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace EcoCompliance.Tests.Controllers;

public class AuthControllerTests
{
    private static AuthController BuildController(string user = "admin", string pass = "admin123")
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Auth:Username"]    = user,
                ["Auth:Password"]    = pass,
                ["Auth:JwtKey"]      = "TestSecretKey-MustBe32CharsLong!!",
                ["Auth:JwtIssuer"]   = "eco-compliance-api",
                ["Auth:JwtAudience"] = "eco-compliance-client"
            })
            .Build();

        return new AuthController(config);
    }

    [Fact]
    public void Login_ValidCredentials_Returns200WithToken()
    {
        var controller = BuildController();
        var request = new LoginRequest { Username = "admin", Password = "admin123" };

        var result = controller.Login(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
        var response = Assert.IsType<LoginResponse>(ok.Value);
        Assert.False(string.IsNullOrWhiteSpace(response.Token));
    }

    [Fact]
    public void Login_InvalidCredentials_Returns401()
    {
        var controller = BuildController();
        var request = new LoginRequest { Username = "wrong", Password = "wrong" };

        var result = controller.Login(request);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }
}
