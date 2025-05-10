namespace UserIdentity.Application.Features.Authentication.Commands.LoginUser;

public record LoginUserCommandResult(
    Guid UserId,
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiration,
    DateTime RefreshTokenExpiration
);