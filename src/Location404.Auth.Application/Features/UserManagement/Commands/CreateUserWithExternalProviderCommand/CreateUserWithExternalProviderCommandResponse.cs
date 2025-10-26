namespace Location404.Auth.Application.Features.UserManagement.Commands.CreateUserWithExternalProviderCommand;

public record CreateUserWithExternalProviderCommandResponse(
    Guid Id,
    string Username,
    string Email
);