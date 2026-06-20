using Notes.Api.Abstractions;
using Notes.Api.Contracts.Authentication;

namespace Notes.Api.Services;

public interface IAuthService
{
    Task<Result<AuthenticationResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken);

    Task<Result<AuthenticationResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
}
