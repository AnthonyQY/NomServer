using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using NomServer.Application.Interfaces;

namespace NomServer.Application.Services;

public class TokenService(AppSettings appSettings) : ITokenService
{
    public string GenerateToken(string name, List<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, name)
        };

        var payload = new JwtPayload(
            issuer: appSettings.Jwt.Issuer,
            audience: appSettings.Jwt.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(1)
        );
        
        foreach (var role in roles)
        {
            payload.AddClaim(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Jwt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(new JwtHeader(creds), payload);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}