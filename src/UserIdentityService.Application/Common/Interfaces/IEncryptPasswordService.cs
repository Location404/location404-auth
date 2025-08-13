namespace UserIdentityService.Application.Common.Interfaces;

public interface IEncryptPasswordService
{
    /// <summary>
    /// Criptografa a senha fornecida.
    /// </summary>
    /// <param name="password">A senha a ser criptografada.</param>
    /// <summary>
/// Encrypts a plain-text password and returns its encrypted representation.
/// </summary>
/// <param name="password">The plain-text password to encrypt.</param>
/// <returns>The encrypted password.</returns>
    string Encrypt(string password);

    /// <summary>
    /// verifica se a senha fornecida corresponde à senha criptografada.
    /// </summary>
    /// <param name="password">A senha em texto simples a ser verificada.</param>
    /// <param name="encryptedPassword">A senha criptografada a ser comparada.</param>
    /// <summary>
/// Verifies whether a plain-text password matches a stored encrypted password.
/// </summary>
/// <param name="password">The plain-text password to verify.</param>
/// <param name="encryptedPassword">The stored encrypted (hashed) password to compare against.</param>
/// <returns><c>true</c> if the plain-text password corresponds to the encrypted password; otherwise <c>false</c>.</returns>
    bool Verify(string password, string encryptedPassword);
}