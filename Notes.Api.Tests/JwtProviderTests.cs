using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Notes.Api.Entities;
using Notes.Api.Services;

namespace Notes.Api.Tests;

public class JwtProviderTests
{
    private const string TestSecret = "this_is_a_very_long_secret_key_that_exceeds_forty_characters_requirement_for_testing_purposes";
    private const string TestIssuer = "test-issuer";
    private const string TestAudience = "test-audience";
    private const int TestExpiryMinutes = 30;

    private static IOptions<JwtOptions> CreateJwtOptions(
        string secret = TestSecret,
        string issuer = TestIssuer,
        string audience = TestAudience,
        int expiryMinutes = TestExpiryMinutes)
    {
        return Options.Create(new JwtOptions
        {
            Secret = secret,
            Issuer = issuer,
            Audience = audience,
            ExpiryMinutes = expiryMinutes
        });
    }

    private static ApplicationUser CreateTestUser()
    {
        return new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "testuser@example.com",
            FirstName = "John",
            LastName = "Doe"
        };
    }

    [Fact]
    public void GenerateToken_ShouldReturnCorrectExpiresIn()
    {
        // Arrange
        IOptions<JwtOptions> options = CreateJwtOptions();
        JwtProvider sut = new JwtProvider(options);
        ApplicationUser user = CreateTestUser();

        // Act
        (string _, int expiresIn) = sut.GenerateToken(user);

        // Assert
        int expectedExpiresIn = options.Value.ExpiryMinutes * 60;
        Assert.Equal(expectedExpiresIn, expiresIn);
    }

    [Fact]
    public void GenerateToken_ShouldGenerateValidJwtTokenString()
    {
        // Arrange
        IOptions<JwtOptions> options = CreateJwtOptions();
        JwtProvider sut = new JwtProvider(options);
        ApplicationUser user = CreateTestUser();

        // Act
        (string token, int _) = sut.GenerateToken(user);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);

        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        Assert.True(handler.CanReadToken(token));
    }

    [Fact]
    public void GenerateToken_ShouldContainAllExpectedClaims()
    {
        // Arrange
        IOptions<JwtOptions> options = CreateJwtOptions();
        JwtProvider sut = new JwtProvider(options);
        ApplicationUser user = CreateTestUser();

        // Act
        (string tokenString, int _) = sut.GenerateToken(user);

        // Assert
        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken = handler.ReadJwtToken(tokenString);

        // JTI should be a valid GUID
        string? jtiClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
        Assert.NotNull(jtiClaim);
        Assert.True(Guid.TryParse(jtiClaim, out _));

        // Issuer should match
        string? issClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Iss)?.Value;
        Assert.Equal(options.Value.Issuer, issClaim);

        // Audience should match
        string? audClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Aud)?.Value;
        Assert.Equal(options.Value.Audience, audClaim);

        // Subject (User ID) should match
        string? subClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        Assert.Equal(user.Id, subClaim);

        // Email should match
        string? emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;
        Assert.Equal(user.Email, emailClaim);

        // GivenName should match
        string? givenNameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.GivenName)?.Value;
        Assert.Equal(user.FirstName, givenNameClaim);

        // FamilyName should match
        string? familyNameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.FamilyName)?.Value;
        Assert.Equal(user.LastName, familyNameClaim);
    }

    [Fact]
    public void GenerateToken_ShouldHaveCorrectExpirationTime()
    {
        // Arrange
        int expiryMinutes = 15;
        IOptions<JwtOptions> options = CreateJwtOptions(expiryMinutes: expiryMinutes);
        JwtProvider sut = new JwtProvider(options);
        ApplicationUser user = CreateTestUser();

        DateTime beforeGeneration = DateTime.UtcNow;

        // Act
        (string tokenString, int _) = sut.GenerateToken(user);

        DateTime afterGeneration = DateTime.UtcNow;

        // Assert
        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken = handler.ReadJwtToken(tokenString);

        DateTime expectedExpiryMin = beforeGeneration.AddMinutes(expiryMinutes);
        DateTime expectedExpiryMax = afterGeneration.AddMinutes(expiryMinutes);

        // ValidTo is in UTC
        Assert.True(jwtToken.ValidTo >= expectedExpiryMin.AddSeconds(-5), "Token expiration is too early.");
        Assert.True(jwtToken.ValidTo <= expectedExpiryMax.AddSeconds(5), "Token expiration is too late.");
    }

    [Fact]
    public void GenerateToken_ShouldBeSignedWithCorrectKeyAndValidatable()
    {
        // Arrange
        IOptions<JwtOptions> options = CreateJwtOptions();
        JwtProvider sut = new JwtProvider(options);
        ApplicationUser user = CreateTestUser();

        // Act
        (string tokenString, int _) = sut.GenerateToken(user);

        // Assert
        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        TokenValidationParameters validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(options.Value.Secret)),
            ValidateIssuer = true,
            ValidIssuer = options.Value.Issuer,
            ValidateAudience = true,
            ValidAudience = options.Value.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(5) // Allow small clock skew
        };

        // If validation fails, it throws. If it passes, the signature is verified.
        ClaimsPrincipal principal = handler.ValidateToken(tokenString, validationParameters, out SecurityToken validatedToken);

        Assert.NotNull(validatedToken);
        Assert.NotNull(principal);
        Assert.True(principal.Identity?.IsAuthenticated);

        // Let's also check that the subject claim in the principal matches the user id
        string? subClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        Assert.Equal(user.Id, subClaim);
    }
}
