using UserIdentityService.Application.Common.Result;
using UserIdentityService.Domain.Entities;

namespace UserIdentityService.Application.Features.Authentication.Interfaces;

public interface ITokenService
{
    Result<string> GenerateAccessToken(Guid userId, string userName, IEnumerable<string> roles);
    Result<RefreshToken> IssueRefreshTokenAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result<(string accessToken, RefreshToken newRefresh)>> RotateAsync(Guid userId, string currentRefreshToken, CancellationToken cancellationToken);
    Task<Result> RevokeAsync(Guid userId, string refreshToken, CancellationToken cancellationToken);
}