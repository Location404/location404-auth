using UserIdentity.Domain.Common;

namespace UserIdentity.Domain.Entities;

public class PasswordAuthentication(string passwordHash, string passwordSalt, string refreshToken, DateTime refreshTokenExpirationDate) : BaseEntity
{
    public virtual UserApplication UserApplication { get; private set; } = null!;

    public string PasswordHash { get; private set; } = passwordHash;
    public string PasswordSalt { get; private set; } = passwordSalt;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime LastLoginAt { get; private set; } = DateTime.UtcNow;

    public string RefreshToken { get; private set; } = refreshToken;
    public DateTime RefreshTokenExpirationDate { get; private set; } = refreshTokenExpirationDate;

    public bool IsExpired() => DateTime.UtcNow > RefreshTokenExpirationDate;
    public void UpdateLastLogin() => LastLoginAt = DateTime.UtcNow;
}
