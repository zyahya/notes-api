using Microsoft.AspNetCore.Identity;

using Notes.Api.Contracts.Authentication;

namespace Notes.Api.Services;

// TODO: Return standard error objects
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<AuthenticationResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return null;
        }

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            return null;
        }

        // TODO: Generate and return jwt token

        // TODO: Return AuthenticationResponse
        return null;
    }
}
