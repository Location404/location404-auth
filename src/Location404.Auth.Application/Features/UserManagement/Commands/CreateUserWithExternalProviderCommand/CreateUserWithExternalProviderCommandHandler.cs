using LiteBus.Commands.Abstractions;
using Location404.Auth.Application.Common.Result;

namespace Location404.Auth.Application.Features.UserManagement.Commands.CreateUserWithExternalProviderCommand;

public class CreateUserWithExternalProviderCommandHandler : ICommandHandler<CreateUserWithExternalProviderCommand, Result<CreateUserWithExternalProviderCommandResponse>>
{
    public Task<Result<CreateUserWithExternalProviderCommandResponse>> HandleAsync(CreateUserWithExternalProviderCommand message,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}