using LiteBus.Commands.Abstractions;
using LiteBus.Messaging.Abstractions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using UserIdentityService.Application.Common.Interfaces;
using UserIdentityService.Application.Common.Result;
using UserIdentityService.Domain.ValueObjects;

namespace UserIdentityService.Application.Features.UserManagement.Commands.UpdateUserInformationsCommand;

public class UpdateUserInformationsCommandHandler(
    IUnitOfWork uow,
    IEncryptPasswordService encryptPassword,
    ILogger<UpdateUserInformationsCommandHandler> logger)
    : ICommandHandler<UpdateUserInformationsCommand, Result<UpdateUserInformationsCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork = uow;
    private readonly IEncryptPasswordService _encryptPassword = encryptPassword;
    private readonly ILogger<UpdateUserInformationsCommandHandler> _logger = logger;


    async Task<Result<UpdateUserInformationsCommandResponse>> IAsyncMessageHandler<UpdateUserInformationsCommand, Result<UpdateUserInformationsCommandResponse>>.HandleAsync(
        UpdateUserInformationsCommand message, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetUserByIdAsync(message.Id, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", message.Id);
            return Result<UpdateUserInformationsCommandResponse>.Failure(new Error(
                "EmailAlreadyInUse",
                "A user with this email already exists.",
                ErrorType.Validation));
        }

        if (user.Email != message.Email && message.Email != null)
        {
            var existingUserByEmail = await _unitOfWork.Users.ExistsByEmailAsync(message.Email, cancellationToken);
            if (existingUserByEmail == true)
            {
                _logger.LogDebug($"User with email {message.Email} already exists.");
                return Result<UpdateUserInformationsCommandResponse>.Failure(new Error(
                    "EmailAlreadyInUse",
                    "A user with this email already exists.",
                    ErrorType.Validation));
            }
        }

        if (message.Password != null && !_encryptPassword.Verify(message.Password, user.Password!))
        {
            _logger.LogDebug("The new password cannot be the same as the old one.");
            return Result<UpdateUserInformationsCommandResponse>.Failure(new Error(
                "SamePassword",
                "The new password cannot be the same as the old one.",
                ErrorType.Validation));
        }

        if (user.Username != message.Username && message.Username != null)
        {
            var existingUserByUsername = await _unitOfWork.Users.ExistsByUsernameAsync(message.Username, cancellationToken);
            if (existingUserByUsername == true)
            {
                _logger.LogDebug($"User with username {message.Username} already exists.");
                return Result<UpdateUserInformationsCommandResponse>.Failure(new Error(
                    "UsernameAlreadyInUse",
                    "A user with this username already exists.",
                    ErrorType.Validation));
            }
        }

        user.UpdateProfile(
            message.Username ?? null,
            message.Email != null ? EmailAddress.Create(message.Email) : null,
            message.Password != null ? _encryptPassword.Encrypt(message.Password) : null);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<UpdateUserInformationsCommandResponse>.Success(new UpdateUserInformationsCommandResponse(
            user.Id,
            user.Email,
            user.Username));
    }

    public async Task<byte[]> GetBytesFromFormFileAsync(IFormFile formFile)
    {
        using var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}