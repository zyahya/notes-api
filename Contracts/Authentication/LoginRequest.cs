namespace Notes.Api.Contracts.Authentication;

public record LoginRequest(
    string Email = "test@test.test",
    string Password = "Test@test.test0"
);
