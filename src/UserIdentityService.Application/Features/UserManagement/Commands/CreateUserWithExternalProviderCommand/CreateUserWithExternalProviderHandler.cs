using LiteBus.Commands.Abstractions;
using UserIdentityService.Application.Common.Result;

namespace UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithExternalProviderCommand;

public class CreateUserWithExternalProviderHandler : ICommandHandler<CreateUserWithExternalProviderCommand, Result<CreateUserWithExternalProviderResponse>>
{
    /// <summary>
    /// Handles a command to create a user using an external authentication provider and returns the creation result.
    /// </summary>
    /// <param name="message">The command containing external provider credentials and user details required to create the user.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="Result{CreateUserWithExternalProviderResponse}"/> representing success or failure and containing the created user's data on success.</returns>
    /// <exception cref="NotImplementedException">Thrown because the handler is not yet implemented.</exception>
    public Task<Result<CreateUserWithExternalProviderResponse>> HandleAsync(
        CreateUserWithExternalProviderCommand message,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}