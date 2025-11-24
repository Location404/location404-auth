using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Location404.Auth.Application.Common.Interfaces;
using Location404.Auth.Application.Features.UserManagement.Queries.GetCurrentUserInformation;
using Location404.Auth.Application.UnitTests.TestHelpers;
using Location404.Auth.Domain.Entities;
using Location404.Auth.Domain.ValueObjects;

namespace Location404.Auth.Application.UnitTests.Features.UserManagement;

public class GetCurrentUserInformationQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetCurrentUserInformationQueryHandler> _logger;
    private readonly GetCurrentUserInformationQueryHandler _handler;
    private readonly TestDataGenerator _dataGenerator;

    public GetCurrentUserInformationQueryHandlerTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _userRepository = Substitute.For<IUserRepository>();
        _logger = Substitute.For<ILogger<GetCurrentUserInformationQueryHandler>>();

        _unitOfWork.Users.Returns(_userRepository);

        _handler = new GetCurrentUserInformationQueryHandler(
            _unitOfWork,
            _logger);

        _dataGenerator = new TestDataGenerator();
    }

    [Fact]
    public async Task HandleAsync_WithValidUserId_ShouldReturnUserInformation()
    {
        var userId = Guid.NewGuid();
        var email = _dataGenerator.GenerateEmail();
        var username = _dataGenerator.GenerateUsername();
        var user = User.Create(
            EmailAddress.Create(email),
            username,
            "hashedPassword");

        _userRepository.GetUserByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        var query = new GetCurrentUserInformationQuery(userId);

        var result = await _handler.HandleAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(user.Id);
        result.Value.Username.Should().Be(user.Username);
        result.Value.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentUserId_ShouldThrowKeyNotFoundException()
    {
        var userId = Guid.NewGuid();

        _userRepository.GetUserByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var query = new GetCurrentUserInformationQuery(userId);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.HandleAsync(query));
    }

    [Fact]
    public async Task HandleAsync_WithUserWithProfileImage_ShouldReturnBase64Image()
    {
        var userId = Guid.NewGuid();
        var profileImage = new byte[] { 1, 2, 3, 4, 5 };
        var email = _dataGenerator.GenerateEmail();
        var username = _dataGenerator.GenerateUsername();
        var user = User.Create(
            EmailAddress.Create(email),
            username,
            "hashedPassword");

        user.UpdateProfile(null, null, null, profileImage);

        _userRepository.GetUserByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        var query = new GetCurrentUserInformationQuery(userId);

        var result = await _handler.HandleAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.ProfileImage.Should().Be(Convert.ToBase64String(profileImage));
    }

    [Fact]
    public async Task HandleAsync_WithUserWithoutProfileImage_ShouldReturnEmptyBase64()
    {
        var userId = Guid.NewGuid();
        var email = _dataGenerator.GenerateEmail();
        var username = _dataGenerator.GenerateUsername();
        var user = User.Create(
            EmailAddress.Create(email),
            username,
            "hashedPassword");

        _userRepository.GetUserByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        var query = new GetCurrentUserInformationQuery(userId);

        var result = await _handler.HandleAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.ProfileImage.Should().Be(Convert.ToBase64String(Array.Empty<byte>()));
    }
}
