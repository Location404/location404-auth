namespace UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithPasswordCommand;

public record CreateUserWithPasswordCommandResponse(
    string Id,
    string Username,
    string Email
);