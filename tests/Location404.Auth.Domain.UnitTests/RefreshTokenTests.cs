using FluentAssertions;
using Location404.Auth.Domain.Entities;
using Location404.Auth.Domain.UnitTests.Builders;

namespace Location404.Auth.Domain.UnitTests;

public class RefreshTokenTests
{
    [Fact]
    public void Create_ShouldInitializeRefreshTokenCorrectly()
    {
        var userId = Guid.NewGuid();
        var token = "refresh-token-123";
        var expiresAt = DateTime.UtcNow.AddDays(7);

        var refreshToken = RefreshToken.Create(userId, token, expiresAt);

        refreshToken.Should().NotBeNull();
        refreshToken.Id.Should().NotBe(Guid.Empty);
        refreshToken.UserId.Should().Be(userId);
        refreshToken.Token.Should().Be(token);
        refreshToken.ExpiresAtUtc.Should().Be(expiresAt);
        refreshToken.RevokedAtUtc.Should().BeNull();
        refreshToken.IsRevoked.Should().BeFalse();
    }

    [Fact]
    public void IsExpired_WhenBeforeExpiryDate_ShouldReturnFalse()
    {
        var refreshToken = new RefreshTokenBuilder()
            .WithExpiresAt(DateTime.UtcNow.AddDays(1))
            .Build();

        var clock = DateTime.UtcNow;

        refreshToken.IsExpired(clock).Should().BeFalse();
    }

    [Fact]
    public void IsExpired_WhenAfterExpiryDate_ShouldReturnTrue()
    {
        var refreshToken = new RefreshTokenBuilder()
            .WithExpiresAt(DateTime.UtcNow.AddDays(-1))
            .Build();

        var clock = DateTime.UtcNow;

        refreshToken.IsExpired(clock).Should().BeTrue();
    }

    [Fact]
    public void IsExpired_WhenExactlyAtExpiryDate_ShouldReturnTrue()
    {
        var expiryDate = DateTime.UtcNow;
        var refreshToken = new RefreshTokenBuilder()
            .WithExpiresAt(expiryDate)
            .Build();

        refreshToken.IsExpired(expiryDate).Should().BeTrue();
    }

    [Fact]
    public void IsRevoked_WhenNotRevoked_ShouldReturnFalse()
    {
        var refreshToken = new RefreshTokenBuilder().Build();

        refreshToken.IsRevoked.Should().BeFalse();
    }

    [Fact]
    public void IsRevoked_WhenRevoked_ShouldReturnTrue()
    {
        var refreshToken = new RefreshTokenBuilder().Build();

        refreshToken.Revoke();

        refreshToken.IsRevoked.Should().BeTrue();
    }

    [Fact]
    public void Revoke_WhenNotRevoked_ShouldSetRevokedAtUtc()
    {
        var refreshToken = new RefreshTokenBuilder().Build();

        refreshToken.Revoke();

        refreshToken.RevokedAtUtc.Should().NotBeNull();
        refreshToken.RevokedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Revoke_WhenAlreadyRevoked_ShouldNotChangeRevokedDate()
    {
        var refreshToken = new RefreshTokenBuilder().Build();
        refreshToken.Revoke();
        var firstRevokedDate = refreshToken.RevokedAtUtc;

        Thread.Sleep(100);
        refreshToken.Revoke();

        refreshToken.RevokedAtUtc.Should().Be(firstRevokedDate);
    }

    [Fact]
    public void IsActive_WhenNotRevokedAndNotExpired_ShouldReturnTrue()
    {
        var refreshToken = new RefreshTokenBuilder()
            .WithExpiresAt(DateTime.UtcNow.AddDays(1))
            .Build();

        var clock = DateTime.UtcNow;

        refreshToken.IsActive(clock).Should().BeTrue();
    }

    [Fact]
    public void IsActive_WhenRevoked_ShouldReturnFalse()
    {
        var refreshToken = new RefreshTokenBuilder()
            .WithExpiresAt(DateTime.UtcNow.AddDays(1))
            .Build();
        refreshToken.Revoke();

        var clock = DateTime.UtcNow;

        refreshToken.IsActive(clock).Should().BeFalse();
    }

    [Fact]
    public void IsActive_WhenExpired_ShouldReturnFalse()
    {
        var refreshToken = new RefreshTokenBuilder()
            .ThatExpired()
            .Build();

        var clock = DateTime.UtcNow;

        refreshToken.IsActive(clock).Should().BeFalse();
    }

    [Fact]
    public void IsActive_WhenRevokedAndExpired_ShouldReturnFalse()
    {
        var refreshToken = new RefreshTokenBuilder()
            .ThatExpired()
            .Build();
        refreshToken.Revoke();

        var clock = DateTime.UtcNow;

        refreshToken.IsActive(clock).Should().BeFalse();
    }
}
