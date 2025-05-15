namespace UserIdentity.Infra.Common;

public abstract record ExternalLoginSettingsBase(string ClientId, string ClientSecret, string CallbackPath)
{
    public static string SectionName = "ExternalLogin__";
    public string ClientId { get; private set; } = ClientId;
    public string ClientSecret { get; private set; } = ClientSecret;
    public string CallbackPath { get; private set; } = CallbackPath;
}