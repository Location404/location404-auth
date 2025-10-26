namespace Location404.Auth.Application.Features.Authentication.Commands.RefreshTokenCommand;

public record RefreshTokenCommandResponse(
    string AccessToken,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt,
    string TokenType = "Bearer")
{
    public int RefreshTokenExpiresInSeconds => (int)(RefreshTokenExpiresAt - DateTime.UtcNow).TotalSeconds;
}