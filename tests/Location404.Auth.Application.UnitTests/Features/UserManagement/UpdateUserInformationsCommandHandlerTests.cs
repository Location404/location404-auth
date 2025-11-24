using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Location404.Auth.Application.Common.Interfaces;
using Location404.Auth.Application.Common.Result;
using Location404.Auth.Application.Features.UserManagement.Commands.UpdateUserInformationsCommand;
using Location404.Auth.Application.UnitTests.TestHelpers;
using Location404.Auth.Domain.Entities;
using Location404.Auth.Domain.ValueObjects;
using LiteBus.Messaging.Abstractions;

namespace Location404.Auth.Application.UnitTests.Features.UserManagement;

public class UpdateUserInformationsCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IEncryptPasswordService _encryptPassword;
    private readonly ILogger<UpdateUserInformationsCommandHandler> _logger;
    private readonly IAsyncMessageHandler<UpdateUserInformationsCommand, Result<UpdateUserInformationsCommandResponse>> _handler;
    private readonly TestDataGenerator _dataGenerator;

    public UpdateUserInformationsCommandHandlerTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _userRepository = Substitute.For<IUserRepository>();
        _encryptPassword = Substitute.For<IEncryptPasswordService>();
        _logger = Substitute.For<ILogger<UpdateUserInformationsCommandHandler>>();

        _unitOfWork.Users.Returns(_userRepository);

        _handler = new UpdateUserInformationsCommandHandler(
            _unitOfWork,
            _encryptPassword,
            _logger);

        _dataGenerator = new TestDataGenerator();
    }

    [Fact]
    public async Task HandleAsync_WhenUserNotFound_ShouldReturnNotFoundError()
    {
        var userId = Guid.NewGuid();

        _userRepository.GetUserByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var command = new UpdateUserInformationsCommand
        {
            Id = userId,
            Email = _dataGenerator.GenerateEmail()
        };

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("UserNotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenEmailAlreadyExists_ShouldReturnValidationError()
    {
        var userId = Guid.NewGuid();
        var existingEmail = _dataGenerator.GenerateEmail();
        var user = User.Create(
            EmailAddress.Create(_dataGenerator.GenerateEmail()),
            _dataGenerator.GenerateUsername(),
            "hashedPassword");

        _userRepository.GetUserByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        _userRepository.ExistsByEmailAsync(existingEmail, Arg.Any<CancellationToken>())
            .Returns(true);

        var command = new UpdateUserInformationsCommand
        {
            Id = userId,
            Email = existingEmail
        };

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("EmailAlreadyInUse");
        result.Error.Type.Should().Be(ErrorType.Validation);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenNewPasswordSameAsOld_ShouldReturnValidationError()
    {
        var userId = Guid.NewGuid();
        var oldPassword = "hashedOldPassword";
        var newPassword = "SamePassword123!";
        var user = User.Create(
            EmailAddress.Create(_dataGenerator.GenerateEmail()),
            _dataGenerator.GenerateUsername(),
            oldPassword);

        _userRepository.GetUserByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        _encryptPassword.Verify(newPassword, oldPassword)
            .Returns(true);

        var command = new UpdateUserInformationsCommand
        {
            Id = userId,
            Password = newPassword
        };

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("SamePassword");
        result.Error.Type.Should().Be(ErrorType.Validation);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenUsernameAlreadyExists_ShouldReturnValidationError()
    {
        var userId = Guid.NewGuid();
        var existingUsername = _dataGenerator.GenerateUsername();
        var user = User.Create(
            EmailAddress.Create(_dataGenerator.GenerateEmail()),
            _dataGenerator.GenerateUsername(),
            "hashedPassword");

        _userRepository.GetUserByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        _userRepository.ExistsByUsernameAsync(existingUsername, Arg.Any<CancellationToken>())
            .Returns(true);

        var command = new UpdateUserInformationsCommand
        {
            Id = userId,
            Username = existingUsername
        };

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("UsernameAlreadyInUse");
        result.Error.Type.Should().Be(ErrorType.Validation);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithValidEmailUpdate_ShouldSucceed()
    {
        var userId = Guid.NewGuid();
        var newEmail = _dataGenerator.GenerateEmail();
        var user = User.Create(
            EmailAddress.Create(_dataGenerator.GenerateEmail()),
            _dataGenerator.GenerateUsername(),
            "hashedPassword");

        _userRepository.GetUserByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        _userRepository.ExistsByEmailAsync(newEmail, Arg.Any<CancellationToken>())
            .Returns(false);

        var command = new UpdateUserInformationsCommand
        {
            Id = userId,
            Email = newEmail
        };

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be(newEmail.ToLowerInvariant());

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithValidUsernameUpdate_ShouldSucceed()
    {
        var userId = Guid.NewGuid();
        var newUsername = _dataGenerator.GenerateUsername();
        var user = User.Create(
            EmailAddress.Create(_dataGenerator.GenerateEmail()),
            _dataGenerator.GenerateUsername(),
            "hashedPassword");

        _userRepository.GetUserByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        _userRepository.ExistsByUsernameAsync(newUsername, Arg.Any<CancellationToken>())
            .Returns(false);

        var command = new UpdateUserInformationsCommand
        {
            Id = userId,
            Username = newUsername
        };

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Username.Should().Be(newUsername);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithValidPasswordUpdate_ShouldSucceed()
    {
        var userId = Guid.NewGuid();
        var oldPassword = "hashedOldPassword";
        var newPassword = "NewPassword123!";
        var newHashedPassword = "hashedNewPassword";
        var user = User.Create(
            EmailAddress.Create(_dataGenerator.GenerateEmail()),
            _dataGenerator.GenerateUsername(),
            oldPassword);

        _userRepository.GetUserByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        _encryptPassword.Verify(newPassword, oldPassword)
            .Returns(false);

        _encryptPassword.Encrypt(newPassword)
            .Returns(newHashedPassword);

        var command = new UpdateUserInformationsCommand
        {
            Id = userId,
            Password = newPassword
        };

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithProfileImageUpdate_ShouldSucceed()
    {
        var userId = Guid.NewGuid();
        var imageBytes = new byte[] { 1, 2, 3, 4, 5 };
        var user = User.Create(
            EmailAddress.Create(_dataGenerator.GenerateEmail()),
            _dataGenerator.GenerateUsername(),
            "hashedPassword");

        _userRepository.GetUserByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        var formFile = Substitute.For<IFormFile>();
        formFile.CopyToAsync(Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(async callInfo =>
            {
                var stream = callInfo.Arg<Stream>();
                await stream.WriteAsync(imageBytes);
            });

        var command = new UpdateUserInformationsCommand
        {
            Id = userId,
            ProfileImage = formFile
        };

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.ProfileImage.Should().NotBeNullOrEmpty();

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithAllFieldsUpdate_ShouldSucceed()
    {
        var userId = Guid.NewGuid();
        var newEmail = _dataGenerator.GenerateEmail();
        var newUsername = _dataGenerator.GenerateUsername();
        var newPassword = "NewPassword123!";
        var newHashedPassword = "hashedNewPassword";
        var imageBytes = new byte[] { 1, 2, 3, 4, 5 };

        var user = User.Create(
            EmailAddress.Create(_dataGenerator.GenerateEmail()),
            _dataGenerator.GenerateUsername(),
            "hashedOldPassword");

        _userRepository.GetUserByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        _userRepository.ExistsByEmailAsync(newEmail, Arg.Any<CancellationToken>())
            .Returns(false);

        _userRepository.ExistsByUsernameAsync(newUsername, Arg.Any<CancellationToken>())
            .Returns(false);

        _encryptPassword.Verify(newPassword, user.Password!)
            .Returns(false);

        _encryptPassword.Encrypt(newPassword)
            .Returns(newHashedPassword);

        var formFile = Substitute.For<IFormFile>();
        formFile.CopyToAsync(Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(async callInfo =>
            {
                var stream = callInfo.Arg<Stream>();
                await stream.WriteAsync(imageBytes);
            });

        var command = new UpdateUserInformationsCommand
        {
            Id = userId,
            Email = newEmail,
            Username = newUsername,
            Password = newPassword,
            ProfileImage = formFile
        };

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be(newEmail.ToLowerInvariant());
        result.Value.Username.Should().Be(newUsername);
        result.Value.ProfileImage.Should().NotBeNullOrEmpty();

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
