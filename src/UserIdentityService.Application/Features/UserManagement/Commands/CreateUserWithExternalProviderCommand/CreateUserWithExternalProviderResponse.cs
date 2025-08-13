namespace UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithExternalProviderCommand;

public record CreateUserWithExternalProviderResponse(
    string Id,
    string Username,
    string Email
);