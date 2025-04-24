namespace UserIdentity.Application.Features.UserApplicationManagement.DTOs;

public record UserApplicationDto(
    Guid Id,
    string Username,
    string DisplayName,
    string Email,
    string ProfilePictureUrl,
    string PreferredLanguage
);