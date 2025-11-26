namespace Location404.Auth.Application.Features.UserManagement.Queries.GetUsersProfiles;

public record UserProfileResponse(
    Guid Id,
    string Username,
    string ProfileImage
);
