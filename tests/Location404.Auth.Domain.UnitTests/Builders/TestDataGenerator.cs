using Bogus;

namespace Location404.Auth.Domain.UnitTests.Builders;

public class TestDataGenerator
{
    private readonly Faker _faker = new();

    public string GenerateEmail() => _faker.Internet.Email();
    public string GenerateUsername() => _faker.Internet.UserName();
    public string GeneratePassword() => _faker.Internet.Password(12, true);
    public string GenerateProviderKey() => _faker.Random.Guid().ToString();
    public string GenerateLanguage() => _faker.PickRandom("pt-BR", "en-US", "es-ES");
    public byte[] GenerateProfileImage() => _faker.Random.Bytes(1024);
}
