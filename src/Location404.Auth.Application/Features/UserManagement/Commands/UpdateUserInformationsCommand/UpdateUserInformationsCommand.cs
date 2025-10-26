using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

using LiteBus.Commands.Abstractions;

using Microsoft.AspNetCore.Http;

using Location404.Auth.Application.Common.Result;

namespace Location404.Auth.Application.Features.UserManagement.Commands.UpdateUserInformationsCommand;

public class UpdateUserInformationsCommand : ICommand<Result<UpdateUserInformationsCommandResponse>>
{
    public required Guid Id { get; set; }

    [EmailAddress]
    public string? Email { get; set; }
    public string? Username { get; set; }
    [AllowNull]
    public string? Password { get; set; }
    public IFormFile? ProfileImage { get; set; }
}