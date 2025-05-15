using Microsoft.EntityFrameworkCore;

using UserIdentity.Application.Features.UserManagement;
using UserIdentity.Domain.Entities;
using UserIdentity.Infra.Context;

namespace UserIdentity.Infra.Persistence;

public class UserRepository(UserIdentityContext context) : IUserRepository
{
    private readonly DbSet<UserApplication> _userDbSet = context.Set<UserApplication>();

    public async Task AddAsync(UserApplication user, CancellationToken cancellationToken = default)
    {
        await _userDbSet.AddAsync(user, cancellationToken);
    }

    public async Task<bool> ExistsByUsernameOrEmailAsync(string? username = null, string? email = null, CancellationToken cancellationToken = default)
    {
        return await _userDbSet.AnyAsync(u => u.Username == username || u.EmailAddress == email, cancellationToken);
    }

    public async Task<UserApplication?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _userDbSet.SingleOrDefaultAsync(u => u.EmailAddress == email, cancellationToken);
    }

    public Task<UserApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _userDbSet.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public Task<UserApplication?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return _userDbSet.SingleOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<UserApplication?> GetByUsernameOrEmailAsync(string? username = null, string? email = null, CancellationToken cancellationToken = default)
    {
        return await _userDbSet.SingleOrDefaultAsync(u => u.Username == username || u.EmailAddress == email, cancellationToken);
    }
}