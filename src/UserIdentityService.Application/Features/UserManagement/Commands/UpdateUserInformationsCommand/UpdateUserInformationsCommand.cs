using System.ComponentModel.DataAnnotations;
using LiteBus.Commands.Abstractions;
using UserIdentityService.Application.Common.Result;

namespace UserIdentityService.Application.Features.UserManagement.Commands.UpdateUserInformationsCommand;

public class UpdateUserInformationsCommand : ICommand<Result<UpdateUserInformationsCommandResponse>>
{
    public required Guid Id { get; set; }

    [EmailAddress]
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}