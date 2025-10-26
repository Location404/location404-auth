namespace Location404.Auth.Domain.Entities;

public class ExternalLogin
{
    public Guid UserId { get; private set; }

    public string LoginProvider { get; private set; }
    public string ProviderKey { get; private set; }

    public virtual User User { get; private set; } = null!;

    public ExternalLogin(Guid userId, string loginProvider, string providerKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(loginProvider, nameof(loginProvider));
        ArgumentException.ThrowIfNullOrWhiteSpace(providerKey, nameof(providerKey));

        UserId = userId;
        LoginProvider = loginProvider;
        ProviderKey = providerKey;
    }
}