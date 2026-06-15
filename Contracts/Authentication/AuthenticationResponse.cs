namespace Notes.Api.Contracts.Authentication;

public record AuthenticationResponse(
    string Id,
    string? Email,
    string FirstName,
    string LastName,
    string Token,
    int ExpiresIn
);
