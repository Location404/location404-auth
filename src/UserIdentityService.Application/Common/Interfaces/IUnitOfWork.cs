namespace UserIdentityService.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}