using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;

using UserIdentityService.Application.Common.Interfaces;
using UserIdentityService.Application.Common.Result;
using UserIdentityService.Domain.Entities;
using UserIdentityService.Domain.ValueObjects;

namespace UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithPasswordCommand;

public class CreateUserWithPasswordCommandHandler(
    IUnitOfWork uow,
    IEncryptPasswordService encryptPassword,
    ILogger<CreateUserWithPasswordCommandHandler> logger) 
    : ICommandHandler<CreateUserWithPasswordCommand, Result<CreateUserWithPasswordCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork = uow;
    private readonly IEncryptPasswordService _encryptPassword = encryptPassword;
    private readonly ILogger<CreateUserWithPasswordCommandHandler> _logger = logger;

    public async Task<Result<CreateUserWithPasswordCommandResponse>> HandleAsync(CreateUserWithPasswordCommand message,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating user with email {Email} and username {Username}.", message.Email, message.Username);

        var existingUserByEmail = await _unitOfWork.Users.ExistsByEmailAsync(message.Email, cancellationToken);
        if (existingUserByEmail == true)
        {
            _logger.LogDebug("User with email {Email} already exists.", message.Email);
            return Result<CreateUserWithPasswordCommandResponse>.Failure(new Error("EmailAlreadyInUse", "A user with this email already exists."));
        }

        var existingUserByUsername = await _unitOfWork.Users.ExistsByUsernameAsync(message.Username, cancellationToken);
        if (existingUserByUsername == true)
        {
            _logger.LogDebug("User with username {Username} already exists.", message.Username);
            return Result<CreateUserWithPasswordCommandResponse>.Failure(new Error("UsernameAlreadyInUse", "A user with this username already exists."));
        }

        var newUser = User.Create(
            EmailAddress.Create(message.Email),
            username: message.Username,
            password: _encryptPassword.Encrypt(message.Password)
        );

        await _unitOfWork.Users.AddUserAsync(newUser, cancellationToken);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("User with email {Email} and username {Username} created successfully.", message.Email, message.Username);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating user with email {Email} and username {Username}.", message.Email, message.Username);
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result<CreateUserWithPasswordCommandResponse>.Failure(new Error("DatabaseError", $"An error occurred while saving the user: {ex.Message}"));
        }

        var response = new CreateUserWithPasswordCommandResponse(newUser.Id, newUser.Username, newUser.Email);
        return Result<CreateUserWithPasswordCommandResponse>.Success(response);
    }
}