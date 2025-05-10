using MediatR;
using UserIdentity.Application.Common.Results;

namespace UserIdentity.Application.Features.Authentication.Commands.LoginUser;

public record LoginUserCommand(
    string Email,
    string Password) : IRequest<Result<LoginUserCommandResult>>;