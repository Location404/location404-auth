using UserIdentity.Infra.Common;

namespace UserIdentity.Infra.Settings;

public record GoogleLoginSettings : ExternalLoginSettingsBase
{
    public static string SectionName = ExternalLoginSectionName + "Google";

    public GoogleLoginSettings(string clientId, string clientSecret, string clientCallbackUrl)
        : base(clientId, clientSecret, clientCallbackUrl) { }
}