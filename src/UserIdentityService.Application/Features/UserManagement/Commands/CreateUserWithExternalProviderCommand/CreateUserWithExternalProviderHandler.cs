using LiteBus.Commands.Abstractions;
using UserIdentityService.Application.Common.Result;

namespace UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithExternalProviderCommand;

public class CreateUserWithExternalProviderHandler : ICommandHandler<CreateUserWithExternalProviderCommand, Result<CreateUserWithExternalProviderResponse>>
{
    public Task<Result<CreateUserWithExternalProviderResponse>> HandleAsync(
        CreateUserWithExternalProviderCommand message,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}