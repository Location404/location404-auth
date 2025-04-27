namespace UserIdentity.Application.Features.UserManagement.DTOs;

public class UserApplicationDto
{
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string DisplayName { get; private set; }
    public string EmailAddress { get; private set; }
    public string? ProfilePictureUrl { get; private set; }
    public string PreferredLanguage { get; private set; }

    public UserApplicationDto(Guid id, string username, string displayName, string emailAddress, string? profilePictureUrl, string preferredLanguage)
    {
        Id = id;
        Username = username;
        DisplayName = displayName;
        EmailAddress = emailAddress;
        ProfilePictureUrl = profilePictureUrl;
        PreferredLanguage = preferredLanguage;
    }
}