using Microsoft.EntityFrameworkCore.Storage;

using UserIdentity.Application.Features.UserManagement;

namespace UserIdentity.Application.Common.Interfaces;

/// <summary>
/// Define contrato para operações de unidade de trabalho, gerenciando transações e persistência.
/// </summary>
public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    public IUserRepository UserRepository { get; }

    /// <summary>
    /// Salva todas as mudanças feitas no contexto de forma atômica.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento da operação</param>
    /// <returns>Número de entidades afetadas</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Inicia uma nova transação.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento da operação</param>
    /// <returns>Task representando a operação assíncrona</returns>
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirma a transação atual.
    /// </summary>
    /// <param name="transaction">Transação a ser confirmada</param>
    /// <param name="cancellationToken">Token de cancelamento da operação</param>
    /// <returns>Task representando a operação assíncrona</returns>
    Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default);

    /// <summary>
    /// Desfaz a transação atual.
    /// </summary>
    /// <param name="transaction">Transação a ser desfeita</param>
    /// <param name="cancellationToken">Token de cancelamento da operação</param>
    /// <returns>Task representando a operação assíncrona</returns>
    Task RollbackTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executa uma função dentro de uma transação, com gerenciamento automático de commit e rollback.
    /// </summary>
    /// <typeparam name="TResult">Tipo do resultado da função</typeparam>
    /// <param name="func">Função a ser executada dentro da transação</param>
    /// <param name="cancellationToken">Token de cancelamento da operação</param>
    /// <returns>Resultado da função executada</returns>
    Task<TResult> ExecuteTransactionAsync<TResult>(Func<Task<TResult>> func, CancellationToken cancellationToken = default);
}