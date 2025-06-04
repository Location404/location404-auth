namespace UserIdentity.UnitTests.Services;

using UserIdentity.Application.Features.Authentication.Interfaces;
using UserIdentity.Infra.Services;

using Xunit;

public class PasswordHasherTests
{
    private readonly IPasswordHasher _passwordHasher = new PasswordHasher();

    [Fact]
    public void CreatePasswordHash_ValidPassword_ReturnsNonEmptyHashAndSalt()
    {
        // Arrange
        string password = "Test@123";

        // Act
        var (hash, salt) = _passwordHasher.CreatePasswordHash(password);

        // Assert
        Assert.False(string.IsNullOrEmpty(hash), "O hash não deve ser vazio.");
        Assert.False(string.IsNullOrEmpty(salt), "O salt não deve ser vazio.");
        Assert.NotEqual(hash, password);
    }

    [Fact]
    public void CreatePasswordHash_SamePassword_DifferentHashAndSalt()
    {
        // Arrange
        string password = "Test@123";

        // Act
        var (hash1, salt1) = _passwordHasher.CreatePasswordHash(password);
        var (hash2, salt2) = _passwordHasher.CreatePasswordHash(password);

        // Assert
        Assert.NotEqual(hash1, hash2);
        Assert.NotEqual(salt1, salt2!);
    }

    [Fact]
    public void CreatePasswordHash_NullPassword_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _passwordHasher.CreatePasswordHash(null!));
    }

    [Fact]
    public void VerifyPasswordHash_ValidPasswordAndHash_ReturnsTrue()
    {
        // Arrange
        string password = "Test@123";
        var (hash, salt) = _passwordHasher.CreatePasswordHash(password);

        // Act
        bool isValid = _passwordHasher.VerifyPasswordHash(password, hash, salt);

        // Assert
        Assert.True(isValid, "A verificação deve retornar true para a senha correta.");
    }

    [Fact]
    public void VerifyPasswordHash_IncorrectPassword_ReturnsFalse()
    {
        // Arrange
        string password = "Test@123";
        string wrongPassword = "Wrong@123";
        var (hash, salt) = _passwordHasher.CreatePasswordHash(password);

        // Act
        bool isValid = _passwordHasher.VerifyPasswordHash(wrongPassword, hash, salt);

        // Assert
        Assert.False(isValid, "A verificação deve retornar false para a senha incorreta.");
    }

    [Fact]
    public void VerifyPasswordHash_NullPassword_ThrowsArgumentNullException()
    {
        // Arrange
        string password = "Test@123";
        var (hash, salt) = _passwordHasher.CreatePasswordHash(password);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _passwordHasher.VerifyPasswordHash(null!, hash, salt));
    }

    [Fact]
    public void VerifyPasswordHash_NullHash_ThrowsArgumentNullException()
    {
        // Arrange
        string password = "Test@123";
        var (_, salt) = _passwordHasher.CreatePasswordHash(password);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _passwordHasher.VerifyPasswordHash(password, null!, salt));
    }

    [Fact]
    public void VerifyPasswordHash_NullSalt_ThrowsArgumentNullException()
    {
        // Arrange
        string password = "Test@123";
        var (hash, _) = _passwordHasher.CreatePasswordHash(password);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _passwordHasher.VerifyPasswordHash(password, hash, null!));
    }
}