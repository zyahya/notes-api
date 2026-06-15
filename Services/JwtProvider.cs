using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace Notes.Api.Services;

public class JwtProvider : IJwtProvider
{
    public (string Token, int ExpiresIn) GenerateToken(ApplicationUser user)
    {
        Claim[] claims = [
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iss, "NotesApp"),
            new(JwtRegisteredClaimNames.Aud, "NotesApp Users"),
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new(JwtRegisteredClaimNames.FamilyName, user.LastName),
        ];

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("qTSCTWh8wbk6B1xCrzOQ9LYcUJZi25kAoO+AmEXbCkA="));

        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var expiresIn = 30;

        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials,
            expires: DateTime.UtcNow.AddMinutes(expiresIn)
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresIn);
    }
}
