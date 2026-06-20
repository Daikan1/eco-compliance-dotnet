using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EcoCompliance.API.ViewModels.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EcoCompliance.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration) => _configuration = configuration;

    /// <summary>Autentica o usuário e retorna um JWT Bearer token.</summary>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var adminUser = _configuration["Auth:Username"];
        var adminPass = _configuration["Auth:Password"];

        if (request.Username != adminUser || request.Password != adminPass)
            return Unauthorized(new { message = "Invalid credentials" });

        var token = GenerateToken(request.Username);
        return Ok(token);
    }

    private LoginResponse GenerateToken(string username)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Auth:JwtKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(8);

        var jwt = new JwtSecurityToken(
            issuer: _configuration["Auth:JwtIssuer"],
            audience: _configuration["Auth:JwtAudience"],
            claims: new[] { new Claim(ClaimTypes.Name, username) },
            expires: expires,
            signingCredentials: creds
        );

        return new LoginResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(jwt),
            ExpiresAt = expires
        };
    }
}
