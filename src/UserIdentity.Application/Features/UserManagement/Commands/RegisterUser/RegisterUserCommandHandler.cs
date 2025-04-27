using MediatR;
using Microsoft.Extensions.Logging;
using UserIdentity.Application.Common.Interfaces;
using UserIdentity.Application.Common.Results;
using UserIdentity.Application.Features.Authentication.Interfaces;
using UserIdentity.Domain.Common.Results;
using UserIdentity.Domain.Entities;

namespace UserIdentity.Application.Features.UserManagement.Commands.RegisterUser;

public class RegisterUserCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordService passwordService,
    ITokenService tokenService,
    ILogger<RegisterUserCommandHandler> logger) : IRequestHandler<RegisterUserCommand, Result<UserRegistrationResult>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPasswordService _passwordService = passwordService;
    private readonly ITokenService _tokenService = tokenService;
    private readonly ILogger<RegisterUserCommandHandler> _logger = logger;

    public async Task<Result<UserRegistrationResult>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.UserRepository.ExistsByUsernameOrEmailAsync(request.Username, request.EmailAddress)
            ? Result<UserRegistrationResult>.Failure(Error.Conflict("User with the same username or email already exists."))
            : await RegisterUserAsync(request);
    }

    private async Task<Result<UserRegistrationResult>> RegisterUserAsync(RegisterUserCommand request)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        _logger.LogInformation("Registering user with username: {Username}", request.Username);

        try
        {
            (string hash, string salt) = _passwordService.CreatePasswordHash(request.Password);
            var user = new UserApplication(request.Username, request.EmailAddress, hash, salt);

            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            (string token, string refreshToken) = _tokenService.GenerateTokens(user);
            transaction.Commit();

            return Result<UserRegistrationResult>.Success(new UserRegistrationResult(user.Id, request.EmailAddress, token, refreshToken));
        }
        catch
        {
            transaction.Rollback();
            return Result<UserRegistrationResult>.Failure(Error.InternalServerError("An error occurred while processing your request."));
            throw;
        }
        finally
        {
            _logger.LogInformation("User registration process completed for username: {Username}", request.Username);
            await transaction.DisposeAsync();
        }
    }
}
