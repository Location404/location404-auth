using UserIdentity.Application.Features.UserManagement.DTOs;
using UserIdentity.Domain.Entities;

namespace UserIdentity.Application.Features.UserManagement;

public interface IUserRepository
{
    Task<UserApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserApplication?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<UserApplication?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserApplication?> GetByUsernameOrEmailAsync(string? username = null, string? email = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUsernameOrEmailAsync(string? username = null, string? email = null, CancellationToken cancellationToken = default);

    Task AddAsync(UserApplication user, CancellationToken cancellationToken = default);
}