using LiteBus.Queries.Abstractions;
using Microsoft.Extensions.Logging;
using Location404.Auth.Application.Common.Interfaces;
using Location404.Auth.Application.Common.Result;

namespace Location404.Auth.Application.Features.UserManagement.Queries.GetCurrentUserInformation;

public class GetCurrentUserInformationQueryHandler(
    IUnitOfWork uow,
    ILogger<GetCurrentUserInformationQueryHandler> logger
) : IQueryHandler<GetCurrentUserInformationQuery, Result<GetCurrentUserInformationQueryResponse>>
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<GetCurrentUserInformationQueryHandler> _logger = logger;

    public async Task<Result<GetCurrentUserInformationQueryResponse>> HandleAsync(GetCurrentUserInformationQuery userId, CancellationToken cancellationToken = default)
    {
        var user = await _uow.Users.GetUserByIdAsync(userId.Value, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", userId);
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        return Result.Success(new GetCurrentUserInformationQueryResponse(
            Id: user.Id,
            Username: user.Username,
            Email: user.Email,
            ProfileImage: Convert.ToBase64String(user.ProfileImage ?? [])
        ));
    }
}