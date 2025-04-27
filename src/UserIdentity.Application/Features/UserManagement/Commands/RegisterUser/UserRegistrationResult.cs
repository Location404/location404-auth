namespace UserIdentity.Application.Features.UserManagement.Commands.RegisterUser;

public record UserRegistrationResult(
    Guid UserId,
    string Username,
    string AccessToken,
    string RefreshToken);