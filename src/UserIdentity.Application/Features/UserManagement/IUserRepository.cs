using UserIdentity.Application.Features.UserManagement.DTOs;
using UserIdentity.Domain.Entities;

namespace UserIdentity.Application.Features.UserManagement;

public interface IUserRepository
{
    Task<UserDto> GetByIdAsync(Guid id);
    Task<UserDto> GetByUsernameAsync(string username);
    Task<bool> ExistsByUsernameOrEmailAsync(string username, string email);

    Task AddAsync(UserApplication user);
}