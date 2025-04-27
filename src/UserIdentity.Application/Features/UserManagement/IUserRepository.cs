using UserIdentity.Application.Features.UserManagement.DTOs;
using UserIdentity.Domain.Entities;

namespace UserIdentity.Application.Features.UserManagement;

public interface IUserRepository
{
    Task<UserApplicationDto?> GetByIdAsync(Guid id);
    Task<UserApplicationDto?> GetByUsernameAsync(string username);
    Task<UserApplicationDto?> GetByEmailAsync(string email);
    Task<bool> ExistsByUsernameOrEmailAsync(string username, string email);

    Task AddAsync(UserApplication user);
}