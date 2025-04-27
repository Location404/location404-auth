
using Microsoft.EntityFrameworkCore;
using UserIdentity.Application.Common.Extensions;
using UserIdentity.Application.Features.UserManagement;
using UserIdentity.Application.Features.UserManagement.DTOs;
using UserIdentity.Domain.Entities;
using UserIdentity.Infra.Context;

namespace UserIdentity.Infra.Persistence;

public class UserRepository(UserIdentityContext context) : IUserRepository
{
    private readonly DbSet<UserApplication> _userDbSet = context.Set<UserApplication>();

    public async Task AddAsync(UserApplication user)
    {
       await _userDbSet.AddAsync(user);
    }

    public async Task<bool> ExistsByUsernameOrEmailAsync(string username, string email)
    {
        return await _userDbSet.AnyAsync(u => u.Username == username || u.EmailAddress == email);
    }

    public async Task<UserApplicationDto?> GetByEmailAsync(string email)
    {
        var user = await _userDbSet.SingleOrDefaultAsync(u => u.EmailAddress == email);
        return user?.ToDto();
    }

    public async Task<UserApplicationDto?> GetByIdAsync(Guid id)
    {
        var user = await _userDbSet.SingleOrDefaultAsync(u => u.Id == id);
        return user?.ToDto();
    }

    public async Task<UserApplicationDto?> GetByUsernameAsync(string username)
    {
        var user = await _userDbSet.SingleOrDefaultAsync(u => u.Username == username);
        return user?.ToDto();
    }
}