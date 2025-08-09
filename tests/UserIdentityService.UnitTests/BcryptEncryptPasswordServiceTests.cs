using Shouldly;
using UserIdentityService.Infrastructure.Services;
namespace UserIdentityService.UnitTests;


public class BcryptEncryptPasswordServiceTests
{
    private readonly BcryptEncryptPasswordService _passwordService;

    public BcryptEncryptPasswordServiceTests()
    {
        _passwordService = new BcryptEncryptPasswordService();
    }

    [Fact]
    public void Encrypt_WithValidPassword_ShouldReturnHashDifferentFromPassword()
    {
        // Arrange
        var password = "mysecretpassword";

        // Act
        var encryptedPassword = _passwordService.Encrypt(password);

        // Assert
        encryptedPassword.ShouldNotBeNullOrEmpty();
        encryptedPassword.ShouldNotBe(password);
    }



    [Theory]
#pragma warning disable xUnit1012
    [InlineData(null)]
#pragma warning restore xUnit1012
    [InlineData("")]
    [InlineData(" ")]
    public void Encrypt_WithInvalidPassword_ShouldThrowArgumentException(string invalidPassword)
    {
        // Act
        Action act = () => _passwordService.Encrypt(invalidPassword);

        // Assert
        Should.Throw<ArgumentException>(act);
    }

    [Fact]
    public void Verify_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "mysecretpassword";
        var encryptedPassword = _passwordService.Encrypt(password);

        // Act
        var result = _passwordService.Verify(password, encryptedPassword);

        // Assert
        result.ShouldBeTrue();
    }



    [Fact]
    public void Verify_WithIncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var originalPassword = "mysecretpassword";
        var wrongPassword = "anotherpassword";
        var encryptedPassword = _passwordService.Encrypt(originalPassword);

        // Act
        var result = _passwordService.Verify(wrongPassword, encryptedPassword);

        // Assert
        result.ShouldBeFalse();
    }
}