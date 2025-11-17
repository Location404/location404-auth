using Shouldly;
using Location404.Auth.Domain.Entities;
using Location404.Auth.Domain.ValueObjects;

namespace Location404.Auth.Domain.UnitTests;

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
        void Act() => user.AddExternalLogin(loginProvider, "key2");

        // Assert
        var exception = Should.Throw<InvalidOperationException>(Act);
    }

    [Fact]
    public void UpdateProfile_WithNewUsername_ShouldUpdateUsernameAndUpdatedAt()
    {
        var user = User.Create(_validEmail, ValidUsername, "password");
        var originalUpdatedAt = user.UpdatedAt;
        Thread.Sleep(10);

        user.UpdateProfile("newusername");

        user.Username.ShouldBe("newusername");
        user.UpdatedAt.ShouldBeGreaterThan(originalUpdatedAt);
    }

    [Fact]
    public void UpdateProfile_WithNewEmail_ShouldUpdateEmailAndUpdatedAt()
    {
        var user = User.Create(_validEmail, ValidUsername, "password");
        var newEmail = EmailAddress.Create("newemail@example.com");

        user.UpdateProfile(null, newEmail);

        user.Email.ShouldBe(newEmail);
    }

    [Fact]
    public void UpdateProfile_WithNewPassword_ShouldUpdatePasswordAndUpdatedAt()
    {
        var user = User.Create(_validEmail, ValidUsername, "oldpassword");
        var newPassword = "newpassword123";

        user.UpdateProfile(ValidUsername, password: newPassword);

        user.Password.ShouldBe(newPassword);
    }

    [Fact]
    public void UpdateProfile_WithNewProfileImage_ShouldUpdateImageAndUpdatedAt()
    {
        var user = User.Create(_validEmail, ValidUsername, "password");
        var profileImage = new byte[] { 1, 2, 3, 4, 5 };

        user.UpdateProfile(null, profileImage: profileImage);

        user.ProfileImage.ShouldBe(profileImage);
    }

    [Fact]
    public void UpdateProfile_WithAllNullValues_ShouldNotUpdateFields()
    {
        var user = User.Create(_validEmail, ValidUsername, "password");
        var originalUsername = user.Username;
        var originalEmail = user.Email;
        var originalUpdatedAt = user.UpdatedAt;

        user.UpdateProfile(null, null, null, null);

        user.Username.ShouldBe(originalUsername);
        user.Email.ShouldBe(originalEmail);
        user.UpdatedAt.ShouldBe(originalUpdatedAt);
    }

    [Fact]
    public void ChangePreferredLanguage_WithValidLanguage_ShouldUpdateLanguage()
    {
        var user = User.Create(_validEmail, ValidUsername, "password");

        user.ChangePreferredLanguage("en-US");

        user.PreferredLanguage.ShouldBe("en-US");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ChangePreferredLanguage_WithInvalidLanguage_ShouldThrowArgumentException(string? invalidLanguage)
    {
        var user = User.Create(_validEmail, ValidUsername, "password");

        void Act() => user.ChangePreferredLanguage(invalidLanguage!);

        Should.Throw<ArgumentException>(Act);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidUsername_ShouldThrowArgumentException(string invalidUsername)
    {
        void Act() => User.Create(_validEmail, invalidUsername, "password");

        Should.Throw<ArgumentException>(Act);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidPassword_ShouldThrowArgumentException(string invalidPassword)
    {
        void Act() => User.Create(_validEmail, ValidUsername, invalidPassword);

        Should.Throw<ArgumentException>(Act);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithExternalLogin_WithInvalidProvider_ShouldThrowArgumentException(string invalidProvider)
    {
        void Act() => User.Create(_validEmail, ValidUsername, invalidProvider, "key");

        Should.Throw<ArgumentException>(Act);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithExternalLogin_WithInvalidProviderKey_ShouldThrowArgumentException(string invalidKey)
    {
        void Act() => User.Create(_validEmail, ValidUsername, "Google", invalidKey);

        Should.Throw<ArgumentException>(Act);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithExternalLogin_WithInvalidUsername_ShouldThrowArgumentException(string invalidUsername)
    {
        void Act() => User.Create(_validEmail, invalidUsername, "Google", "key");

        Should.Throw<ArgumentException>(Act);
    }
}