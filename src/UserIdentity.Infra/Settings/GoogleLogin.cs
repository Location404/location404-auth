
using UserIdentity.Infra.Common;

namespace UserIdentity.Infra.Settings;

public record GoogleLoginSettings : ExternalLoginSettingsBase
{
    public GoogleLoginSettings(string clientId, string clientSecret, string callbackPath) 
        : base(clientId, clientSecret, callbackPath)
    {
        SectionName += "GoogleLogin";
    }
}