using System.ComponentModel.DataAnnotations;
using LiteBus.Commands.Abstractions;
using UserIdentityService.Application.Common.Result;

namespace UserIdentityService.Application.Features.UserManagement.Commands.UpdateUserInformationsCommand;

public class UpdateUserInformationsCommand : ICommand<Result<UpdateUserInformationsCommandResponse>>
{
    [Required]
    public required Guid Id { get; set; }

    [EmailAddress, Required]
    public required string Email { get; set; }
    [MaxLength(24), MinLength(3), Required]
    public required string Username { get; set; }
    [MinLength(6), Required]
    public required string Password { get; set; }
    public byte[]? ProfileImage { get; set; }
}