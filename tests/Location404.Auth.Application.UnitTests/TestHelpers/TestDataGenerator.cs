using Bogus;

namespace Location404.Auth.Application.UnitTests.TestHelpers;

public class TestDataGenerator
{
    private readonly Faker _faker = new();

    public string GenerateEmail() => _faker.Internet.Email();
    public string GenerateUsername() => _faker.Internet.UserName();
    public string GeneratePassword() => _faker.Internet.Password(12, true);
    public string GenerateToken() => _faker.Random.Guid().ToString();
    public string GenerateProviderKey() => _faker.Random.Guid().ToString();
}
