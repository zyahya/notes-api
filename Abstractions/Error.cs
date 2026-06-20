namespace Notes.Api.Abstractions;

public record Error(
    string Code,
    string Description,
<<<<<<< HEAD
    int? Status
=======
    int? StatusCode
>>>>>>> add-error-handling
)
{
    public static readonly Error None = new(string.Empty, string.Empty, null);
}
