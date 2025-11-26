using LiteBus.Queries.Abstractions;
using Location404.Auth.Application.Common.Interfaces;
using Location404.Auth.Application.Common.Result;
using Microsoft.Extensions.Logging;

namespace Location404.Auth.Application.Features.UserManagement.Queries.GetUsersProfiles;

public class GetUsersProfilesQueryHandler(
    IUnitOfWork uow,
    ILogger<GetUsersProfilesQueryHandler> logger) : IQueryHandler<GetUsersProfilesQuery, Result<List<UserProfileResponse>>>
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<GetUsersProfilesQueryHandler> _logger = logger;

    public async Task<Result<List<UserProfileResponse>>> HandleAsync(GetUsersProfilesQuery query, CancellationToken cancellationToken = default)
    {
        if (query.UserIds == null || query.UserIds.Count == 0)
        {
            return Result.Success(new List<UserProfileResponse>());
        }

        var users = await _uow.Users.GetUsersByIdsAsync(query.UserIds, cancellationToken);

        var profiles = users.Select(u => new UserProfileResponse(
            u.Id,
            u.Username,
            u.ProfileImage != null ? Convert.ToBase64String(u.ProfileImage) : string.Empty
        )).ToList();

        _logger.LogInformation("Retrieved {Count} user profiles", profiles.Count);

        return Result.Success(profiles);
    }
}
