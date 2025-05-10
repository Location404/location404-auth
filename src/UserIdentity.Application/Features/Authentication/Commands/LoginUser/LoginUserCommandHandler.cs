using MediatR;

using UserIdentity.Application.Common.Interfaces;
using UserIdentity.Application.Common.Results;
using UserIdentity.Application.Features.Authentication.Interfaces;
using UserIdentity.Domain.Common.Results;

namespace UserIdentity.Application.Features.Authentication.Commands.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<LoginUserCommandResult>>
{
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public LoginUserCommandHandler(ITokenService tokenService, IPasswordService passwordService, IUnitOfWork unitOfWork)
    {
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
    }

    public async Task<Result<LoginUserCommandResult>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var userExist = await _unitOfWork.UserRepository.ExistsByUsernameOrEmailAsync(
            email: request.Email,
            cancellationToken: cancellationToken);

        if (!userExist)
        {
            return Result<LoginUserCommandResult>.Failure(Error.NotFound("User with this email or username not found"));
        }

        var user = await _unitOfWork.UserRepository.GetByUsernameOrEmailAsync(
            email: request.Email,
            cancellationToken: cancellationToken);

        var isAuthenticated = _passwordService.VerifyPasswordHash(
            password: request.Password,
            storedHash: user!.PasswordHash,
            storedSalt: user.PasswordSalt);

        if (!isAuthenticated)
        {
            return Result<LoginUserCommandResult>.Failure(Error.Unauthorized("Invalid credentials"));
        }

        var (token, refreshToken) = _tokenService.GenerateTokens(user);

        var loginUserResult = new LoginUserCommandResult(
            UserId: user.Id,
            AccessToken: token,
            RefreshToken: refreshToken,
            AccessTokenExpiration: _tokenService.GetAccessTokenExpirationTime(),
            RefreshTokenExpiration: _tokenService.GetRefreshTokenExpirationTime());

        return Result<LoginUserCommandResult>.Success(loginUserResult);
    }
}