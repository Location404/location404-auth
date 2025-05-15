namespace UserIdentity.Application.Features.Authentication.Commands.RegisterUser;

public record RegisterUserResult(
    Guid UserId,
    string Username,
    string AccessToken,
    string RefreshToken);