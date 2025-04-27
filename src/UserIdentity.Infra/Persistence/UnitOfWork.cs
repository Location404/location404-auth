using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

using UserIdentity.Application.Common.Interfaces;
using UserIdentity.Application.Features.UserManagement;
using UserIdentity.Infra.Context;

namespace UserIdentity.Infra.Persistence;



/// <summary>
/// Implementação concreta do padrão Unit of Work para gerenciar transações e persistência de dados.
/// </summary>
/// <remarks>
/// Inicializa uma nova instância da classe UnitOfWork.
/// </remarks>
/// <param name="dbContext">Contexto de banco de dados da aplicação</param>
/// <param name="logger">Logger para operações de transação</param>
public class UnitOfWork(UserIdentityContext dbContext, IUserRepository userRepository, ILogger<UnitOfWork> logger) : IUnitOfWork
{
    private readonly UserIdentityContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private readonly ILogger<UnitOfWork> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    public IUserRepository UserRepository { get; } = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

    /// <inheritdoc/>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Opcionalmente, aqui podem ser adicionados comportamentos como:
            // - Timestamp automático em entidades auditáveis
            // - Soft delete
            // - Registro de histórico de alterações

            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Conflito de concorrência ao salvar alterações");
            throw;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Erro ao salvar alterações no banco de dados");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Iniciando nova transação de banco de dados");
        return await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        try
        {
            await transaction.CommitAsync(cancellationToken);
            _logger.LogDebug("Transação confirmada com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao confirmar transação");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task RollbackTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        try
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogDebug("Transação desfeita com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao desfazer transação");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<TResult> ExecuteTransactionAsync<TResult>(Func<Task<TResult>> func, CancellationToken cancellationToken = default)
    {
        using var transaction = await BeginTransactionAsync(cancellationToken);
        
        try
        {
            var result = await func();
            await CommitTransactionAsync(transaction, cancellationToken);
            return result;
        }
        catch (Exception)
        {
            await RollbackTransactionAsync(transaction, cancellationToken);
            throw;
        }
    }

    #region [ IDisposable Implementation ]

    private bool _disposed;


    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _dbContext?.Dispose();
        }
        
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            if (_dbContext != null)
            {
                await _dbContext.DisposeAsync();
            }
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }

    #endregion
}
