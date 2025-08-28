using System.ComponentModel.DataAnnotations;
using LiteBus.Commands.Abstractions;
using UserIdentityService.Application.Common.Result;

namespace UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithExternalProviderCommand;

public sealed class CreateUserWithExternalProviderCommand : ICommand<Result<CreateUserWithExternalProviderCommandResponse>>
{
    [EmailAddress, Required]
    public required string Email { get; set; }
    [MaxLength(24), MinLength(3), Required]
    public required string Username { get; set; }
    [Required]
    public required string Provider { get; set; }
    [Required]
    public required string ProviderUserId { get; set; }
}