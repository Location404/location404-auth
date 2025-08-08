using UserIdentityService.Application.Common.Interfaces;

namespace UserIdentityService.Infrastructure.Services;

public class BcryptEncryptPasswordService : IEncryptPasswordService
{
    public string Encrypt(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));
        }

        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool Verify(string password, string encryptedPassword)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(
                password,
                nameof(password));
        }

        if (string.IsNullOrWhiteSpace(encryptedPassword))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(
                encryptedPassword,
                nameof(encryptedPassword));
        }

        return BCrypt.Net.BCrypt.Verify(password, encryptedPassword);
    }
}