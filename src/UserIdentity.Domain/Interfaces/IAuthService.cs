using UserIdentity.Domain.Entities;

namespace UserIdentity.Domain.Interfaces;

public record AuthResult(bool Success, string? Token = null, string? RefreshToken = null, DateTime? Expiration = null, User? User = null, string? ErrorMessage = null);
public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string username, string email, string displayName, string password);
    Task<AuthResult> LoginAsync(string usermameOrEmail, string password);
    Task<AuthResult> RefreshTokenAsync(string token, string refreshToken);
    Task<bool> RevokeTokenAsync(string username);
    Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
    Task<User> GetUserByIdAsync(Guid userId);
    Task<User> GetUserByUsernameAsync(string username);
    Task<User> GetUserByEmailAsync(string email);
}

