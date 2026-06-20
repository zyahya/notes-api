namespace Notes.Api.Abstractions;

public record Error(
    string Code,
    string Description,
    int? Status
)
{
    public static readonly Error None = new(string.Empty, string.Empty, null);
}
