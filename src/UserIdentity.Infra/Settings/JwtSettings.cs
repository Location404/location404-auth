namespace UserIdentity.Infra.Settings;

public record JwtSettings
{
    public required string SecretKey { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public int AccessTokenExpirationMinutes { get; init; } = 15;
    public int RefreshTokenExpirationDays { get; init; } = 7;
    public bool ValidateLifetime { get; init; } = true;

    public const string SectionName = "JwtSettings";
}
