namespace UserIdentityService.Application.Common.Interfaces;

public interface IEncryptPasswordService
{
    /// <summary>
    /// Criptografa a senha fornecida.
    /// </summary>
    /// <param name="password">A senha a ser criptografada.</param>
    /// <returns>A senha criptografada.</returns>
    string Encrypt(string password);

    /// <summary>
    /// verifica se a senha fornecida corresponde à senha criptografada.
    /// </summary>
    /// <param name="password">A senha em texto simples a ser verificada.</param>
    /// <param name="encryptedPassword">A senha criptografada a ser comparada.</param>
    /// <returns>True se as senhas corresponderem, caso contrário false.</returns>
    bool Verify(string password, string encryptedPassword);
}