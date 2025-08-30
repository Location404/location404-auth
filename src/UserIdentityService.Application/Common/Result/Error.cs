namespace UserIdentityService.Application.Common.Result;

public enum ErrorType
{
    /// <summary>
    /// Um erro genérico de falha.
    /// </summary>
    Failure,

    /// <summary>
    /// Um erro de validação (ex: input inválido).
    /// </summary>
    Validation,

    /// <summary>
    /// Um recurso não foi encontrado.
    /// </summary>
    NotFound,

    /// <summary>
    /// Houve um conflito de estado (ex: tentar criar um recurso que já existe).
    /// </summary>
    Conflict,

    /// <summary>
    /// O requisitante não tem autorização.
    /// </summary>
    Unauthorized,

    /// <summary>
    /// O requisitante não está autenticado.
    /// </summary>
    UnAuthenticated,

    /// <summary>
    /// Ocorreu um erro ao acessar o banco de dados.
    /// </summary>
    Database
}
public record Error(string Code, string Message, ErrorType Type = ErrorType.Failure)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}