using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace Notes.Api.Services;

public class JwtOptions
{
    public const string Section = "Jwt";

    public string Secret { get; set; } = string.Empty;
    public SecurityKey SecretSecurityKeyBytes => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret));
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 30;
}
