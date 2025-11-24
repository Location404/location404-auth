using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Location404.Auth.Application.Common.Interfaces;
using Location404.Auth.Application.Common.Result;
using Location404.Auth.Application.Features.UserManagement.Commands.CreateUserWithPasswordCommand;
using Location404.Auth.Application.UnitTests.TestHelpers;

namespace Location404.Auth.Application.UnitTests.Features.UserManagement;

public class CreateUserWithPasswordCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IEncryptPasswordService _encryptPassword;
    private readonly ILogger<CreateUserWithPasswordCommandHandler> _logger;
    private readonly CreateUserWithPasswordCommandHandler _handler;
    private readonly TestDataGenerator _dataGenerator;

    public CreateUserWithPasswordCommandHandlerTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _userRepository = Substitute.For<IUserRepository>();
        _encryptPassword = Substitute.For<IEncryptPasswordService>();
        _logger = Substitute.For<ILogger<CreateUserWithPasswordCommandHandler>>();

        _unitOfWork.Users.Returns(_userRepository);

        _handler = new CreateUserWithPasswordCommandHandler(
            _unitOfWork,
            _encryptPassword,
            _logger);

        _dataGenerator = new TestDataGenerator();
    }

    [Fact]
    public async Task HandleAsync_WithValidData_ShouldCreateUserSuccessfully()
    {
        var email = _dataGenerator.GenerateEmail();
        var username = _dataGenerator.GenerateUsername();
        var password = "ValidPassword123!";
        var hashedPassword = "hashed_password";

        _userRepository.ExistsByEmailAsync(email, Arg.Any<CancellationToken>())
            .Returns(false);

        _userRepository.ExistsByUsernameAsync(username, Arg.Any<CancellationToken>())
            .Returns(false);

        _encryptPassword.Encrypt(password)
            .Returns(hashedPassword);

        var command = new CreateUserWithPasswordCommand
        {
            Email = email,
            Username = username,
            Password = password
        };

        var result = await _handler.HandleAsync(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be(email.ToLowerInvariant());
        result.Value.Username.Should().Be(username);

        await _userRepository.Received(1).AddUserAsync(Arg.Any<Domain.Entities.User>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithExistingEmail_ShouldReturnValidationError()
    {
        var email = _dataGenerator.GenerateEmail();

        _userRepository.ExistsByEmailAsync(email, Arg.Any<CancellationToken>())
            .Returns(true);

        var command = new CreateUserWithPasswordCommand
        {
            Email = email,
            Username = _dataGenerator.GenerateUsername(),
            Password = "ValidPassword123!"
        };

        var result = await _handler.HandleAsync(command);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("EmailAlreadyInUse");
        result.Error.Type.Should().Be(ErrorType.Validation);

        await _userRepository.DidNotReceive().AddUserAsync(Arg.Any<Domain.Entities.User>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithExistingUsername_ShouldReturnValidationError()
    {
        var username = _dataGenerator.GenerateUsername();

        _userRepository.ExistsByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _userRepository.ExistsByUsernameAsync(username, Arg.Any<CancellationToken>())
            .Returns(true);

        var command = new CreateUserWithPasswordCommand
        {
            Email = _dataGenerator.GenerateEmail(),
            Username = username,
            Password = "ValidPassword123!"
        };

        var result = await _handler.HandleAsync(command);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("UsernameAlreadyInUse");
        result.Error.Type.Should().Be(ErrorType.Validation);

        await _userRepository.DidNotReceive().AddUserAsync(Arg.Any<Domain.Entities.User>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenDatabaseSaveFails_ShouldReturnDatabaseError()
    {
        var email = _dataGenerator.GenerateEmail();
        var username = _dataGenerator.GenerateUsername();
        var password = "ValidPassword123!";

        _userRepository.ExistsByEmailAsync(email, Arg.Any<CancellationToken>())
            .Returns(false);

        _userRepository.ExistsByUsernameAsync(username, Arg.Any<CancellationToken>())
            .Returns(false);

        _encryptPassword.Encrypt(password)
            .Returns("hashed_password");

        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromException<int>(new Exception("Database connection error")));

        var command = new CreateUserWithPasswordCommand
        {
            Email = email,
            Username = username,
            Password = password
        };

        var result = await _handler.HandleAsync(command);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("DatabaseError");
        result.Error.Type.Should().Be(ErrorType.Database);

        await _unitOfWork.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
    }
}
