namespace Notes.Api.Services;

public interface IJwtProvider
{
    // TODO: Try to implement this method using record instead of tuple
    (string Token, int ExpiresIn) GenerateToken(ApplicationUser user);
}
