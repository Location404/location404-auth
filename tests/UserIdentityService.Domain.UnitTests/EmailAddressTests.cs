using UserIdentityService.Domain.ValueObjects;
using Shouldly;

namespace UserIdentityService.UnitTests;

public class EmailAddressTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("test.name@example.co.uk")]
    [InlineData("test_123@sub.domain.com")]
    public void Create_WithValidEmail_ShouldCreateInstance(string validEmail)
    {
        // Act
        var email = EmailAddress.Create(validEmail);

        // Assert
        email.ShouldNotBeNull();
        email.Value.ShouldBe(validEmail.ToLowerInvariant());
    }

    [Theory]
    [InlineData("plainaddress")]
    [InlineData("@missingusername.com")]
    [InlineData("test@.com")]
    [InlineData("test@domain")]
    [InlineData("")]
    [InlineData(" ")]
#pragma warning disable xUnit1012
    [InlineData(null)]
#pragma warning restore xUnit1012
    public void Create_WithInvalidEmail_ShouldThrowArgumentException(string invalidEmail)
    {
        // Act
        Action act = () => EmailAddress.Create(invalidEmail);

        // Assert
        Should.Throw<ArgumentException>(act);
    }
}