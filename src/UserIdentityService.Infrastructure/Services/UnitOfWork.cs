using UserIdentityService.Application.Common.Interfaces;
using UserIdentityService.Infrastructure.Context;

namespace UserIdentityService.Infrastructure.Services;

public class UnitOfWork(IUserRepository userRepository, UserIdentityDbContext context) : IUnitOfWork
{
    public IUserRepository Users => userRepository;
    private readonly UserIdentityDbContext _context = context;

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// The number of state entries written to the database.
    /// </returns>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await _context.Database.RollbackTransactionAsync(cancellationToken);
    }
}