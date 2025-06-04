using Microsoft.EntityFrameworkCore;

using UserIdentity.Domain.Entities;
using UserIdentity.Infra.Context;
using UserIdentity.Infra.Persistence;

namespace UserIdentity.UnitTests.Services.Persistence;

public class UserRepositoryTests : IDisposable
{
    private readonly UserIdentityContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<UserIdentityContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new UserIdentityContext(options);
        _repository = new UserRepository(_context);
    }

    private static UserApplication CreateTestUser(string username = "testuser", string email = "testuser@example.com")
    {
        return new(
            username: username,
            email: email,
            passwordHash: "hashedpassword",
            passwordSalt: "salt"
        );
    }


    #region [ AddAsync Tests ]

    [Fact]
    public async Task AddAsync_ShouldAddUserToDatabase()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        await _repository.AddAsync(user);
        await _context.SaveChangesAsync();

        // Assert
        var addedUser = await _context.Set<UserApplication>().FindAsync(user.Id);
        Assert.NotNull(addedUser);
        Assert.Equal(user.Username, addedUser.Username);
        Assert.Equal(user.Email, addedUser.Email);
    }

    [Fact]
    public async Task AddAsync_WithValidUser_ShouldNotThrow()
    {
        // Arrange
        var user = CreateTestUser();
        using var cts = new CancellationTokenSource();

        // Act & Assert - Should not throw any exception
        var exception = await Record.ExceptionAsync(() => _repository.AddAsync(user, cts.Token));
        Assert.Null(exception);
    }

    #endregion

    #region [ ExistsByUsernameOrEmailAsync Tests ]

    [Theory]
    [InlineData("testuser", "testuser@example.com", true)]
    [InlineData("testuser", null, true)]
    [InlineData(null, "testuser@example.com", true)]
    [InlineData("nonexistent", null, false)]
    [InlineData(null, "nonexistent@example.com", false)]
    [InlineData("nonexistent", "nonexistent@example.com", false)]
    public async Task ExistsByUsernameOrEmailAsync_ShouldReturnCorrectResult(
        string? username,
        string? email,
        bool expectedExists)
    {
        // Arrange
        if (expectedExists && (username == "testuser" || email == "testuser@example.com"))
        {
            var existingUser = CreateTestUser();
            _context.Set<UserApplication>().Add(existingUser);
            await _context.SaveChangesAsync();
        }

        // Act
        var result = await _repository.ExistsByUsernameOrEmailAsync(username, email);

        // Assert
        Assert.Equal(expectedExists, result);
    }

    [Fact]
    public async Task ExistsByUsernameOrEmailAsync_WithValidParameters_ShouldNotThrow()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        // Act & Assert - Should not throw any exception
        var exception = await Record.ExceptionAsync(() => _repository.ExistsByUsernameOrEmailAsync("test", "test@example.com", cts.Token));
        Assert.Null(exception);
    }

    #endregion

    #region [ GetByEmailAsync Tests ]

    [Fact]
    public async Task GetByEmailAsync_WithExistingEmail_ShouldReturnUser()
    {
        // Arrange
        var user = CreateTestUser();
        _context.Set<UserApplication>().Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync("testuser@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Username, result.Username);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_WithNonExistingEmail_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByEmailAsync("nonexistent@example.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ShouldNotThrow()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        // Act & Assert - Should not throw any exception
        var exception = await Record.ExceptionAsync(() => _repository.GetByEmailAsync("test@example.com", cts.Token));
        Assert.Null(exception);
    }

    [Fact]
    public async Task GetByEmailAsync_WithDuplicateEmails_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var user1 = CreateTestUser("user1", "duplicate@example.com");
        var user2 = CreateTestUser("user2", "duplicate@example.com");

        _context.Set<UserApplication>().AddRange(user1, user2);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.GetByEmailAsync("duplicate@example.com"));
    }

    #endregion

    #region [ GetByIdAsync Tests ]

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnUser()
    {
        // Arrange
        var user = CreateTestUser();
        _context.Set<UserApplication>().Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Username, result.Username);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldNotThrow()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        // Act & Assert - Should not throw any exception
        var exception = await Record.ExceptionAsync(() => _repository.GetByIdAsync(Guid.NewGuid(), cts.Token));
        Assert.Null(exception);
    }

    #endregion

    #region [ GetByUsernameAsync Tests ]

    [Fact]
    public async Task GetByUsernameAsync_WithExistingUsername_ShouldReturnUser()
    {
        // Arrange
        var user = CreateTestUser();
        _context.Set<UserApplication>().Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUsernameAsync("testuser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Username, result.Username);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task GetByUsernameAsync_WithNonExistingUsername_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByUsernameAsync("nonexistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUsernameAsync_WithValidUsername_ShouldNotThrow()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        // Act & Assert - Should not throw any exception
        var exception = await Record.ExceptionAsync(() => _repository.GetByUsernameAsync("testuser", cts.Token));
        Assert.Null(exception);
    }

    [Fact]
    public async Task GetByUsernameAsync_WithDuplicateUsernames_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var user1 = CreateTestUser("duplicate", "user1@example.com");
        var user2 = CreateTestUser("duplicate", "user2@example.com");

        _context.Set<UserApplication>().AddRange(user1, user2);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.GetByUsernameAsync("duplicate"));
    }

    #endregion

    #region [ GetByUsernameOrEmailAsync Tests ]

    [Theory]
    [InlineData("testuser", null)]
    [InlineData(null, "testuser@example.com")]
    [InlineData("testuser", "testuser@example.com")]
    public async Task GetByUsernameOrEmailAsync_WithExistingData_ShouldReturnUser(
        string? username, string? email)
    {
        // Arrange
        var user = CreateTestUser();
        _context.Set<UserApplication>().Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUsernameOrEmailAsync(username, email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Username, result.Username);
        Assert.Equal(user.Email, result.Email);
    }

    [Theory]
    [InlineData("nonexistent", null)]
    [InlineData(null, "nonexistent@example.com")]
    [InlineData("nonexistent", "nonexistent@example.com")]
    public async Task GetByUsernameOrEmailAsync_WithNonExistingData_ShouldReturnNull(
        string? username, string? email)
    {
        // Act
        var result = await _repository.GetByUsernameOrEmailAsync(username, email);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUsernameOrEmailAsync_WithValidParameters_ShouldNotThrow()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        // Act & Assert - Should not throw any exception
        var exception = await Record.ExceptionAsync(() => _repository.GetByUsernameOrEmailAsync("test", "test@example.com", cts.Token));
        Assert.Null(exception);
    }

    [Fact]
    public async Task GetByUsernameOrEmailAsync_WithDuplicateUsers_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var user1 = CreateTestUser("user1", "duplicate@example.com");
        var user2 = CreateTestUser("user2", "duplicate@example.com");

        _context.Set<UserApplication>().AddRange(user1, user2);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.GetByUsernameOrEmailAsync(email: "duplicate@example.com"));
    }

    #endregion

    #region [ Edge Cases and Validation Tests ]

    [Fact]
    public async Task ExistsByUsernameOrEmailAsync_WithBothParametersNull_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.ExistsByUsernameOrEmailAsync(null, null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetByUsernameOrEmailAsync_WithBothParametersNull_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByUsernameOrEmailAsync(null, null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ExistsByUsernameOrEmailAsync_WithEmptyStrings_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.ExistsByUsernameOrEmailAsync("", "");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetByUsernameOrEmailAsync_WithEmptyStrings_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByUsernameOrEmailAsync("", "");

        // Assert
        Assert.Null(result);
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
        GC.SuppressFinalize(this);
    }
}