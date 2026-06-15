using Notes.Api.Contracts.Authentication;

namespace Notes.Api.Services;

public class AuthService : IAuthService
{
    public Task<AuthenticationResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
