namespace Notes.Api.Abstractions;

public record UserError(
    string Code,
    string Description,
    int? Status
)
{
    public static readonly Error InvalidCredentials = new("InvalidCredentials", "Invalid email or password.", StatusCodes.Status400BadRequest);

    public static readonly Error DuplicateEmail = new("DuplicateEmail", "Another user with this email already exists.", StatusCodes.Status400BadRequest);
}
