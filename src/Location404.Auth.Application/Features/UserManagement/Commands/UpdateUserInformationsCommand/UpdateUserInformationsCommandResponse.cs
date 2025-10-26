namespace Location404.Auth.Application.Features.UserManagement.Commands.UpdateUserInformationsCommand;

public record UpdateUserInformationsCommandResponse(
    Guid Id,
    string Username,
    string Email,
    byte[]? ProfileImage
);