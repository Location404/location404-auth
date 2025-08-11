using System.ComponentModel.DataAnnotations;

namespace UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithPasswordCommand;

public sealed class CreateUserWithPasswordCommand
{
    [EmailAddress, Required]
    public required string Email { get; set; }
    [MaxLength(24), MinLength(3), Required]
    public required string Username { get; set; }
    [MinLength(6), Required]
    public required string Password { get; set; }
}