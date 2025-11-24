using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Location404.Auth.Application.Common.Interfaces;
using Location404.Auth.Application.Common.Result;
using Location404.Auth.Application.Features.Authentication.Commands.RefreshTokenCommand;
using Location404.Auth.Application.Features.Authentication.Interfaces;
using Location404.Auth.Application.UnitTests.TestHelpers;
using Location404.Auth.Domain.Entities;

namespace Location404.Auth.Application.UnitTests.Features.Authentication;

public class RefreshTokenCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;
    private readonly RefreshTokenCommandHandler _handler;
    private readonly TestDataGenerator _dataGenerator;

    public RefreshTokenCommandHandlerTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _tokenService = Substitute.For<ITokenService>();
        _logger = Substitute.For<ILogger<RefreshTokenCommandHandler>>();

        _handler = new RefreshTokenCommandHandler(
            _tokenService,
            _logger,
            _unitOfWork);

        _dataGenerator = new TestDataGenerator();
    }

    [Fact]
    public async Task HandleAsync_WithValidRefreshToken_ShouldReturnNewTokens()
    {
        var userId = Guid.NewGuid();
        var refreshToken = _dataGenerator.GenerateToken();
        var newAccessToken = _dataGenerator.GenerateToken();
        var newRefreshToken = RefreshToken.Create(userId, _dataGenerator.GenerateToken(), DateTime.UtcNow.AddDays(7));

        _tokenService.RotateAsync(userId, refreshToken, Arg.Any<CancellationToken>())
            .Returns(Result<(string accessToken, RefreshToken newRefresh)>.Success((newAccessToken, newRefreshToken)));

        var command = new RefreshTokenCommand(userId, refreshToken);

        var result = await _handler.HandleAsync(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be(newAccessToken);
        result.Value.RefreshToken.Should().Be(newRefreshToken.Token);
        result.Value.RefreshTokenExpiresAt.Should().Be(newRefreshToken.ExpiresAtUtc);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithNullRefreshToken_ShouldReturnValidationError()
    {
        var userId = Guid.NewGuid();
        var command = new RefreshTokenCommand(userId, string.Empty);

        var result = await _handler.HandleAsync(command);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("RefreshTokenIsMissing");
        result.Error.Type.Should().Be(ErrorType.Validation);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithWhitespaceRefreshToken_ShouldReturnValidationError()
    {
        var userId = Guid.NewGuid();
        var command = new RefreshTokenCommand(userId, "   ");

        var result = await _handler.HandleAsync(command);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("RefreshTokenIsMissing");
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task HandleAsync_WhenRotateFails_ShouldReturnError()
    {
        var userId = Guid.NewGuid();
        var refreshToken = _dataGenerator.GenerateToken();
        var error = new Error("InvalidRefreshToken", "The refresh token is invalid or expired", ErrorType.Validation);

        _tokenService.RotateAsync(userId, refreshToken, Arg.Any<CancellationToken>())
            .Returns(Result<(string accessToken, RefreshToken newRefresh)>.Failure(error));

        var command = new RefreshTokenCommand(userId, refreshToken);

        var result = await _handler.HandleAsync(command);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("InvalidRefreshToken");
        result.Error.Type.Should().Be(ErrorType.Validation);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenDatabaseSaveFails_ShouldReturnDatabaseError()
    {
        var userId = Guid.NewGuid();
        var refreshToken = _dataGenerator.GenerateToken();
        var newAccessToken = _dataGenerator.GenerateToken();
        var newRefreshToken = RefreshToken.Create(userId, _dataGenerator.GenerateToken(), DateTime.UtcNow.AddDays(7));

        _tokenService.RotateAsync(userId, refreshToken, Arg.Any<CancellationToken>())
            .Returns(Result<(string accessToken, RefreshToken newRefresh)>.Success((newAccessToken, newRefreshToken)));

        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromException<int>(new Exception("Database connection error")));

        var command = new RefreshTokenCommand(userId, refreshToken);

        var result = await _handler.HandleAsync(command);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("RefreshTokenFailed");
        result.Error.Type.Should().Be(ErrorType.Database);
    }
}
