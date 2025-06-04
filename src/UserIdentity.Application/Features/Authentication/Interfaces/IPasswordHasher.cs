namespace UserIdentity.Application.Features.Authentication.Interfaces;

public interface IPasswordHasher
{
    /// <summary>
    /// Cria um hash e um salt para a senha fornecida.
    /// </summary>
    /// <param name="password">A senha a ser hasheada.</param>
    /// <returns>Uma tupla contendo o hash e o salt gerados.</returns>
    (string hash, string salt) CreatePasswordHash(string password);

    /// <summary>
    /// Verifica se a senha fornecida corresponde ao hash armazenado.
    /// </summary>
    /// <param name="password">A senha a ser verificada.</param>
    /// <param name="storedHash">O hash armazenado.</param>
    /// <param name="storedSalt">O salt armazenado.</param>
    /// <returns>Um valor booleano indicando se a senha corresponde ao hash.</returns>
    bool VerifyPasswordHash(string password, string storedHash, string storedSalt);
}