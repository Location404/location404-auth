using FluentAssertions;
using Location404.Auth.Domain.Entities;

namespace Location404.Auth.Domain.UnitTests;

public class ExternalLoginTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateExternalLogin()
    {
        var userId = Guid.NewGuid();
        var loginProvider = "Google";
        var providerKey = "google-user-123";

        var externalLogin = new ExternalLogin(userId, loginProvider, providerKey);

        externalLogin.UserId.Should().Be(userId);
        externalLogin.LoginProvider.Should().Be(loginProvider);
        externalLogin.ProviderKey.Should().Be(providerKey);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidLoginProvider_ShouldThrowArgumentException(string? invalidProvider)
    {
        var userId = Guid.NewGuid();
        var providerKey = "key-123";

        var act = () => new ExternalLogin(userId, invalidProvider!, providerKey);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*loginProvider*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidProviderKey_ShouldThrowArgumentException(string? invalidKey)
    {
        var userId = Guid.NewGuid();
        var loginProvider = "Google";

        var act = () => new ExternalLogin(userId, loginProvider, invalidKey!);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*providerKey*");
    }

    [Fact]
    public void Constructor_WithDifferentProviders_ShouldCreateSeparateLogins()
    {
        var userId = Guid.NewGuid();
        var googleLogin = new ExternalLogin(userId, "Google", "google-key");
        var facebookLogin = new ExternalLogin(userId, "Facebook", "facebook-key");

        googleLogin.LoginProvider.Should().Be("Google");
        facebookLogin.LoginProvider.Should().Be("Facebook");
        googleLogin.ProviderKey.Should().NotBe(facebookLogin.ProviderKey);
    }
}
