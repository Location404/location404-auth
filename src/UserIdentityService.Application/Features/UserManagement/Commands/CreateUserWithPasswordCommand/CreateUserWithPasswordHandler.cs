using LiteBus.Commands.Abstractions;
using UserIdentityService.Application.Common.Result;

namespace UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithPasswordCommand;

public class CreateUserWithPasswordHandler : ICommandHandler<CreateUserWithPasswordCommand, Result<CreateUserWithPasswordCommandResponse>>
{
    public async Task<Result<CreateUserWithPasswordCommandResponse>> HandleAsync(CreateUserWithPasswordCommand message,
        CancellationToken cancellationToken = default)
    {
        var result = new CreateUserWithPasswordCommandResponse("user-id", message.Username, message.Email);
        return await Task.FromResult(Result<CreateUserWithPasswordCommandResponse>.Success(result));
    }
}