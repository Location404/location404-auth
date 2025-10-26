using Location404.Auth.Application.Common.Result;
using Location404.Auth.Domain.Entities;

namespace Location404.Auth.Application.Features.Authentication.Interfaces;

public interface ITokenService
{
    Result<string> GenerateAccessToken(Guid userId, string userName, IEnumerable<string> roles);
    Result<RefreshToken> IssueRefreshTokenAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result<(string accessToken, RefreshToken newRefresh)>> RotateAsync(Guid userId, string currentRefreshToken, CancellationToken cancellationToken);
    Task<Result> RevokeAsync(Guid userId, string refreshToken, CancellationToken cancellationToken);
}