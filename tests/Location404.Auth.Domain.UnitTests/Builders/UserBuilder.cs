using Location404.Auth.Domain.Entities;
using Location404.Auth.Domain.ValueObjects;

namespace Location404.Auth.Domain.UnitTests.Builders;

public class UserBuilder
{
    private EmailAddress _email = EmailAddress.Create("test@example.com");
    private string _username = "testuser";
    private string? _password = "ValidPassword123!";
    private string? _loginProvider;
    private string? _providerKey;

    public UserBuilder WithEmail(string email)
    {
        _email = EmailAddress.Create(email);
        return this;
    }

    public UserBuilder WithUsername(string username)
    {
        _username = username;
        return this;
    }

    public UserBuilder WithPassword(string password)
    {
        _password = password;
        _loginProvider = null;
        _providerKey = null;
        return this;
    }

    public UserBuilder WithExternalLogin(string provider, string key)
    {
        _loginProvider = provider;
        _providerKey = key;
        _password = null;
        return this;
    }

    public User Build()
    {
        if (_loginProvider != null && _providerKey != null)
            return User.Create(_email, _username, _loginProvider, _providerKey);

        return User.Create(_email, _username, _password!);
    }
}
