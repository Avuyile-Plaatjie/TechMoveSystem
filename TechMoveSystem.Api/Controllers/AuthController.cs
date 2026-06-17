using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TechMoveSystem.Api.Dtos;

namespace TechMoveSystem.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public AuthController(IConfiguration configuration) => _configuration = configuration;

    [HttpPost("token")]
    public IActionResult CreateToken(LoginRequest request)
    {
        var expectedClientId = _configuration["ApiClient:ClientId"] ?? "mvc-client";
        var expectedSecret = _configuration["ApiClient:ClientSecret"] ?? "mvc-secret";

        if (request.ClientId != expectedClientId || request.ClientSecret != expectedSecret)
            return Unauthorized(new { message = "Invalid API client credentials." });

        var expires = DateTime.UtcNow.AddHours(2);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: new[] { new Claim(ClaimTypes.Name, request.ClientId) },
            expires: expires,
            signingCredentials: credentials);

        return Ok(new TokenResponse(new JwtSecurityTokenHandler().WriteToken(token), expires));
    }
}
