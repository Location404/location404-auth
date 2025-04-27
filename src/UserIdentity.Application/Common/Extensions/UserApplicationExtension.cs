using UserIdentity.Application.Features.UserManagement.DTOs;
using UserIdentity.Domain.Entities;

namespace UserIdentity.Application.Common.Extensions;

public static class UserApplicationExtensions
{
    public static UserApplicationDto ToDto(this UserApplication user)
    {
        return new UserApplicationDto(
            id: user.Id,
            username: user.Username,
            displayName: user.DisplayName,
            emailAddress: user.EmailAddress,
            profilePictureUrl: user.ProfilePictureUrl,
            preferredLanguage: user.PreferredLanguage);
    }
}