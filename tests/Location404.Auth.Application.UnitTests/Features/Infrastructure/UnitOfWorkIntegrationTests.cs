using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Location404.Auth.Domain.Entities;
using Location404.Auth.Domain.ValueObjects;
using Location404.Auth.Infrastructure.Context;
using Location404.Auth.Infrastructure.Services;

namespace Location404.Auth.Application.UnitTests.Features.Infrastructure;

public class UnitOfWorkIntegrationTests : IDisposable
{
    private readonly UserIdentityDbContext _context;
    private readonly UserRepository _userRepository;
    private readonly UnitOfWork _unitOfWork;

    public UnitOfWorkIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<UserIdentityDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new UserIdentityDbContext(options);
        _context.Database.EnsureCreated();
        _userRepository = new UserRepository(_context);
        _unitOfWork = new UnitOfWork(_userRepository, _context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _userRepository.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void Users_ShouldReturnUserRepository()
    {
        var users = _unitOfWork.Users;

        users.Should().NotBeNull();
        users.Should().BeSameAs(_userRepository);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistChanges()
    {
        var user = User.Create(
            EmailAddress.Create("test@example.com"),
            "testuser",
            "hashedPassword");

        await _userRepository.AddUserAsync(user);
        var result = await _unitOfWork.SaveChangesAsync();

        result.Should().BeGreaterThan(0);

        var savedUser = await _context.Users.FindAsync(user.Id);
        savedUser.Should().NotBeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_WithNoChanges_ShouldReturnZero()
    {
        var result = await _unitOfWork.SaveChangesAsync();

        result.Should().Be(0);
    }

    [Fact]
    public async Task SaveChangesAsync_WithMultipleChanges_ShouldReturnCorrectCount()
    {
        var user1 = User.Create(
            EmailAddress.Create("user1@example.com"),
            "user1",
            "password1");

        var user2 = User.Create(
            EmailAddress.Create("user2@example.com"),
            "user2",
            "password2");

        await _userRepository.AddUserAsync(user1);
        await _userRepository.AddUserAsync(user2);

        var result = await _unitOfWork.SaveChangesAsync();

        result.Should().BeGreaterThanOrEqualTo(2);
    }
}
