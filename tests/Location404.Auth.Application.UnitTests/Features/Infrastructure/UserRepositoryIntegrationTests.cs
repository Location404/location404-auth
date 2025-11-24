using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Location404.Auth.Domain.Entities;
using Location404.Auth.Domain.ValueObjects;
using Location404.Auth.Infrastructure.Context;
using Location404.Auth.Infrastructure.Services;

namespace Location404.Auth.Application.UnitTests.Features.Infrastructure;

public class UserRepositoryIntegrationTests : IDisposable
{
    private readonly UserIdentityDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<UserIdentityDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new UserIdentityDbContext(options);
        _context.Database.EnsureCreated();
        _repository = new UserRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _repository.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task AddUserAsync_ShouldAddUserToContext()
    {
        var user = User.Create(
            EmailAddress.Create("test@example.com"),
            "testuser",
            "hashedPassword");

        await _repository.AddUserAsync(user);
        await _context.SaveChangesAsync();

        var users = await _context.Users.ToListAsync();
        users.Should().HaveCount(1);
        ((string)users[0].Email).Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetUserByIdAsync_WithExistingUser_ShouldReturnUser()
    {
        var user = User.Create(
            EmailAddress.Create("test@example.com"),
            "testuser",
            "hashedPassword");

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _repository.GetUserByIdAsync(user.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        ((string)result.Email).Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetUserByIdAsync_WithNonExistentUser_ShouldReturnNull()
    {
        var result = await _repository.GetUserByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    // [Fact]
    // public async Task GetUserByEmailAsync_WithExistingUser_ShouldReturnUser()
    // {
    //     var email = "test@example.com";
    //     var user = User.Create(
    //         EmailAddress.Create(email),
    //         "testuser",
    //         "hashedPassword");

    //     await _context.Users.AddAsync(user);
    //     await _context.SaveChangesAsync();

    //     var result = await _repository.GetUserByEmailAsync(email);

    //     result.Should().NotBeNull();
    //     ((string)result!.Email).Should().Be(email);
    // }

    [Fact]
    public async Task GetUserByEmailAsync_WithNonExistentEmail_ShouldReturnNull()
    {
        var result = await _repository.GetUserByEmailAsync("nonexistent@example.com");

        result.Should().BeNull();
    }

    // [Fact]
    // public async Task ExistsByEmailAsync_WithExistingEmail_ShouldReturnTrue()
    // {
    //     var email = "test@example.com";
    //     var user = User.Create(
    //         EmailAddress.Create(email),
    //         "testuser",
    //         "hashedPassword");

    //     await _context.Users.AddAsync(user);
    //     await _context.SaveChangesAsync();

    //     var result = await _repository.ExistsByEmailAsync(email);

    //     result.Should().BeTrue();
    // }

    [Fact]
    public async Task ExistsByEmailAsync_WithNonExistentEmail_ShouldReturnFalse()
    {
        var result = await _repository.ExistsByEmailAsync("nonexistent@example.com");

        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsByUsernameAsync_WithExistingUsername_ShouldReturnTrue()
    {
        var username = "testuser";
        var user = User.Create(
            EmailAddress.Create("test@example.com"),
            username,
            "hashedPassword");

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _repository.ExistsByUsernameAsync(username);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByUsernameAsync_WithNonExistentUsername_ShouldReturnFalse()
    {
        var result = await _repository.ExistsByUsernameAsync("nonexistent");

        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsByUserIdAsync_WithExistingUserId_ShouldReturnTrue()
    {
        var user = User.Create(
            EmailAddress.Create("test@example.com"),
            "testuser",
            "hashedPassword");

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _repository.ExistsByUserIdAsync(user.Id);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByUserIdAsync_WithNonExistentUserId_ShouldReturnFalse()
    {
        var result = await _repository.ExistsByUserIdAsync(Guid.NewGuid());

        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldMarkUserAsModified()
    {
        var user = User.Create(
            EmailAddress.Create("test@example.com"),
            "testuser",
            "hashedPassword");

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        user.UpdateProfile("newusername", null, null, null);

        var result = _repository.UpdateUserAsync(user);

        result.Should().BeTrue();

        await _context.SaveChangesAsync();

        var updatedUser = await _context.Users.FindAsync(user.Id);
        updatedUser!.Username.Should().Be("newusername");
    }

    [Fact]
    public async Task DeleteUserAsync_WithExistingUser_ShouldReturnTrueAndRemoveUser()
    {
        var user = User.Create(
            EmailAddress.Create("test@example.com"),
            "testuser",
            "hashedPassword");

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _repository.DeleteUserAsync(user.Id);

        result.Should().BeTrue();

        await _context.SaveChangesAsync();

        var deletedUser = await _context.Users.FindAsync(user.Id);
        deletedUser.Should().BeNull();
    }

    [Fact]
    public async Task DeleteUserAsync_WithNonExistentUser_ShouldReturnFalse()
    {
        var result = await _repository.DeleteUserAsync(Guid.NewGuid());

        result.Should().BeFalse();
    }

    [Fact]
    public void AddRefreshToken_ShouldAddTokenToContext()
    {
        var userId = Guid.NewGuid();
        var token = RefreshToken.Create(userId, "test_token", DateTime.UtcNow.AddDays(7));

        _repository.AddRefreshToken(token, CancellationToken.None);
        _context.SaveChanges();

        var tokens = _context.RefreshTokens.ToList();
        tokens.Should().HaveCount(1);
        tokens[0].Token.Should().Be("test_token");
    }

    [Fact]
    public async Task GetByTokenAsync_WithExistingToken_ShouldReturnToken()
    {
        var userId = Guid.NewGuid();
        var tokenString = "test_token";
        var token = RefreshToken.Create(userId, tokenString, DateTime.UtcNow.AddDays(7));

        await _context.RefreshTokens.AddAsync(token);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByTokenAsync(userId, tokenString, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Token.Should().Be(tokenString);
        result.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task GetByTokenAsync_WithNonExistentToken_ShouldReturnNull()
    {
        var result = await _repository.GetByTokenAsync(Guid.NewGuid(), "nonexistent_token", CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task RevokeAllByUserAsync_ShouldRemoveAllUserTokens()
    {
        var userId = Guid.NewGuid();
        var token1 = RefreshToken.Create(userId, "token1", DateTime.UtcNow.AddDays(7));
        var token2 = RefreshToken.Create(userId, "token2", DateTime.UtcNow.AddDays(7));
        var otherUserToken = RefreshToken.Create(Guid.NewGuid(), "token3", DateTime.UtcNow.AddDays(7));

        await _context.RefreshTokens.AddRangeAsync(token1, token2, otherUserToken);
        await _context.SaveChangesAsync();

        await _repository.RevokeAllByUserAsync(userId, CancellationToken.None);
        await _context.SaveChangesAsync();

        var remainingTokens = await _context.RefreshTokens.ToListAsync();
        remainingTokens.Should().HaveCount(1);
        remainingTokens[0].Token.Should().Be("token3");
    }
}
