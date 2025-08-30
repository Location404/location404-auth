using UserIdentityService.Domain.Entities;

namespace UserIdentityService.Application.Common.Interfaces;

public interface IUserRepository : IDisposable
{
    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task AddUserAsync(User user, CancellationToken cancellationToken = default);
    bool UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);


    #region [RefreshToken]

    void AddRefreshToken(RefreshToken token, CancellationToken cancellationToken);
    Task<RefreshToken?> GetByTokenAsync(Guid userId, string token, CancellationToken cancellationToken);
    Task RevokeAllByUserAsync(Guid userId, CancellationToken cancellationToken);

    #endregion
}