namespace UserIdentityService.Application.Features.Authentication.Commands.AuthenticateUserWithPasswordCommand;

public record AuthenticateUserWithPasswordCommandResponse(
    string AccessToken,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt,
    string TokenType = "Bearer")
{
    public int RefreshTokenExpiresInSeconds => (int)(RefreshTokenExpiresAt - DateTime.UtcNow).TotalSeconds;
}