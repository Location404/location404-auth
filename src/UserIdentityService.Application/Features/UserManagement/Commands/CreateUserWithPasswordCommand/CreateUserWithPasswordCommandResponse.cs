namespace UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithPasswordCommand;

public record CreateUserWithPasswordCommandResponse(
    Guid Id,
    string Username,
    string Email
);