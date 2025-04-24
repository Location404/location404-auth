using UserIdentity.Domain.Common;
using UserIdentity.Domain.ValueObject;

namespace UserIdentity.Domain.Entities;

public class UserApplication : BaseEntity
{
    public string DisplayName { get; private set; }
    public string Username { get; private set; }
    public string EmailAddress { get; private set; }
    public string? ProfilePictureUrl { get; private set; }

    public string PreferredLanguage { get; private set; } = "pt-BR";

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public virtual PasswordAuthentication? LoginPassword { get; private set; }
    public virtual OAuthAuthentication? LoginOAuth { get; private set; }

    public UserApplication(string username, string emailAddress, PasswordAuthentication? passwordAuthentication = null, OAuthAuthentication? oAuthAuthentication = null)
    {
        Username = username;
        DisplayName = username;
        EmailAddress = new EmailAddress(emailAddress).Value;
        PreferredLanguage = "pt-BR";
        CreatedAt = DateTime.UtcNow;

        if (passwordAuthentication != null)
        {
            LoginPassword = passwordAuthentication;
            passwordAuthentication.SetUserApplication(this);
        }

        if (oAuthAuthentication != null)
        {
            LoginOAuth = oAuthAuthentication;
            oAuthAuthentication.SetUserApplication(this);
        }

        if (this is { LoginPassword: null, LoginOAuth: null })
        {
            throw new ArgumentException("At least one authentication method must be provided.");
        }
    }

    public void UpdateProfile(string displayName, string? profilePictureUrl = null)
    {
        DisplayName = displayName;
        if (profilePictureUrl != null) ProfilePictureUrl = profilePictureUrl;
        UpdatedAt = DateTime.UtcNow;
    }
}