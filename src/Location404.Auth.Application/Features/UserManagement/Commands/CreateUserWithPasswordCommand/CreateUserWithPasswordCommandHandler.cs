using LiteBus.Commands.Abstractions;

using Microsoft.Extensions.Logging;

using Location404.Auth.Application.Common.Interfaces;
using Location404.Auth.Application.Common.Result;
using Location404.Auth.Domain.Entities;
using Location404.Auth.Domain.ValueObjects;

namespace Location404.Auth.Application.Features.UserManagement.Commands.CreateUserWithPasswordCommand;

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
        _logger.LogInformation($"Creating user with email {message.Email} and username {message.Username}.");

        var existingUserByEmail = await _unitOfWork.Users.ExistsByEmailAsync(message.Email, cancellationToken);
        if (existingUserByEmail == true)
        {
            _logger.LogDebug($"User with email {message.Email} already exists.");
            return Result<CreateUserWithPasswordCommandResponse>.Failure(new Error(
                "EmailAlreadyInUse",
                "A user with this email already exists.",
                ErrorType.Validation));
        }

        var existingUserByUsername = await _unitOfWork.Users.ExistsByUsernameAsync(message.Username, cancellationToken);
        if (existingUserByUsername == true)
        {
            _logger.LogDebug($"User with username {message.Username} already exists.");
            return Result<CreateUserWithPasswordCommandResponse>.Failure(new Error(
                "UsernameAlreadyInUse",
                "A user with this username already exists.",
                ErrorType.Validation));
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
            _logger.LogInformation($"User with email {message.Email} and username {message.Username} created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while creating user with email {message.Email} and username {message.Username}.");

            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result<CreateUserWithPasswordCommandResponse>.Failure(new Error(
                "DatabaseError",
                $"An error occurred while saving the user: {ex.Message}",
                ErrorType.Database));
        }

        var response = new CreateUserWithPasswordCommandResponse(newUser.Id, newUser.Username, newUser.Email);
        return Result<CreateUserWithPasswordCommandResponse>.Success(response);
    }
}