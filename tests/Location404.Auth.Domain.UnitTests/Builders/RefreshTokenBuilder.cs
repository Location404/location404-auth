using Location404.Auth.Domain.Entities;

namespace Location404.Auth.Domain.UnitTests.Builders;

public class RefreshTokenBuilder
{
    private Guid _userId = Guid.NewGuid();
    private string _token = Guid.NewGuid().ToString();
    private DateTime _expiresAt = DateTime.UtcNow.AddDays(7);

    public RefreshTokenBuilder WithUserId(Guid userId)
    {
        _userId = userId;
        return this;
    }

    public RefreshTokenBuilder WithToken(string token)
    {
        _token = token;
        return this;
    }

    public RefreshTokenBuilder WithExpiresAt(DateTime expiresAt)
    {
        _expiresAt = expiresAt;
        return this;
    }

    public RefreshTokenBuilder ThatExpired()
    {
        _expiresAt = DateTime.UtcNow.AddDays(-1);
        return this;
    }

    public RefreshToken Build()
    {
        return RefreshToken.Create(_userId, _token, _expiresAt);
    }
}
