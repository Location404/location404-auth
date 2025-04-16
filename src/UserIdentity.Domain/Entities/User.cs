using UserIdentity.Domain.Common;
using UserIdentity.Domain.ValueObject;

namespace UserIdentity.Domain.Entities;

public class User(string username, string emailAddress, string displayName, string passwordHash, string passwordSalt) : BaseEntity
{
    #region [Properties]

    public string Username { get; private set; } = username;
    public string EmailAddress { get; private set; } = new EmailAddress(emailAddress).Value;
    public string PasswordHash { get; private set; } = passwordHash;
    public string PasswordSalt { get; private set; } = passwordSalt;
    public string DisplayName { get; private set; } = displayName;
    public string? ProfilePictureUrl { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string PreferredLanguage { get; private set; } = "pt-BR";
    public int FailedLoginAttempts { get; private set; } = 0;
    public DateTime? LockoutEndDate { get; private set; }
    public string RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }
    // public ICollection<Role> UserRoles { get; private set; } = [];

    #endregion

    #region [Public Methods]

    public void UpdateDisplayName(string displayName) => DisplayName = displayName;

    public void UpdateEmail(string email) => EmailAddress = new EmailAddress(email).Value;

    public void UpdateProfilePicture(string url) => ProfilePictureUrl = url;

    public void SetPreferredLanguage(string language) => PreferredLanguage = language;

    public void UpdatePassword(string passwordHash, string passwordSalt)
    {
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
    }

    public void SetRefreshToken(string refreshToken, DateTime expiryTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = expiryTime;
    }

    public void RecordSuccessfulLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        FailedLoginAttempts = 0;
        LockoutEndDate = null;
    }

    public void RecordFailedLogin(int maxAttempts, TimeSpan lockoutDuration)
    {
        FailedLoginAttempts++;
        if (FailedLoginAttempts >= maxAttempts)
        {
            LockoutEndDate = DateTime.UtcNow.Add(lockoutDuration);
        }
    }

    public bool IsLockedOut() => LockoutEndDate.HasValue && DateTime.UtcNow <= LockoutEndDate.Value;

    public void DeactivateAccount() => IsActive = false;

    public void ReactivateAccount()
    {
        IsActive = true;
        FailedLoginAttempts = 0;
        LockoutEndDate = null;
    }

    // public void RemoveRole(params List<Role> roles)
    // {
    //     roles.ForEach(role =>
    //     {
    //         if (!UserRoles.Any(ur => ur.Id == role.Id))
    //         {
    //             UserRoles.Remove(role);
    //         }
    //     });
    // }

    // public void AddRole(params List<Role> roles)
    // {
    //     roles.ForEach(role =>
    //     {
    //         if (!UserRoles.Any(ur => ur.Id == role.Id))
    //         {
    //             UserRoles.Add(role);
    //         }
    //     });
    // }

    #endregion
}