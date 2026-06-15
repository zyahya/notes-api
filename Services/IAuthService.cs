using Notes.Api.Contracts.Authentication;

namespace Notes.Api.Services;

public interface IAuthService
{
    Task<AuthenticationResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken);
}
