using FluentAssertions;
using Location404.Auth.Infrastructure.Services;

namespace Location404.Auth.Application.UnitTests.Features.Infrastructure;

public class BcryptEncryptPasswordServiceTests
{
    private readonly BcryptEncryptPasswordService _service;

    public BcryptEncryptPasswordServiceTests()
    {
        _service = new BcryptEncryptPasswordService();
    }

    [Fact]
    public void Encrypt_WithValidPassword_ShouldReturnHashedPassword()
    {
        var password = "MySecurePassword123!";

        var hashedPassword = _service.Encrypt(password);

        hashedPassword.Should().NotBeNullOrWhiteSpace();
        hashedPassword.Should().NotBe(password);
        hashedPassword.Should().StartWith("$2");
    }

    [Fact]
    public void Encrypt_WithNullPassword_ShouldThrowArgumentException()
    {
        string? password = null;

        var act = () => _service.Encrypt(password!);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("password");
    }

    [Fact]
    public void Encrypt_WithEmptyPassword_ShouldThrowArgumentException()
    {
        var password = string.Empty;

        var act = () => _service.Encrypt(password);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("password");
    }

    [Fact]
    public void Encrypt_WithWhitespacePassword_ShouldThrowArgumentException()
    {
        var password = "   ";

        var act = () => _service.Encrypt(password);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("password");
    }

    [Fact]
    public void Verify_WithCorrectPassword_ShouldReturnTrue()
    {
        var password = "MySecurePassword123!";
        var hashedPassword = _service.Encrypt(password);

        var result = _service.Verify(password, hashedPassword);

        result.Should().BeTrue();
    }

    [Fact]
    public void Verify_WithIncorrectPassword_ShouldReturnFalse()
    {
        var password = "MySecurePassword123!";
        var wrongPassword = "WrongPassword123!";
        var hashedPassword = _service.Encrypt(password);

        var result = _service.Verify(wrongPassword, hashedPassword);

        result.Should().BeFalse();
    }

    [Fact]
    public void Verify_WithNullPassword_ShouldThrowArgumentException()
    {
        string? password = null;
        var hashedPassword = _service.Encrypt("SomePassword123!");

        var act = () => _service.Verify(password!, hashedPassword);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("password");
    }

    [Fact]
    public void Verify_WithEmptyPassword_ShouldThrowArgumentException()
    {
        var password = string.Empty;
        var hashedPassword = _service.Encrypt("SomePassword123!");

        var act = () => _service.Verify(password, hashedPassword);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("password");
    }

    [Fact]
    public void Verify_WithWhitespacePassword_ShouldThrowArgumentException()
    {
        var password = "   ";
        var hashedPassword = _service.Encrypt("SomePassword123!");

        var act = () => _service.Verify(password, hashedPassword);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("password");
    }

    [Fact]
    public void Verify_WithNullEncryptedPassword_ShouldThrowArgumentException()
    {
        var password = "MySecurePassword123!";
        string? encryptedPassword = null;

        var act = () => _service.Verify(password, encryptedPassword!);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("encryptedPassword");
    }

    [Fact]
    public void Verify_WithEmptyEncryptedPassword_ShouldThrowArgumentException()
    {
        var password = "MySecurePassword123!";
        var encryptedPassword = string.Empty;

        var act = () => _service.Verify(password, encryptedPassword);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("encryptedPassword");
    }

    [Fact]
    public void Verify_WithWhitespaceEncryptedPassword_ShouldThrowArgumentException()
    {
        var password = "MySecurePassword123!";
        var encryptedPassword = "   ";

        var act = () => _service.Verify(password, encryptedPassword);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("encryptedPassword");
    }

    [Fact]
    public void Encrypt_SamePlainTextPassword_ShouldGenerateDifferentHashes()
    {
        var password = "MySecurePassword123!";

        var hash1 = _service.Encrypt(password);
        var hash2 = _service.Encrypt(password);

        hash1.Should().NotBe(hash2);

        _service.Verify(password, hash1).Should().BeTrue();
        _service.Verify(password, hash2).Should().BeTrue();
    }
}
