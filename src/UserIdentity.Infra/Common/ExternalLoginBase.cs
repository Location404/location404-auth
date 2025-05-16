namespace UserIdentity.Infra.Common;

public abstract record ExternalLoginSettingsBase(string ClientId, string ClientSecret, string ClientCallbackUrl)
{
    protected static string ExternalLoginSectionName = "ExternalLogin:";
    public string ClientId { get; private set; } = ClientId;
    public string ClientSecret { get; private set; } = ClientSecret;
    public string ClientCallbackUrl { get; private set; } = ClientCallbackUrl;
}