namespace UserIdentityService.Application.Features.Authentication.Commands.AuthenticateUserWithPasswordCommand;

public record AuthenticateUserWithPasswordCommandResponse(
    Guid UserId,
    string Username,
    string Email,
    string? ProfileImage,
    string AccessToken,
    string RefreshToken);