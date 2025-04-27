using MediatR;
using UserIdentity.Application.Common.Results;

namespace UserIdentity.Application.Features.UserManagement.Commands;

public record RegisterUserCommand(
    string Username,
    string EmailAddress,
    string Password) : IRequest<Result<UserRegistrationResult>>;

