namespace UserIdentityService.Application.Features.UserManagement.Commands.UpdateUserInformationsCommand;

public record UpdateUserInformationsCommandResponse(
    Guid Id,
    string Username,
    string Email
);