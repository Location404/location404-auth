using UserIdentity.Domain.Entities;

namespace UserIdentity.Domain.Interfaces;

public interface IUserApplicationService
{
    Task<UserApplication> CreateUserPasswordAsync(string username, string emailAddress, string passwordHash, string passwordSalt);
    Task<UserApplication> CreateUserOAuthAsync(string username, string emailAddress, string provider, string providerId);
    Task<UserApplication> GetUserByIdAsync(Guid userId);
    Task<UserApplication> GetUserByUsernameAsync(string username);
    Task<UserApplication> GetUserByEmailAsync(string emailAddress);
}