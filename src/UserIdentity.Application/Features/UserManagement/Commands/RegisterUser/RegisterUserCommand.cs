using MediatR;
using UserIdentity.Application.Common.Results;

namespace UserIdentity.Application.Features.UserManagement.Commands.RegisterUser;

public record RegisterUserCommand(
    string Username,
    string Password,
    string EmailAddress
    ) : IRequest<Result<UserRegistrationResult>>;

