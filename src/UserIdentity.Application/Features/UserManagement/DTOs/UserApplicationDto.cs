namespace UserIdentity.Application.Features.UserManagement.DTOs;

public record UserDto(
    Guid Id,
    string Username,
    string DisplayName,
    string Email,
    string ProfilePictureUrl,
    string PreferredLanguage
);