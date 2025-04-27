namespace UserIdentity.Infra.Settings;

public record JwtSettings(
    string SecretKey,
    string Issuer,
    string Audience,
    int AccessTokenExpirationMinutes = 15,
    int RefreshTokenExpirationDays = 7);