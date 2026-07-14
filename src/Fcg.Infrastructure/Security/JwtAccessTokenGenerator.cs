using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Fcg.Application.Security;
using Fcg.Domain.Users;
using Microsoft.IdentityModel.Tokens;

namespace Fcg.Infrastructure.Security;

public sealed class JwtAccessTokenGenerator : IAccessTokenGenerator
{
    private readonly JwtTokenSettings _settings;

    public JwtAccessTokenGenerator(JwtTokenSettings settings)
    {
        _settings = settings;
    }

    public AccessToken Generate(User user)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_settings.ExpirationInMinutes);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        return new AccessToken(token, expiresAtUtc);
    }
}
