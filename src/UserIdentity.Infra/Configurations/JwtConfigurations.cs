namespace UserIdentity.Infra.Configurations;

public record JwtConfiguration(
    string Issuer,
    string Audience,
    string SecretKey,
    int ExpirationInMinutes
);