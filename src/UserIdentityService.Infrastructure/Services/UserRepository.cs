using Microsoft.EntityFrameworkCore;

using UserIdentityService.Application.Common.Interfaces;
using UserIdentityService.Domain.Entities;
using UserIdentityService.Infrastructure.Context;

namespace UserIdentityService.Infrastructure.Services;

public class UserRepository(UserIdentityDbContext context) : IUserRepository
{
    private readonly UserIdentityDbContext _context = context;

    public async Task AddUserAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public async Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await GetUserByIdAsync(userId, cancellationToken);
        if (user != null)
        {
            _context.Users.Remove(user);
            return true;
        }

        return false;
    }

    public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public bool UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        var entry = _context.Users.Update(user);
        return entry.State == EntityState.Modified;
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    # region [RefreshTokens]

    public void AddRefreshToken(RefreshToken token, CancellationToken cancellationToken)
    {
        _context.RefreshTokens.Add(token);
    }

    public async Task<RefreshToken?> GetByTokenAsync(Guid userId, string token, CancellationToken cancellationToken)
    {
        return await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.Token == token)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task RevokeAllByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var refreshTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId)
            .ToListAsync(cancellationToken);

        _context.RefreshTokens.RemoveRange(refreshTokens);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    #endregion
}