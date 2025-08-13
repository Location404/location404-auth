namespace UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithPasswordCommand;

public record CreateUserResult(
    string Id,
    string Username,
    string Email
);