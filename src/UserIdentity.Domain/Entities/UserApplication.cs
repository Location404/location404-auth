using UserIdentity.Domain.Common;
using UserIdentity.Domain.ValueObject;

namespace UserIdentity.Domain.Entities;

public class UserApplication(string username, string emailAddress, string passwordHash, string passwordSalt) : BaseEntity
{
    public string DisplayName { get; private set; } = username;
    public string Username { get; private set; } = username;
    public string EmailAddress { get; private set; } = new EmailAddress(emailAddress).Value;
    public string? ProfilePictureUrl { get; private set; }

    public string PreferredLanguage { get; private set; } = "pt-BR";

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    public string PasswordHash { get; private set; } = passwordHash;
    public string PasswordSalt { get; private set; } = passwordSalt;
    
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }

    public bool ExternalLogin { get; private set; } = false;
    public string? ExternalProviderId { get; private set; }
    public string? ExternalProvider { get; private set; }

    public void UpdateProfile(string displayName, string? profilePictureUrl = null)
    {
        DisplayName = displayName;
        if (profilePictureUrl != null) ProfilePictureUrl = profilePictureUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRefreshToken(string refreshToken, DateTime expiryTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = expiryTime;
    }

    public void SetExternalLogin(string provider, string externalId)
    {
        ExternalLogin = true;
        ExternalProvider = provider;
        ExternalProviderId = externalId;
    }
}