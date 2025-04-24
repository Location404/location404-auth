using UserIdentity.Domain.Entities;

namespace UserIdentity.Application.Features.UserApplicationManagement;

public interface IUserApplicationWriteRepository
{
    Task<bool> ExistsByUsernameOrEmailAsync(string username, string email);
    Task AddAsync(UserApplication user);
    Task<UserApplication> GetByIdAsync(Guid id);
    Task<UserApplication> GetByUsernameOrEmailAsync(string usernameOrEmail);
    Task SaveChangesAsync();
}