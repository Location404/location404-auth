using MediatR;
using Microsoft.Extensions.Logging;
using UserIdentity.Application.Common.Interfaces;
using UserIdentity.Application.Common.Results;
using UserIdentity.Application.Features.Authentication.Interfaces;
using UserIdentity.Domain.Entities;

namespace UserIdentity.Application.Features.Authentication.Commands.RegisterUser;

public class RegisterUserCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordService passwordService,
    ITokenService tokenService,
    ILogger<RegisterUserCommandHandler> logger) : IRequestHandler<RegisterUserCommand, Result<RegisterUserResult>>
{
    public async Task<Result<RegisterUserResult>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await unitOfWork.UserRepository.ExistsByUsernameOrEmailAsync(request.Username, request.EmailAddress, cancellationToken)
            ? Result<RegisterUserResult>.Failure(Error.Conflict("User with the same username or email already exists."))
            : await SignUpUserAsync(request, cancellationToken);
    }

    private async Task<Result<RegisterUserResult>> SignUpUserAsync(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        logger.LogInformation("registering user with username: {Username}", request.Username);

        try
        {
            (string hash, string salt) = passwordService.CreatePasswordHash(request.Password);
            var user = new UserApplication(request.Username, request.EmailAddress, hash, salt);

            await unitOfWork.UserRepository.AddAsync(user, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            (string token, string refreshToken) = tokenService.GenerateTokens(user);
            await transaction.CommitAsync(cancellationToken);

            return Result<RegisterUserResult>.Success(new RegisterUserResult(user.Id, request.EmailAddress, token, refreshToken));
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            logger.LogInformation("User registration process completed for username: {Username}", request.Username);
        }
    }
}