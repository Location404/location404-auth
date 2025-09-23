using LiteBus.Queries.Abstractions;

using UserIdentityService.Application.Common.Result;

namespace UserIdentityService.Application.Features.UserManagement.Queries.GetCurrentUserInformation;

public class GetCurrentUserInformationQuery(Guid userId) : IQuery<Result<GetCurrentUserInformationQueryResponse>>
{
    public Guid Value { get; } = userId;
}