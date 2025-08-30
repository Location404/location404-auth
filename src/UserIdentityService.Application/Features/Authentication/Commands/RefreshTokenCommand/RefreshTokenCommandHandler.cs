using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;

using UserIdentityService.Application.Common.Interfaces;
using UserIdentityService.Application.Common.Result;
using UserIdentityService.Application.Features.Authentication.Interfaces;

namespace UserIdentityService.Application.Features.Authentication.Commands.RefreshTokenCommand;

public class RefreshTokenCommandHandler(
    ITokenService tokenService,
    ILogger<RefreshTokenCommandHandler> logger,
    IUnitOfWork unitOfWork
) : ICommandHandler<RefreshTokenCommand, Result<RefreshTokenCommandResponse>>
{
    private readonly ITokenService _tokenService = tokenService;
    private readonly ILogger<RefreshTokenCommandHandler> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<RefreshTokenCommandResponse>> HandleAsync(RefreshTokenCommand message,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(message.RefreshToken))
        {
            _logger.LogWarning("RefreshToken is null or empty.");
            return Result<RefreshTokenCommandResponse>.Failure(new Error(
                "RefreshTokenIsMissing",
                "RefreshToken must be provided.",
                ErrorType.Validation));
        }

        var resultRotate = await _tokenService.RotateAsync(message.UserId, message.RefreshToken, cancellationToken);
        if (resultRotate.IsFailure)
        {
            _logger.LogWarning("Failed to rotate token for user {UserId}: {Error}", message.UserId, resultRotate.Error);
            return Result<RefreshTokenCommandResponse>.Failure(resultRotate.Error);
        }
        
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error committing transaction for user {UserId}", message.UserId);
            return Result<RefreshTokenCommandResponse>.Failure(new Error(
                "RefreshTokenFailed",
                "An error occurred while committing the transaction.",
                ErrorType.Database));
        }

        return Result<RefreshTokenCommandResponse>.Success(new(
            AccessToken: resultRotate.Value.accessToken,
            RefreshToken: resultRotate.Value.newRefresh.Token,
            RefreshTokenExpiresAt: resultRotate.Value.newRefresh.ExpiresAtUtc
        ));
    }
}
