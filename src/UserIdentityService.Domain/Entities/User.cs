using UserIdentityService.Domain.ValueObjects;

namespace UserIdentityService.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public EmailAddress Email { get; private set; }
    public string? Password { get; private set; }
    public string Username { get; private set; }
    public string? ProfileImageUrl { get; private set; }

    public bool EmailVerified { get; private set; }
    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    // public Level Level { get; private set; }

    public string PreferredLanguage { get; private set; }

    public ICollection<ExternalLogin> ExternalLogins { get; private set; }

    private User(string email, string username, string? password = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email, nameof(email));
        ArgumentException.ThrowIfNullOrWhiteSpace(username, nameof(username));

        Id = Guid.NewGuid();
        Email = EmailAddress.Create(email);
        Username = username;
        Password = password;
        EmailVerified = false;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        PreferredLanguage = "pt-BR";
        // Level = Level.Initial;
        ExternalLogins = [];
    }

    public static User Create(string email, string username, string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));

        return new User(email, username, password);
    }

    public static User Create(string email, string username, string loginProvider, string providerKey)
    {
        var user = new User(email, username);
        user.AddExternalLogin(loginProvider, providerKey);
        user.EmailVerified = true;

        return user;
    }

    public void UpdateProfile(string? username, string? profileImageUrl = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username, nameof(username));

        ProfileImageUrl = profileImageUrl;
        Username = username;
        UpdatedAt = DateTime.UtcNow;
    }

    public void VerifyEmail()
    {
        EmailVerified = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    // public void AddExperience(int points)
    // {
    //     Level = Level.AddExperience(points);
    //     UpdatedAt = DateTime.UtcNow;
    // }

    public void ChangePreferredLanguage(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language, nameof(language));

        PreferredLanguage = language;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddExternalLogin(string loginProvider, string providerKey)
    {
        var alreadyExists = ExternalLogins.Any(l => l.LoginProvider.Equals(loginProvider, StringComparison.OrdinalIgnoreCase));
        if (alreadyExists)
        {
            throw new InvalidOperationException($"Um login do provedor '{loginProvider}' já está associado a este usuário.");
        }

        var newLogin = new ExternalLogin(Id, loginProvider, providerKey);
        ExternalLogins.Add(newLogin);

        UpdatedAt = DateTime.UtcNow;
    }
}