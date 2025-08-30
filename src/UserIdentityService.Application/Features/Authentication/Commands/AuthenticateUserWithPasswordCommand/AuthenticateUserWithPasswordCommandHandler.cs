using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;

using UserIdentityService.Application.Common.Interfaces;
using UserIdentityService.Application.Common.Result;
using UserIdentityService.Application.Features.Authentication.Interfaces;

namespace UserIdentityService.Application.Features.Authentication.Commands.AuthenticateUserWithPasswordCommand;

public class AuthenticateUserWithPasswordCommandHandler(
    IUnitOfWork uow,
    IEncryptPasswordService encryptPassword,
    ITokenService tokenService,
    ILogger<AuthenticateUserWithPasswordCommandHandler> logger
) : ICommandHandler<AuthenticateUserWithPasswordCommand, Result<AuthenticateUserWithPasswordCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork = uow;
    private readonly IEncryptPasswordService _encryptPassword = encryptPassword;
    private readonly ILogger<AuthenticateUserWithPasswordCommandHandler> _logger = logger;
    private readonly ITokenService _tokenService = tokenService;

    public async Task<Result<AuthenticateUserWithPasswordCommandResponse>> HandleAsync(AuthenticateUserWithPasswordCommand message,
        CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetUserByEmailAsync(message.Email, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User with email {Email} not found.", message.Email);
            return Result<AuthenticateUserWithPasswordCommandResponse>.Failure(new Error(
                "UserEmailNotFound",
                "User with the provided email does not exist.",
                ErrorType.NotFound));
        }

        if (user.Password is null)
        {
            _logger.LogWarning("User with email {Email} has no password set.", message.Email);
            return Result<AuthenticateUserWithPasswordCommandResponse>.Failure(new Error(
                "PasswordNotSet",
                "User does not have a password set.",
                ErrorType.Validation));
        }

        var isPasswordValid = _encryptPassword.Verify(message.Password, user.Password);
        if (!isPasswordValid)
        {
            _logger.LogWarning("Invalid password for user {Email}.", message.Email);
            return Result<AuthenticateUserWithPasswordCommandResponse>.Failure(new Error(
                "InvalidCredentials",
                "Invalid email or password.",
                ErrorType.Validation));
        }

        var token = _tokenService.GenerateAccessToken(user.Id, user.Username, ["User"]);
        var refreshTokenResult = _tokenService.IssueRefreshTokenAsync(user.Id, cancellationToken);
        if (!refreshTokenResult.IsSuccess)
        {
            _logger.LogError("Failed to issue refresh token for user {Email}.", message.Email);
            return Result<AuthenticateUserWithPasswordCommandResponse>.Failure(refreshTokenResult.Error);
        }

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("User {Email} authenticated successfully.", message.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while authenticating user {Email}.", message.Email);
            return Result<AuthenticateUserWithPasswordCommandResponse>.Failure(new Error(
                "AuthenticationFailed",
                "An error occurred while authenticating the user.",
                ErrorType.Database));
        }

        return Result<AuthenticateUserWithPasswordCommandResponse>.Success(new(
            AccessToken: token.Value,
            RefreshToken: refreshTokenResult.Value.Token,
            RefreshTokenExpiresAt: refreshTokenResult.Value.ExpiresAtUtc
        ));
    }
}
