using LiteBus.Commands.Abstractions;
using UserIdentityService.Application.Common.Result;

namespace UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithPasswordCommand;

public class CreateUserWithPasswordHandler : ICommandHandler<CreateUserWithPasswordCommand, Result<CreateUserResult>>
{
    public async Task<Result<CreateUserResult>> HandleAsync(CreateUserWithPasswordCommand message, CancellationToken cancellationToken = default)
    {
        var result = new CreateUserResult("user-id", message.Username, message.Email);
        return await Task.FromResult(Result<CreateUserResult>.Success(result));
    }
}