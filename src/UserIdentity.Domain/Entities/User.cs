using UserIdentity.Domain.Common;
using UserIdentity.Domain.Validations;
using UserIdentity.Domain.ValueObject;

namespace UserIdentity.Domain.Entities;

public sealed class User : BaseEntity
{
    public string UserName { get; private set; }
    public string EmailAddress { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime? LastLogin { get; private set; }
    public bool IsActive { get; private set; } = true;

    public User(string userName, string emailAddress, string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName, nameof(UserName));
        ArgumentOutOfRangeException.ThrowIfLessThan(userName.Length, 3, nameof(UserName));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(userName.Length, 12, nameof(UserName));
        ArgumentValidationExtensions.ThrowIfContainsInvalidCharacters(userName, nameof(UserName), ' ', '.', '-', '_');

        EmailAddress = new EmailAddress(emailAddress).Value;
        UserName = userName.Trim();
        PasswordHash = passwordHash;
    }

    public void UpdateLastLogin() => LastLogin = DateTime.UtcNow;
    public void UpdateEmail(string emailAddress)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(emailAddress, nameof(EmailAddress));
        EmailAddress = new EmailAddress(emailAddress).Value;
    }

    public void UpdatePassword(string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash, nameof(PasswordHash));
        PasswordHash = passwordHash;
    }

    public void UpdateUserName(string userName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName, nameof(UserName));
        ArgumentOutOfRangeException.ThrowIfLessThan(userName.Length, 3, nameof(UserName));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(userName.Length, 12, nameof(UserName));
        ArgumentValidationExtensions.ThrowIfContainsInvalidCharacters(userName, nameof(UserName), ' ', '.', '-', '_');

        UserName = userName.Trim();
    }
}