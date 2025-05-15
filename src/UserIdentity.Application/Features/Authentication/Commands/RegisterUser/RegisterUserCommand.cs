using MediatR;
using UserIdentity.Application.Common.Results;

namespace UserIdentity.Application.Features.Authentication.Commands.RegisterUser;

public record RegisterUserCommand(
    string Username,
    string Password,
    string EmailAddress
    ) : IRequest<Result<RegisterUserResult>>;

