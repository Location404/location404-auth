using UserIdentityService.Application.Common.Interfaces;

namespace UserIdentityService.Infrastructure.Services;

public class BcryptEncryptPasswordService : IEncryptPasswordService
{
    /// <summary>
    /// Hashes a plaintext password using the BCrypt algorithm.
    /// </summary>
    /// <param name="password">The plaintext password to hash; must not be null, empty, or whitespace.</param>
    /// <returns>The BCrypt-hashed representation of <paramref name="password"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="password"/> is null, empty, or consists only of whitespace.</exception>
    public string Encrypt(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));
        }

        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Verifies whether the provided plaintext password matches the given BCrypt hashed password.
    /// </summary>
    /// <param name="password">The plaintext password to verify. Must not be null, empty, or whitespace.</param>
    /// <param name="encryptedPassword">The BCrypt hash to verify against. Must not be null, empty, or whitespace.</param>
    /// <returns>True if the plaintext password matches the hashed password; otherwise false.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="password"/> or <paramref name="encryptedPassword"/> is null, empty, or consists only of whitespace.</exception>
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