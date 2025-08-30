using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using UserIdentityService.Application.Common.Interfaces;
using UserIdentityService.Application.Common.Result;
using UserIdentityService.Application.Features.Authentication.Interfaces;
using UserIdentityService.Domain.Entities;
using UserIdentityService.Infrastructure.Settings;

namespace UserIdentityService.Infrastructure.Services;

public sealed class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IUnitOfWork _uow;
    private readonly SigningCredentials _creds;

    public TokenService(IOptions<JwtSettings> jwtSettings, IUnitOfWork unitOfWork)
    {
        _jwtSettings = jwtSettings.Value;
        _uow = unitOfWork;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SigningKey));
        _creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    public Result<string> GenerateAccessToken(Guid userId, string userName, IEnumerable<string> roles)
    {
        var now = DateTime.UtcNow;
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, userName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        var jwt = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(_jwtSettings.AccessTokenMinutes),
            signingCredentials: _creds
        );

        return Result.Success(new JwtSecurityTokenHandler().WriteToken(jwt));
    }

    public Result<RefreshToken> IssueRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        var refresh = RefreshToken.Create(
            userId: userId,
            token: GenerateSecureRandomString(64),
            expiresAtUtc: DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays));

        _uow.Users.AddRefreshToken(refresh, cancellationToken);
        return Result.Success(refresh);
    }

    public async Task<Result<(string accessToken, RefreshToken newRefresh)>> RotateAsync(Guid userId, string currentRefreshToken, CancellationToken cancellationToken)
    {
        var existing = await _uow.Users.GetByTokenAsync(userId, currentRefreshToken, cancellationToken);

        if (existing == null)
        {
            return Result<(string accessToken, RefreshToken newRefresh)>.Failure(new Error(
                "InvalidRefreshToken",
                "The provided refresh token is invalid.",
                ErrorType.Validation));
        }

        if (!existing.IsActive(DateTime.UtcNow))
        {
            return Result<(string accessToken, RefreshToken newRefresh)>.Failure(new Error(
                "InvalidRefreshToken",
                "The provided refresh token is no longer active.",
                ErrorType.Validation));

        }

        existing.Revoke();
        var newRefresh = IssueRefreshTokenAsync(existing.UserId, cancellationToken);
        var access = GenerateAccessToken(existing.UserId, userName: existing.UserId.ToString(), roles: []);

        return Result.Success((access.Value, newRefresh.Value));
    }

    public async Task<Result> RevokeAsync(Guid userId, string refreshToken, CancellationToken cancellationToken)
    {
        var existing = await _uow.Users.GetByTokenAsync(userId, refreshToken, cancellationToken);

        if (existing == null)
        {
            return Result.Failure(new Error(
                "RefreshTokenNotFound",
                "O refresh token fornecido não foi encontrado.",
                ErrorType.Validation));
        }

        existing.Revoke();
        return Result.Success();
    }

    private static string GenerateSecureRandomString(int bytes)
    {
        var buffer = new byte[bytes];
        RandomNumberGenerator.Fill(buffer);
        return Convert.ToBase64String(buffer);
    }
}