using System.Security.Cryptography;
using System.Text;

using UserIdentity.Application.Features.Authentication.Interfaces;

namespace UserIdentity.Infra.Services;

public class PasswordService : IPasswordService
{
    public (string hash, string salt) CreatePasswordHash(string password)
    {
        using var hmac = new HMACSHA512();
        var salt = Convert.ToBase64String(hmac.Key);
        var hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        
        return (hash, salt);
    }

    public bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
    {
        var saltBytes = Convert.FromBase64String(storedSalt);
        using var hmac = new HMACSHA512(saltBytes);
        var computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        
        return computedHash == storedHash;
    }
}