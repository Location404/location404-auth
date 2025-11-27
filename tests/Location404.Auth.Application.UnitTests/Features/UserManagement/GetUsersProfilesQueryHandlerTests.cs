using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Location404.Auth.Application.Common.Interfaces;
using Location404.Auth.Application.Features.UserManagement.Queries.GetUsersProfiles;
using Location404.Auth.Application.UnitTests.TestHelpers;
using Location404.Auth.Domain.Entities;
using Location404.Auth.Domain.ValueObjects;

namespace Location404.Auth.Application.UnitTests.Features.UserManagement;

public class GetUsersProfilesQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUsersProfilesQueryHandler> _logger;
    private readonly GetUsersProfilesQueryHandler _handler;
    private readonly TestDataGenerator _dataGenerator;

    public GetUsersProfilesQueryHandlerTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _userRepository = Substitute.For<IUserRepository>();
        _logger = Substitute.For<ILogger<GetUsersProfilesQueryHandler>>();

        _unitOfWork.Users.Returns(_userRepository);

        _handler = new GetUsersProfilesQueryHandler(
            _unitOfWork,
            _logger);

        _dataGenerator = new TestDataGenerator();
    }

    [Fact]
    public async Task HandleAsync_WithNullUserIds_ShouldReturnEmptyList()
    {
        var query = new GetUsersProfilesQuery(null!);

        var result = await _handler.HandleAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_WithEmptyUserIds_ShouldReturnEmptyList()
    {
        var query = new GetUsersProfilesQuery(new List<Guid>());

        var result = await _handler.HandleAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_WithValidUserIds_ShouldReturnUserProfiles()
    {
        var userId = Guid.NewGuid();
        var email = _dataGenerator.GenerateEmail();
        var username = _dataGenerator.GenerateUsername();
        var user = User.Create(
            EmailAddress.Create(email),
            username,
            "hashedPassword");

        _userRepository.GetUsersByIdsAsync(
            Arg.Is<List<Guid>>(ids => ids.Contains(userId)),
            Arg.Any<CancellationToken>())
            .Returns(new List<User> { user });

        var query = new GetUsersProfilesQuery(new List<Guid> { userId });

        var result = await _handler.HandleAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].Id.Should().Be(user.Id);
        result.Value[0].Username.Should().Be(user.Username);
    }

    [Fact]
    public async Task HandleAsync_WithMultipleUserIds_ShouldReturnAllProfiles()
    {
        var user1 = User.Create(
            EmailAddress.Create(_dataGenerator.GenerateEmail()),
            _dataGenerator.GenerateUsername(),
            "hashedPassword1");

        var user2 = User.Create(
            EmailAddress.Create(_dataGenerator.GenerateEmail()),
            _dataGenerator.GenerateUsername(),
            "hashedPassword2");

        var userIds = new List<Guid> { user1.Id, user2.Id };

        _userRepository.GetUsersByIdsAsync(
            Arg.Is<List<Guid>>(ids => ids.Count == 2),
            Arg.Any<CancellationToken>())
            .Returns(new List<User> { user1, user2 });

        var query = new GetUsersProfilesQuery(userIds);

        var result = await _handler.HandleAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().Contain(p => p.Id == user1.Id && p.Username == user1.Username);
        result.Value.Should().Contain(p => p.Id == user2.Id && p.Username == user2.Username);
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

        _userRepository.GetUsersByIdsAsync(
            Arg.Is<List<Guid>>(ids => ids.Contains(userId)),
            Arg.Any<CancellationToken>())
            .Returns(new List<User> { user });

        var query = new GetUsersProfilesQuery(new List<Guid> { userId });

        var result = await _handler.HandleAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Value[0].ProfileImage.Should().Be(Convert.ToBase64String(profileImage));
    }

    [Fact]
    public async Task HandleAsync_WithUserWithoutProfileImage_ShouldReturnEmptyString()
    {
        var userId = Guid.NewGuid();
        var email = _dataGenerator.GenerateEmail();
        var username = _dataGenerator.GenerateUsername();
        var user = User.Create(
            EmailAddress.Create(email),
            username,
            "hashedPassword");

        _userRepository.GetUsersByIdsAsync(
            Arg.Is<List<Guid>>(ids => ids.Contains(userId)),
            Arg.Any<CancellationToken>())
            .Returns(new List<User> { user });

        var query = new GetUsersProfilesQuery(new List<Guid> { userId });

        var result = await _handler.HandleAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Value[0].ProfileImage.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_WithMixedProfileImages_ShouldHandleBothCases()
    {
        var profileImage = new byte[] { 10, 20, 30 };

        var userWithImage = User.Create(
            EmailAddress.Create(_dataGenerator.GenerateEmail()),
            _dataGenerator.GenerateUsername(),
            "hashedPassword1");
        userWithImage.UpdateProfile(null, null, null, profileImage);

        var userWithoutImage = User.Create(
            EmailAddress.Create(_dataGenerator.GenerateEmail()),
            _dataGenerator.GenerateUsername(),
            "hashedPassword2");

        var userIds = new List<Guid> { userWithImage.Id, userWithoutImage.Id };

        _userRepository.GetUsersByIdsAsync(
            Arg.Is<List<Guid>>(ids => ids.Count == 2),
            Arg.Any<CancellationToken>())
            .Returns(new List<User> { userWithImage, userWithoutImage });

        var query = new GetUsersProfilesQuery(userIds);

        var result = await _handler.HandleAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);

        var profileWithImage = result.Value.First(p => p.Id == userWithImage.Id);
        profileWithImage.ProfileImage.Should().Be(Convert.ToBase64String(profileImage));

        var profileWithoutImage = result.Value.First(p => p.Id == userWithoutImage.Id);
        profileWithoutImage.ProfileImage.Should().BeEmpty();
    }
}
