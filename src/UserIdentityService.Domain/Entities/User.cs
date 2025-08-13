using UserIdentityService.Domain.ValueObjects;

namespace UserIdentityService.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public EmailAddress Email { get; private set; }
    public string? Password { get; private set; }
    public string Username { get; private set; }
    public byte[]? ProfileImage { get; private set; }

    public bool EmailVerified { get; private set; }
    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    // public Level Level { get; private set; }

    public string PreferredLanguage { get; private set; }

    public ICollection<ExternalLogin> ExternalLogins { get; private set; }

    /// <summary>
    /// Initializes a new User instance with the specified email and username and optional password.
    /// </summary>
    /// <param name="email">The user's email address value object.</param>
    /// <param name="username">The user's display name. Must not be null or whitespace.</param>
    /// <param name="password">Optional password; null when the user is created via external login.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="username"/> is null or whitespace.</exception>
    /// <remarks>
    /// Sets default state for a new user: generates a new Id, marks the account active with email unverified,
    /// sets CreatedAt and UpdatedAt to UTC now, defaults PreferredLanguage to "pt-BR", and initializes the ExternalLogins collection.
    /// </remarks>
    private User(EmailAddress email, string username, string? password = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username, nameof(username));

        Id = Guid.NewGuid();
        Email = email;
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

    /// <summary>
    /// Creates a new user account using an email, username, and password.
    /// </summary>
    /// <param name="email">User's validated email address value object.</param>
    /// <param name="username">User's display name; must not be null or whitespace.</param>
    /// <param name="password">User's password; must not be null or whitespace.</param>
    /// <returns>A new <see cref="User"/> instance initialized with the provided credentials. The returned user is active, email-verified is false, and creation/updated timestamps are set to UTC now.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="password"/> or <paramref name="username"/> is null or whitespace.</exception>
    public static User Create(EmailAddress email, string username, string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));

        return new User(email, username, password);
    }

    /// <summary>
    /// Creates a new user associated with an external login provider.
    /// </summary>
    /// <remarks>
    /// The created user is initialized without a local password and has <see cref="EmailVerified"/> set to <c>true</c>.
    /// An <see cref="ExternalLogin"/> for the specified provider is added to the user's <see cref="ExternalLogins"/>.
    /// </remarks>
    /// <param name="email">The user's email address value object.</param>
    /// <param name="username">The user's username; must not be null or whitespace.</param>
    /// <param name="loginProvider">The external login provider name; must not be null or whitespace.</param>
    /// <param name="providerKey">The provider-specific key/identifier; must not be null or whitespace.</param>
    /// <returns>A new <see cref="User"/> configured with the external login and email verified.</returns>
    /// <exception cref="System.ArgumentException">Thrown when <paramref name="username"/>, <paramref name="loginProvider"/>, or <paramref name="providerKey"/> is null or whitespace.</exception>
    public static User Create(EmailAddress email, string username, string loginProvider, string providerKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(loginProvider, nameof(loginProvider));
        ArgumentException.ThrowIfNullOrWhiteSpace(providerKey, nameof(providerKey));
        ArgumentException.ThrowIfNullOrWhiteSpace(username, nameof(username));

        var user = new User(email, username);
        user.AddExternalLogin(loginProvider, providerKey);
        user.EmailVerified = true;

        return user;
    }

    /// <summary>
    /// Updates the user's display name and optional profile image.
    /// </summary>
    /// <param name="username">New username; must not be null or whitespace.</param>
    /// <param name="profileImage">Optional profile image bytes; pass null to clear.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="username"/> is null or whitespace.</exception>
    /// <remarks>The user's UpdatedAt timestamp is set to the current UTC time.</remarks>
    public void UpdateProfile(string? username, byte[]? profileImage = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username, nameof(username));

        ProfileImage = profileImage;
        Username = username;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the user's email as verified.
    /// </summary>
    /// <remarks>
    /// Sets <see cref="EmailVerified"/> to true and updates <see cref="UpdatedAt"/> to the current UTC time.
    /// </remarks>
    public void VerifyEmail()
    {
        EmailVerified = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Records a successful user login by updating the user's last-login timestamp.
    /// </summary>
    /// <remarks>
    /// Sets <see cref="LastLoginAt"/> and <see cref="UpdatedAt"/> to the current UTC time.
    /// </remarks>
    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the user as inactive.
    /// </summary>
    /// <remarks>
    /// Sets <see cref="IsActive"/> to <c>false</c> and updates <see cref="UpdatedAt"/> to the current UTC time.
    /// </remarks>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the user as active.
    /// </summary>
    /// <remarks>
    /// Sets <see cref="IsActive"/> to <c>true</c> and updates <see cref="UpdatedAt"/> to the current UTC time.
    /// </remarks>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    // public void AddExperience(int points)
    // {
    //     Level = Level.AddExperience(points);
    //     UpdatedAt = DateTime.UtcNow;
    /// <summary>
    /// Changes the preferred language of the user.
    /// </summary>
    /// <param name="language">The new preferred language to set for the user.</param>
    /// <remarks>
    /// This method updates the `PreferredLanguage` property of the user and sets the `UpdatedAt` property to the current UTC time.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown if the `language` parameter is null or whitespace.</exception>

    public void ChangePreferredLanguage(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language, nameof(language));

        PreferredLanguage = language;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Associates an external login (OAuth/OpenID provider) with this user.
    /// </summary>
    /// <param name="loginProvider">The external provider name (e.g., "google", "facebook"). Comparison is case-insensitive.</param>
    /// <param name="providerKey">The unique identifier/key provided by the external provider for this user.</param>
    /// <exception cref="InvalidOperationException">Thrown if an external login for the same provider is already associated with the user.</exception>
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