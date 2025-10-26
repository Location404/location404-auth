using LiteBus.Queries.Abstractions;

using Location404.Auth.Application.Common.Result;

namespace Location404.Auth.Application.Features.UserManagement.Queries.GetCurrentUserInformation;

public class GetCurrentUserInformationQuery(Guid userId) : IQuery<Result<GetCurrentUserInformationQueryResponse>>
{
    public Guid Value { get; } = userId;
}