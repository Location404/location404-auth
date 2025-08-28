namespace UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithExternalProviderCommand;

public record CreateUserWithExternalProviderCommandResponse(
    string Id,
    string Username,
    string Email
);