using LiteBus.Commands.Abstractions;
using UserIdentityService.Application.Common.Result;

namespace UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithPasswordCommand;

public class CreateUserWithPasswordHandler : ICommandHandler<CreateUserWithPasswordCommand, Result<CreateUserWithPasswordCommandResponse>>
{
    /// <summary>
    /// Handles a CreateUserWithPasswordCommand by constructing a CreateUserWithPasswordCommandResponse and returning it wrapped in a successful Result.
    /// </summary>
    /// <param name="message">The command containing the new user's Username and Email.</param>
    /// <param name="cancellationToken">Cancellation token (not observed by this implementation).</param>
    /// <returns>A successful Result containing a CreateUserWithPasswordCommandResponse with a hard-coded Id ("user-id"), and the Username and Email from the command.</returns>
    public async Task<Result<CreateUserWithPasswordCommandResponse>> HandleAsync(CreateUserWithPasswordCommand message, CancellationToken cancellationToken = default)
    {
        var result = new CreateUserWithPasswordCommandResponse("user-id", message.Username, message.Email);
        return await Task.FromResult(Result<CreateUserWithPasswordCommandResponse>.Success(result));
    }
}