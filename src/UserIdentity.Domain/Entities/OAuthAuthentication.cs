using UserIdentity.Domain.Common;

namespace UserIdentity.Domain.Entities;

public class OAuthAuthentication(string provider, string providerId, string refreshToken, DateTime expirationDate) : BaseEntity
{
    public virtual UserApplication UserApplication { get; private set; } = null!;
    public Guid UserApplicationId { get; private set; }

    public string Provider { get; private set; } = provider;
    public string ProviderId { get; private set; } = providerId;

    public string RefreshToken { get; private set; } = refreshToken;
    public DateTime ExpirationDate { get; private set; } = expirationDate;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime LastLoginAt { get; private set; } = DateTime.UtcNow;

    public bool IsExpired => DateTime.UtcNow > ExpirationDate;
    public void UpdateLastLogin() => LastLoginAt = DateTime.UtcNow;

    public void SetUserApplication(UserApplication userApplication)
    {
        UserApplication = userApplication;
    }
}