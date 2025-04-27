namespace UserIdentity.Application.Features.UserManagement.Commands;

public record UserRegistrationResult(
    Guid UserId,
    string Username,
    string AccessToken,
    string RefreshToken);