using UserIdentity.Domain.Common;
using UserIdentity.Domain.ValueObject;

namespace UserIdentity.Domain.Entities;

public class UserApplication(string username, string emailAddress, PasswordAuthentication? passwordAuthentication = null, OAuthAuthentication? oAuthAuthentication = null) : BaseEntity
{
    public string DisplayName { get; private set; } = username;
    public string Username { get; private set; } = username;
    public string EmailAddress { get; private set; } = new EmailAddress(emailAddress).Value;
    public string? ProfilePictureUrl { get; private set; }

    public string PreferredLanguage { get; private set; } = "pt-BR";

    public virtual PasswordAuthentication? LoginPassword { get; private set; } = passwordAuthentication;
    public virtual OAuthAuthentication? LoginOAuth { get; private set; } = oAuthAuthentication;
}