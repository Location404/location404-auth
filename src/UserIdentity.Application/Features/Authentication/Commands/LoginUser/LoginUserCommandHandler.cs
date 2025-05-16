using MediatR;
using UserIdentity.Application.Common.Interfaces;
using UserIdentity.Application.Common.Results;
using UserIdentity.Application.Features.Authentication.Interfaces;
using UserIdentity.Domain.Common.Results;

namespace UserIdentity.Application.Features.Authentication.Commands.LoginUser;

public class LoginUserCommandHandler(
    ITokenService tokenService,
    IPasswordService passwordService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<LoginUserCommand, Result<LoginUserCommandResult>>
{
    public async Task<Result<LoginUserCommandResult>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var userExist = await unitOfWork.UserRepository.ExistsByUsernameOrEmailAsync(
            email: request.Email,
            cancellationToken: cancellationToken);

        if (!userExist)
        {
            return Result<LoginUserCommandResult>.Failure(Error.NotFound("User with this email or username not found"));
        }

        var user = await unitOfWork.UserRepository.GetByUsernameOrEmailAsync(
            email: request.Email,
            cancellationToken: cancellationToken);

        var isAuthenticated = passwordService.VerifyPasswordHash(
            password: request.Password,
            storedHash: user!.PasswordHash,
            storedSalt: user.PasswordSalt);

        if (!isAuthenticated)
        {
            return Result<LoginUserCommandResult>.Failure(Error.Unauthorized("Invalid credentials"));
        }

        var (token, refreshToken) = tokenService.GenerateTokens(user);

        var signInUserResult = new LoginUserCommandResult(
            UserId: user.Id,
            AccessToken: token,
            RefreshToken: refreshToken,
            AccessTokenExpiration: tokenService.GetAccessTokenExpirationTime(),
            RefreshTokenExpiration: tokenService.GetRefreshTokenExpirationTime());

        return Result<LoginUserCommandResult>.Success(signInUserResult);
    }
}