using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using UserIdentity.Domain.Entities;
using UserIdentity.Domain.Interfaces;
using UserIdentity.Infra.Configurations;
using UserIdentity.Infra.Context;

namespace UserIdentity.Infra.Services;

public class AuthService(UserIdentityContext dbContext, IOptions<JwtConfiguration> jwtConfig) : IAuthService
{
    private readonly UserIdentityContext _dbContext = dbContext;
    private readonly JwtConfiguration _jwtConfig = jwtConfig.Value;

    public async Task<AuthResult> RegisterAsync(string username, string email, string displayName, string password)
    {
        if (await _dbContext.Users.AnyAsync(u => u.Username == username || u.EmailAddress == email))
        {
            return new AuthResult(
                Success: false,
                ErrorMessage: "Username or email already exists");
        }

        CreatePasswordHash(password, out string passwordHash, out string passwordSalt);
        var user = new User(username, email, displayName, passwordHash, passwordSalt);

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        await _dbContext.SaveChangesAsync();

        return new AuthResult(
            Success: true,
            Token: token,
            RefreshToken: refreshToken,
            Expiration: DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationInMinutes),
            User: user
        );
    }

    public async Task<AuthResult> LoginAsync(string usernameOrEmail, string password)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.EmailAddress == usernameOrEmail);
        ArgumentNullException.ThrowIfNull(user, nameof(user));

        if (user is null)
        {
            return new AuthResult
            (
                Success: false,
                ErrorMessage: "Invalid credentials"
            );
        }

        if (user.IsLockedOut())
        {
            return new AuthResult(
                Success: false,
                ErrorMessage: "Account is locked. Try again later."
            );
        }

        if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
        {
            user.RecordFailedLogin(5, TimeSpan.FromMinutes(15));
            await _dbContext.SaveChangesAsync();

            return new AuthResult(
                Success: false,
                ErrorMessage: "Invalid credentials"
            );
        }

        user.RecordSuccessfulLogin();

        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        await _dbContext.SaveChangesAsync();

        return new AuthResult(
            Success: true,
            Token: token,
            RefreshToken: refreshToken,
            Expiration: DateTime.UtcNow.AddHours(1),
            User: user
        );
    }

    private string GenerateJwtToken(User user)
    {
        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.EmailAddress),
            new("DisplayName", user.DisplayName)
        ];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationInMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
    {
        using var hmac = new HMACSHA512();
        var saltBytes = hmac.Key;
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        passwordSalt = Convert.ToBase64String(saltBytes);
        passwordHash = Convert.ToBase64String(hashBytes);
    }

    private bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
    {
        var saltBytes = Convert.FromBase64String(storedSalt);
        var hashBytes = Convert.FromBase64String(storedHash);

        using var hmac = new HMACSHA512(saltBytes);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != hashBytes[i]) return false;
        }

        return true;
    }

    public Task<AuthResult> RefreshTokenAsync(string token, string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RevokeTokenAsync(string username)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetUserByIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetUserByUsernameAsync(string username)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetUserByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }
}