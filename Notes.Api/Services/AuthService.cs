using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Notes.Api.Abstractions;
using Notes.Api.Contracts.Authentication;

namespace Notes.Api.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtProvider _jwtProvider;

    public AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider)
    {
        _userManager = userManager;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<AuthenticationResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return Result.Failure<AuthenticationResponse>(UserError.InvalidCredentials);
        }

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            return Result.Failure<AuthenticationResponse>(UserError.InvalidCredentials);
        }

        var (token, expiresIn) = _jwtProvider.GenerateToken(user);

        return Result.Success(new AuthenticationResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            token,
            expiresIn
        ));
    }

    public async Task<Result<AuthenticationResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var isEmailRegistered = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken: cancellationToken);

        if (isEmailRegistered)
        {
            return Result.Failure<AuthenticationResponse>(UserError.DuplicateEmail);
        }

        var user = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure<AuthenticationResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        var (token, expiresIn) = _jwtProvider.GenerateToken(user);

        return Result.Success(new AuthenticationResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            token,
            expiresIn
        ));
    }
}
