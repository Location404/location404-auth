using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Location404.Auth.Application.Common.Interfaces;
using Location404.Auth.Application.Common.Result;
using Location404.Auth.Application.Features.Authentication.Commands.AuthenticateUserWithPasswordCommand;
using Location404.Auth.Application.Features.Authentication.Interfaces;
using Location404.Auth.Application.UnitTests.TestHelpers;
using Location404.Auth.Domain.Entities;
using Location404.Auth.Domain.ValueObjects;

namespace Location404.Auth.Application.UnitTests.Features.Authentication;

public class AuthenticateUserWithPasswordCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IEncryptPasswordService _encryptPassword;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthenticateUserWithPasswordCommandHandler> _logger;
    private readonly AuthenticateUserWithPasswordCommandHandler _handler;
    private readonly TestDataGenerator _dataGenerator;

    public AuthenticateUserWithPasswordCommandHandlerTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _userRepository = Substitute.For<IUserRepository>();
        _encryptPassword = Substitute.For<IEncryptPasswordService>();
        _tokenService = Substitute.For<ITokenService>();
        _logger = Substitute.For<ILogger<AuthenticateUserWithPasswordCommandHandler>>();

        _unitOfWork.Users.Returns(_userRepository);

        _handler = new AuthenticateUserWithPasswordCommandHandler(
            _unitOfWork,
            _encryptPassword,
            _tokenService,
            _logger);

        _dataGenerator = new TestDataGenerator();
    }

    [Fact]
    public async Task HandleAsync_WithValidCredentials_ShouldReturnSuccessWithTokens()
    {
        var email = _dataGenerator.GenerateEmail();
        var password = "ValidPassword123!";
        var hashedPassword = "hashed_password";
        var user = User.Create(
            EmailAddress.Create(email),
            _dataGenerator.GenerateUsername(),
            hashedPassword);

        var accessToken = _dataGenerator.GenerateToken();
        var refreshTokenEntity = RefreshToken.Create(user.Id, _dataGenerator.GenerateToken(), DateTime.UtcNow.AddDays(7));

        _userRepository.GetUserByEmailAsync(email, Arg.Any<CancellationToken>())
            .Returns(user);

        _encryptPassword.Verify(password, hashedPassword)
            .Returns(true);

        _tokenService.GenerateAccessToken(user.Id, user.Username, Arg.Any<IEnumerable<string>>())
            .Returns(Result<string>.Success(accessToken));

        _tokenService.IssueRefreshTokenAsync(user.Id, Arg.Any<CancellationToken>())
            .Returns(Result<RefreshToken>.Success(refreshTokenEntity));

        var command = new AuthenticateUserWithPasswordCommand(email, password);

        var result = await _handler.HandleAsync(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be(accessToken);
        result.Value.RefreshToken.Should().Be(refreshTokenEntity.Token);
        result.Value.Email.Should().Be(email.ToLowerInvariant());
        result.Value.UserId.Should().Be(user.Id);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithInvalidEmail_ShouldReturnNotFoundError()
    {
        var email = _dataGenerator.GenerateEmail();

        _userRepository.GetUserByEmailAsync(email, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var command = new AuthenticateUserWithPasswordCommand(email, "password");

        var result = await _handler.HandleAsync(command);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("UserEmailNotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithUserWithoutPassword_ShouldReturnValidationError()
    {
        var email = _dataGenerator.GenerateEmail();
        var user = User.Create(
            EmailAddress.Create(email),
            _dataGenerator.GenerateUsername(),
            "Google",
            _dataGenerator.GenerateProviderKey());

        _userRepository.GetUserByEmailAsync(email, Arg.Any<CancellationToken>())
            .Returns(user);

        var command = new AuthenticateUserWithPasswordCommand(email, "password");

        var result = await _handler.HandleAsync(command);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("PasswordNotSet");
        result.Error.Type.Should().Be(ErrorType.Validation);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithInvalidPassword_ShouldReturnValidationError()
    {
        var email = _dataGenerator.GenerateEmail();
        var password = "wrong_password";
        var hashedPassword = "hashed_password";
        var user = User.Create(
            EmailAddress.Create(email),
            _dataGenerator.GenerateUsername(),
            hashedPassword);

        _userRepository.GetUserByEmailAsync(email, Arg.Any<CancellationToken>())
            .Returns(user);

        _encryptPassword.Verify(password, hashedPassword)
            .Returns(false);

        var command = new AuthenticateUserWithPasswordCommand(email, password);

        var result = await _handler.HandleAsync(command);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("InvalidCredentials");
        result.Error.Type.Should().Be(ErrorType.Validation);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenRefreshTokenFails_ShouldReturnFailure()
    {
        var email = _dataGenerator.GenerateEmail();
        var password = "ValidPassword123!";
        var hashedPassword = "hashed_password";
        var user = User.Create(
            EmailAddress.Create(email),
            _dataGenerator.GenerateUsername(),
            hashedPassword);

        _userRepository.GetUserByEmailAsync(email, Arg.Any<CancellationToken>())
            .Returns(user);

        _encryptPassword.Verify(password, hashedPassword)
            .Returns(true);

        _tokenService.GenerateAccessToken(user.Id, user.Username, Arg.Any<IEnumerable<string>>())
            .Returns(Result<string>.Success(_dataGenerator.GenerateToken()));

        var error = new Error("TokenError", "Failed to issue refresh token", ErrorType.Failure);
        _tokenService.IssueRefreshTokenAsync(user.Id, Arg.Any<CancellationToken>())
            .Returns(Result<RefreshToken>.Failure(error));

        var command = new AuthenticateUserWithPasswordCommand(email, password);

        var result = await _handler.HandleAsync(command);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("TokenError");

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenDatabaseSaveFails_ShouldReturnDatabaseError()
    {
        var email = _dataGenerator.GenerateEmail();
        var password = "ValidPassword123!";
        var hashedPassword = "hashed_password";
        var user = User.Create(
            EmailAddress.Create(email),
            _dataGenerator.GenerateUsername(),
            hashedPassword);

        var refreshTokenEntity = RefreshToken.Create(user.Id, _dataGenerator.GenerateToken(), DateTime.UtcNow.AddDays(7));

        _userRepository.GetUserByEmailAsync(email, Arg.Any<CancellationToken>())
            .Returns(user);

        _encryptPassword.Verify(password, hashedPassword)
            .Returns(true);

        _tokenService.GenerateAccessToken(user.Id, user.Username, Arg.Any<IEnumerable<string>>())
            .Returns(Result<string>.Success(_dataGenerator.GenerateToken()));

        _tokenService.IssueRefreshTokenAsync(user.Id, Arg.Any<CancellationToken>())
            .Returns(Result<RefreshToken>.Success(refreshTokenEntity));

        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromException<int>(new Exception("Database error")));

        var command = new AuthenticateUserWithPasswordCommand(email, password);

        var result = await _handler.HandleAsync(command);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("AuthenticationFailed");
        result.Error.Type.Should().Be(ErrorType.Database);
    }
}
