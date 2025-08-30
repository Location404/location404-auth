using System.ComponentModel.DataAnnotations;
using LiteBus.Commands.Abstractions;

using UserIdentityService.Application.Common.Result;

namespace UserIdentityService.Application.Features.Authentication.Commands.AuthenticateUserWithPasswordCommand;

public record AuthenticateUserWithPasswordCommand(
    [EmailAddress] string Email,
    [Required, MinLength(6)] string Password) : ICommand<Result<AuthenticateUserWithPasswordCommandResponse>>;