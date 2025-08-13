namespace UserIdentityService.Domain.Entities;

public class ExternalLogin
{
    public Guid UserId { get; private set; }

    public string LoginProvider { get; private set; }
    public string ProviderKey { get; private set; }

    public virtual User User { get; private set; } = null!;

    /// <summary>
    /// Initializes a new <see cref="ExternalLogin"/> for the specified user and external provider.
    /// </summary>
    /// <param name="userId">Identifier of the user to which this external login belongs.</param>
    /// <param name="loginProvider">Name of the external login provider (e.g., "Google", "Facebook"). Must not be null or whitespace.</param>
    /// <param name="providerKey">Provider-specific key identifying the external account. Must not be null or whitespace.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="loginProvider"/> or <paramref name="providerKey"/> is null, empty, or consists only of white-space characters.</exception>
    /// <remarks>The <see cref="User"/> navigation property is not set by this constructor and is expected to be populated by the ORM.</remarks>
    public ExternalLogin(Guid userId, string loginProvider, string providerKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(loginProvider, nameof(loginProvider));
        ArgumentException.ThrowIfNullOrWhiteSpace(providerKey, nameof(providerKey));

        UserId = userId;
        LoginProvider = loginProvider;
        ProviderKey = providerKey;
    }
}