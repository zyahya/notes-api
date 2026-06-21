using System.ComponentModel.DataAnnotations;
using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace Notes.Api.Services;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    [MinLength(40)]
    public string Secret { get; set; } = string.Empty;

    public SecurityKey SecretSecurityKeyBytes => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret));

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Range(10, int.MaxValue)]
    public int ExpiryMinutes { get; set; } = 30;
}
