namespace UserIdentityService.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = default!;
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public bool IsRevoked => RevokedAtUtc.HasValue;
    public bool IsExpired(DateTime clock) => clock >= ExpiresAtUtc;
    public bool IsActive(DateTime clock) => !IsRevoked && !IsExpired(clock);

    public virtual User User { get; private set; } = default!;

    private RefreshToken() { }

    private RefreshToken(Guid userId, string token, DateTime expiresAtUtc)
    {
        UserId = userId;
        Token = token;
        ExpiresAtUtc = expiresAtUtc;
    }

    public static RefreshToken Create(Guid userId, string token, DateTime expiresAtUtc)
        => new(userId, token, expiresAtUtc);

    public void Revoke()
    {
        if (IsRevoked) return;
        RevokedAtUtc = DateTime.UtcNow;
    }
}

