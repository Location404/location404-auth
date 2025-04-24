
using UserIdentity.Application.Features.UserApplicationManagement.DTOs;

namespace UserIdentity.Application.Features.UserApplicationManagement;

public interface IUserApplicationReadRepository
{
    Task<UserApplicationDto> GetByIdAsync(Guid id);
    Task<UserApplicationDto> GetByUsernameAsync(string username);
    Task<bool> ExistsByUsernameOrEmailAsync(string username, string email);
}