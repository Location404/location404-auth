using Shouldly;
using UserIdentityService.Domain.Entities;
using UserIdentityService.Domain.ValueObjects;

namespace UserIdentityService.UnitTests;

public class UserTests
{
    private readonly EmailAddress _validEmail = EmailAddress.Create("test@example.com");
    private const string ValidUsername = "testuser";

    [Fact]
    public void Create_WithPassword_ShouldInitializeUserCorrectly()
    {
        // Arrange
        var password = "StrongPassword123!";

        // Act
        var user = User.Create(_validEmail, ValidUsername, password);

        // Assert
        user.ShouldNotBeNull();
        user.Id.ShouldNotBe(Guid.Empty);
        user.Email.ShouldBe(_validEmail);
        user.Username.ShouldBe(ValidUsername);
        user.Password.ShouldBe(password);
        user.EmailVerified.ShouldBeFalse();
        user.IsActive.ShouldBeTrue();
        user.PreferredLanguage.ShouldBe("pt-BR");
        user.ExternalLogins.ShouldBeEmpty();
        user.CreatedAt.ShouldBe(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.UpdatedAt.ShouldBe(user.CreatedAt, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_WithExternalLogin_ShouldInitializeUserCorrectly()
    {
        // Arrange
        var loginProvider = "Google";
        var providerKey = "google-user-id-123";

        // Act
        var user = User.Create(_validEmail, ValidUsername, loginProvider, providerKey);

        // Assert
        user.ShouldNotBeNull();
        user.Password.ShouldBeNull();
        user.EmailVerified.ShouldBeTrue();
        user.IsActive.ShouldBeTrue();
        
        var login = user.ExternalLogins.ShouldHaveSingleItem();
        login.LoginProvider.ShouldBe(loginProvider);
        login.ProviderKey.ShouldBe(providerKey);
    }

    [Fact]
    public void UpdateProfile_WithValidData_ShouldUpdateProfileAndDate()
    {
        // Arrange
        var user = User.Create(_validEmail, ValidUsername, "password");
        var originalUpdateDate = user.UpdatedAt;
        var newUsername = "new_username";
        var newImageBytes = new byte[] { 1, 2, 3, 4, 5 };

        // Act
        user.UpdateProfile(newUsername, newImageBytes);

        // Assert
        user.Username.ShouldBe(newUsername);
        user.ProfileImage.ShouldBe(newImageBytes);
        user.UpdatedAt.ShouldBeGreaterThan(originalUpdateDate);
    }

    [Fact]
    public void VerifyEmail_WhenCalled_ShouldMarkEmailAsVerified()
    {
        // Arrange
        var user = User.Create(_validEmail, ValidUsername, "password");

        // Act
        user.VerifyEmail();

        // Assert
        user.EmailVerified.ShouldBeTrue();
    }

    [Fact]
    public void RecordLogin_WhenCalled_ShouldUpdateLastLoginDate()
    {
        // Arrange
        var user = User.Create(_validEmail, ValidUsername, "password");

        // Act
        user.RecordLogin();

        // Assert
        user.LastLoginAt.ShouldNotBeNull();
        user.LastLoginAt.Value.ShouldBe(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void DeactivateAndActivate_WhenCalled_ShouldChangeIsActiveStatus()
    {
        // Arrange
        var user = User.Create(_validEmail, ValidUsername, "password");

        // Act & Assert for Deactivate
        user.Deactivate();
        user.IsActive.ShouldBeFalse();

        // Act & Assert for Activate
        user.Activate();
        user.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void AddExternalLogin_WhenProviderDoesNotExist_ShouldAddLogin()
    {
        // Arrange
        var user = User.Create(_validEmail, ValidUsername, "password");
        var loginProvider = "Facebook";
        var providerKey = "fb-key-456";

        // Act
        user.AddExternalLogin(loginProvider, providerKey);

        // Assert
        var login = user.ExternalLogins.ShouldHaveSingleItem();
        login.LoginProvider.ShouldBe(loginProvider);
    }

    [Fact]
    public void AddExternalLogin_WhenProviderAlreadyExists_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var loginProvider = "Google";
        var user = User.Create(_validEmail, ValidUsername, loginProvider, "key1");

        // Act
        Action act = () => user.AddExternalLogin(loginProvider, "key2");

        // Assert
        var exception = Should.Throw<InvalidOperationException>(act);
        // A mensagem da exceção deve ser idêntica à do código-fonte, por isso permanece em português.
        exception.Message.ShouldBe($"Um login do provedor '{loginProvider}' já está associado a este usuário.");
    }
}
