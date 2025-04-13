using UserIdentity.Domain.Common;
using UserIdentity.Domain.Validations;
using UserIdentity.Domain.ValueObject;

namespace UserIdentity.Domain.Entities;

public sealed class User : BaseEntity
{
    public string UserName { get; private set; }
    public string EmailAddress { get; private set; }
    public string PasswordHash { get; private set; }

    public User(string userName, string emailAddress, string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName, nameof(UserName));
        ArgumentOutOfRangeException.ThrowIfLessThan(userName.Length, 3, nameof(UserName));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(userName.Length, 12, nameof(UserName));
        ArgumentValidationExtensions.ThrowIfContainsInvalidCharacters(userName, nameof(UserName), ' ', '.', '-', '_');

        EmailAddress = new EmailAddress(emailAddress).Value;
        UserName = userName;
        PasswordHash = passwordHash;
    }
}