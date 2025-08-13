namespace UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithExternalProviderCommand;

public record CreateUserResult(
    string Id,
    string Username,
    string Email
);