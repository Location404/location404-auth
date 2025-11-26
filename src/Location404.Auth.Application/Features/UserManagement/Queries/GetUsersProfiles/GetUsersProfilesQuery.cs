using LiteBus.Queries.Abstractions;
using Location404.Auth.Application.Common.Result;

namespace Location404.Auth.Application.Features.UserManagement.Queries.GetUsersProfiles;

public record GetUsersProfilesQuery(List<Guid> UserIds) : IQuery<Result<List<UserProfileResponse>>>;
