using Location404.Auth.Domain.Entities;

namespace Location404.Auth.Application.Common.Interfaces;

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

    void AddRefreshToken(RefreshToken token, CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetByTokenAsync(Guid userId, string token, CancellationToken cancellationToken = default);
    Task RevokeAllByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    #endregion
}