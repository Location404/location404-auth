using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

using Moq;

using UserIdentity.Domain.Entities;
using UserIdentity.Infra.Context;
using UserIdentity.Infra.Persistence;
using UserIdentity.UnitTests.Helpers;

namespace UserIdentity.UnitTests.Services.Persistence;

public sealed class UnitOfWorkIntegrationTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly UserIdentityContext _context;
    private readonly Mock<ILogger<UnitOfWork>> _loggerMock;
    private readonly UserRepository _userRepository;
    private readonly UnitOfWork _unitOfWork;

    public UnitOfWorkIntegrationTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<UserIdentityContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new UserIdentityContext(options);
        _context.Database.EnsureCreated();

        _loggerMock = new Mock<ILogger<UnitOfWork>>();
        _userRepository = new UserRepository(_context);
        _unitOfWork = new UnitOfWork(_context, _userRepository, _loggerMock.Object);
    }

    #region [ SaveChangesAsync Tests ]

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistChanges_WhenUserIsAdded()
    {
        // Arrange
        var user = CreateValidUser("testuser", "test@example.com");
        await _unitOfWork.UserRepository.AddAsync(user);

        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        Assert.Equal(1, result);
        var savedUser = await _context.Set<UserApplication>().FirstOrDefaultAsync(u => u.Id == user.Id);

        Assert.NotNull(savedUser);
        Assert.Equal(user.Username, savedUser.Username);
        Assert.Equal(user.Email, savedUser.Email);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldReturnZero_WhenNoChangesToSave()
    {
        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldRespectCancellationToken_WhenTokenIsCancelled()
    {
        // Arrange
        var user = CreateValidUser("testuser", "test@example.com");
        await _unitOfWork.UserRepository.AddAsync(user);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => _unitOfWork.SaveChangesAsync(cts.Token));
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldLogError_WhenDbUpdateExceptionOccurs()
    {
        // Arrange
        var user1 = CreateValidUser("testuser", "test@example.com");
        var user2 = CreateValidUser("testuser", "test@example.com");

        await _unitOfWork.UserRepository.AddAsync(user1);
        await _unitOfWork.SaveChangesAsync();

        await _unitOfWork.UserRepository.AddAsync(user2);

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => _unitOfWork.SaveChangesAsync());
    }

    #endregion

    #region [ Transaction Tests ]

    [Fact]
    public async Task BeginTransactionAsync_ShouldReturnTransaction_WhenSuccessful()
    {
        // Act
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        // Assert
        Assert.NotNull(transaction);
        Assert.IsAssignableFrom<IDbContextTransaction>(transaction);

        // Verify logging
        _loggerMock.VerifyLog(LogLevel.Debug, "Iniciando nova transação de banco de dados", Times.Once);
    }

    [Fact]
    public async Task CommitTransactionAsync_ShouldCommitChanges_WhenTransactionIsValid()
    {
        // Arrange
        var user = CreateValidUser("testuser", "test@example.com");

        using var transaction = await _unitOfWork.BeginTransactionAsync();
        await _unitOfWork.UserRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Act
        await _unitOfWork.CommitTransactionAsync(transaction);

        // Assert
        var savedUser = await _context.Set<UserApplication>().FirstOrDefaultAsync(u => u.Id == user.Id);

        Assert.NotNull(savedUser);

        // using verifylog
        _loggerMock.VerifyLog(LogLevel.Debug, "Transação confirmada com sucesso", Times.Once);
    }

    [Fact]
    public async Task CommitTransactionAsync_ShouldThrowArgumentNullException_WhenTransactionIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _unitOfWork.CommitTransactionAsync(null!));
    }

    [Fact]
    public async Task RollbackTransactionAsync_ShouldRollbackChanges_WhenTransactionIsValid()
    {
        // Arrange
        var user = CreateValidUser("testuser", "test@example.com");

        using var transaction = await _unitOfWork.BeginTransactionAsync();
        await _unitOfWork.UserRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Act
        await _unitOfWork.RollbackTransactionAsync(transaction);

        // Assert - User should not exist after rollback
        var userExists = await _unitOfWork.UserRepository.ExistsByUsernameOrEmailAsync("testuser", "test@example.com");
        Assert.False(userExists);

        // Verify logging
        _loggerMock.VerifyLog(LogLevel.Debug, "Transação desfeita com sucesso", Times.Once);
    }

    [Fact]
    public async Task RollbackTransactionAsync_ShouldThrowArgumentNullException_WhenTransactionIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _unitOfWork.RollbackTransactionAsync(null!));
    }

    #endregion

    #region [ ExecuteTransactionAsync Tests ]

    [Fact]
    public async Task ExecuteTransactionAsync_ShouldCommitTransaction_WhenFunctionSucceeds()
    {
        // Arrange
        var user = CreateValidUser("testuser", "test@example.com");

        // Act
        var result = await _unitOfWork.ExecuteTransactionAsync(async () =>
        {
            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return "Success";
        });

        // Assert
        Assert.Equal("Success", result);

        var savedUser = await _unitOfWork.UserRepository.GetByIdAsync(user.Id);
        Assert.NotNull(savedUser);
        Assert.Equal(user.Username, savedUser.Username);
    }

    [Fact]
    public async Task ExecuteTransactionAsync_ShouldRollbackTransaction_WhenFunctionThrows()
    {
        // Arrange
        var user = CreateValidUser("testuser", "test@example.com");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _unitOfWork.ExecuteTransactionAsync<string>(async () =>
            {
                await _unitOfWork.UserRepository.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();
                throw new InvalidOperationException("Test exception");
            }));

        // Assert - User should not exist after rollback
        var userExists = await _unitOfWork.UserRepository.ExistsByUsernameOrEmailAsync("testuser", "test@example.com");
        Assert.False(userExists);
    }

    [Fact]
    public async Task ExecuteTransactionAsync_ShouldReturnCorrectResult_WhenFunctionReturnsValue()
    {
        // Arrange
        var user = CreateValidUser("testuser", "test@example.com");

        // Act
        var result = await _unitOfWork.ExecuteTransactionAsync(async () =>
        {
            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return new { UserId = user.Id, Success = true };
        });

        // Assert
        Assert.Equal(user.Id, result.UserId);
        Assert.True(result.Success);
    }

    #endregion

    #region [ Repository Integration Tests ]

    [Fact]
    public async Task UserRepository_ShouldWorkCorrectly_WithUnitOfWork()
    {
        // Arrange
        var user = CreateValidUser("integrationuser", "integration@example.com");

        // Act - Add user through UnitOfWork
        await _unitOfWork.UserRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Assert - Retrieve user through UnitOfWork
        var retrievedUser = await _unitOfWork.UserRepository.GetByEmailAsync("integration@example.com");
        Assert.NotNull(retrievedUser);
        Assert.Equal(user.Id, retrievedUser.Id);
        Assert.Equal(user.Username, retrievedUser.Username);

        // Assert - Check existence through UnitOfWork
        var exists = await _unitOfWork.UserRepository.ExistsByUsernameOrEmailAsync("integrationuser");
        Assert.True(exists);
    }

    [Fact]
    public async Task UserRepository_ShouldHandleComplexScenario_WithMultipleOperations()
    {
        // Arrange
        var user1 = CreateValidUser("user1", "user1@example.com");
        var user2 = CreateValidUser("user2", "user2@example.com");
        var user3 = CreateValidUser("user3", "user3@example.com");

        // Act - Execute multiple operations in a transaction
        var result = await _unitOfWork.ExecuteTransactionAsync(async () =>
        {
            // Add multiple users
            await _unitOfWork.UserRepository.AddAsync(user1);
            await _unitOfWork.UserRepository.AddAsync(user2);
            await _unitOfWork.UserRepository.AddAsync(user3);

            var saveResult = await _unitOfWork.SaveChangesAsync();

            // Verify they exist
            var user1Exists = await _unitOfWork.UserRepository.ExistsByUsernameOrEmailAsync("user1");
            var user2Exists = await _unitOfWork.UserRepository.ExistsByUsernameOrEmailAsync("user2");
            var user3Exists = await _unitOfWork.UserRepository.ExistsByUsernameOrEmailAsync("user3");

            return new { SavedUsers = saveResult, AllExist = user1Exists && user2Exists && user3Exists };
        });

        // Assert
        Assert.Equal(3, result.SavedUsers);
        Assert.True(result.AllExist);

        // Verify persistence after transaction
        var finalUser1 = await _unitOfWork.UserRepository.GetByUsernameAsync("user1");
        var finalUser2 = await _unitOfWork.UserRepository.GetByEmailAsync("user2@example.com");
        var finalUser3 = await _unitOfWork.UserRepository.GetByIdAsync(user3.Id);

        Assert.NotNull(finalUser1);
        Assert.NotNull(finalUser2);
        Assert.NotNull(finalUser3);
    }

    [Fact]
    public async Task UserRepository_ShouldRollbackCorrectly_WhenTransactionFails()
    {
        // Arrange
        var user1 = CreateValidUser("rollbackuser1", "rollback1@example.com");
        var user2 = CreateValidUser("rollbackuser2", "rollback2@example.com");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _unitOfWork.ExecuteTransactionAsync<string>(async () =>
            {
                await _unitOfWork.UserRepository.AddAsync(user1);
                await _unitOfWork.SaveChangesAsync();

                // Verify user1 was added
                var user1Exists = await _unitOfWork.UserRepository.ExistsByUsernameOrEmailAsync("rollbackuser1");
                Assert.True(user1Exists);

                await _unitOfWork.UserRepository.AddAsync(user2);
                await _unitOfWork.SaveChangesAsync();

                // Force an exception
                throw new InvalidOperationException("Forcing rollback");
            }));

        // Assert - Both users should not exist after rollback
        var user1ExistsAfterRollback = await _unitOfWork.UserRepository.ExistsByUsernameOrEmailAsync("rollbackuser1");
        var user2ExistsAfterRollback = await _unitOfWork.UserRepository.ExistsByUsernameOrEmailAsync("rollbackuser2");

        Assert.False(user1ExistsAfterRollback);
        Assert.False(user2ExistsAfterRollback);
    }

    #endregion

    #region [ UserRepository Specific Tests ]

    [Fact]
    public async Task UserRepository_AddAsync_ShouldWorkWithUnitOfWork()
    {
        // Arrange
        var user = CreateValidUser("addtest", "addtest@example.com");

        // Act
        await _unitOfWork.UserRepository.AddAsync(user);
        var changes = await _unitOfWork.SaveChangesAsync();

        // Assert
        Assert.Equal(1, changes);
        var exists = await _unitOfWork.UserRepository.ExistsByUsernameOrEmailAsync("addtest");
        Assert.True(exists);
    }

    [Fact]
    public async Task UserRepository_GetByUsernameOrEmailAsync_ShouldHandleNullValues()
    {
        // Arrange
        var user = CreateValidUser("nulltest", "nulltest@example.com");
        await _unitOfWork.UserRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Act & Assert
        var resultByUsername = await _unitOfWork.UserRepository.GetByUsernameOrEmailAsync("nulltest", null);
        var resultByEmail = await _unitOfWork.UserRepository.GetByUsernameOrEmailAsync(null, "nulltest@example.com");
        var resultByBoth = await _unitOfWork.UserRepository.GetByUsernameOrEmailAsync("nulltest", "nulltest@example.com");
        var resultByNeither = await _unitOfWork.UserRepository.GetByUsernameOrEmailAsync(null, null);

        Assert.NotNull(resultByUsername);
        Assert.NotNull(resultByEmail);
        Assert.NotNull(resultByBoth);
        Assert.Null(resultByNeither);
    }

    [Theory]
    [InlineData("testuser1", "test1@example.com")]
    [InlineData("testuser2", "test2@example.com")]
    [InlineData("testuser3", "test3@example.com")]
    public async Task UserRepository_ShouldHandleMultipleUsers_WithUnitOfWork(string username, string email)
    {
        // Arrange
        var user = CreateValidUser(username, email);

        // Act
        await _unitOfWork.UserRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Assert
        var retrievedByUsername = await _unitOfWork.UserRepository.GetByUsernameAsync(username);
        var retrievedByEmail = await _unitOfWork.UserRepository.GetByEmailAsync(email);
        var retrievedById = await _unitOfWork.UserRepository.GetByIdAsync(user.Id);

        Assert.NotNull(retrievedByUsername);
        Assert.NotNull(retrievedByEmail);
        Assert.NotNull(retrievedById);

        Assert.Equal(user.Id, retrievedByUsername.Id);
        Assert.Equal(user.Id, retrievedByEmail.Id);
        Assert.Equal(user.Id, retrievedById.Id);
    }

    #endregion

    #region [ Helper Methods ]

    private static UserApplication CreateValidUser(string username, string email)
    {
        return new(
            username: username,
            email: email,
            passwordHash: "hashedpassword",
            passwordSalt: "salt"
        );
    }

    #endregion

    #region [ Dispose ]

    public void Dispose()
    {
        _unitOfWork?.Dispose();
        _context?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }

    #endregion
}