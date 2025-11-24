using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Location404.Auth.Application.Common.Interfaces;
using Location404.Auth.Infrastructure.Services;
using Location404.Auth.Infrastructure.Settings;
using System.IdentityModel.Tokens.Jwt;

namespace Location404.Auth.Application.UnitTests.Features.Infrastructure;

public class TokenServiceTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly TokenService _tokenService;
    private readonly JwtSettings _jwtSettings;

    public TokenServiceTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _jwtSettings = new JwtSettings
        {
            Issuer = "test-issuer",
            Audience = "test-audience",
            SigningKey = "this-is-a-very-long-secret-key-that-must-be-at-least-32-characters-long-for-hmac-sha256",
            AccessTokenMinutes = 15,
            RefreshTokenDays = 7
        };

        var options = Substitute.For<IOptions<JwtSettings>>();
        options.Value.Returns(_jwtSettings);

        _tokenService = new TokenService(options, _unitOfWork);
    }

    [Fact]
    public void GenerateAccessToken_WithValidData_ShouldReturnToken()
    {
        var userId = Guid.NewGuid();
        var userName = "testuser";
        var roles = new[] { "User", "Admin" };

        var result = _tokenService.GenerateAccessToken(userId, userName, roles);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GenerateAccessToken_ShouldGenerateValidJwtToken()
    {
        var userId = Guid.NewGuid();
        var userName = "testuser";
        var roles = new[] { "User" };

        var result = _tokenService.GenerateAccessToken(userId, userName, roles);

        result.IsSuccess.Should().BeTrue();

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(result.Value);

        token.Should().NotBeNull();
        token.Issuer.Should().Be(_jwtSettings.Issuer);
        token.Audiences.Should().Contain(_jwtSettings.Audience);
    }

    [Fact]
    public void GenerateAccessToken_ShouldIncludeUserIdInClaims()
    {
        var userId = Guid.NewGuid();
        var userName = "testuser";
        var roles = new[] { "User" };

        var result = _tokenService.GenerateAccessToken(userId, userName, roles);

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(result.Value);

        var subClaim = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
        subClaim.Should().NotBeNull();
        subClaim!.Value.Should().Be(userId.ToString());
    }

    [Fact]
    public void GenerateAccessToken_ShouldIncludeUserNameInClaims()
    {
        var userId = Guid.NewGuid();
        var userName = "testuser";
        var roles = new[] { "User" };

        var result = _tokenService.GenerateAccessToken(userId, userName, roles);

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(result.Value);

        var userNameClaim = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName);
        userNameClaim.Should().NotBeNull();
        userNameClaim!.Value.Should().Be(userName);
    }

    [Fact]
    public void GenerateAccessToken_ShouldIncludeRolesInClaims()
    {
        var userId = Guid.NewGuid();
        var userName = "testuser";
        var roles = new[] { "User", "Admin" };

        var result = _tokenService.GenerateAccessToken(userId, userName, roles);

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(result.Value);

        var roleClaims = token.Claims.Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").ToList();
        roleClaims.Should().HaveCount(2);
        roleClaims.Select(c => c.Value).Should().Contain(new[] { "User", "Admin" });
    }

    [Fact]
    public void GenerateAccessToken_WithEmptyRoles_ShouldNotIncludeRoleClaims()
    {
        var userId = Guid.NewGuid();
        var userName = "testuser";
        var roles = Array.Empty<string>();

        var result = _tokenService.GenerateAccessToken(userId, userName, roles);

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(result.Value);

        var roleClaims = token.Claims.Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").ToList();
        roleClaims.Should().BeEmpty();
    }

    [Fact]
    public void IssueRefreshTokenAsync_ShouldReturnRefreshToken()
    {
        var userId = Guid.NewGuid();

        var result = _tokenService.IssueRefreshTokenAsync(userId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.UserId.Should().Be(userId);
        result.Value.Token.Should().NotBeNullOrWhiteSpace();
        result.Value.ExpiresAtUtc.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void IssueRefreshTokenAsync_ShouldGenerateSecureRandomToken()
    {
        var userId = Guid.NewGuid();

        var result1 = _tokenService.IssueRefreshTokenAsync(userId, CancellationToken.None);
        var result2 = _tokenService.IssueRefreshTokenAsync(userId, CancellationToken.None);

        result1.Value.Token.Should().NotBe(result2.Value.Token);
        result1.Value.Token.Length.Should().BeGreaterThan(50);
    }
}
