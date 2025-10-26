using System.ComponentModel.DataAnnotations;
using LiteBus.Commands.Abstractions;

using Location404.Auth.Application.Common.Result;

namespace Location404.Auth.Application.Features.Authentication.Commands.AuthenticateUserWithPasswordCommand;

public record AuthenticateUserWithPasswordCommand(
    [EmailAddress] string Email,
    [Required, MinLength(6)] string Password) : ICommand<Result<AuthenticateUserWithPasswordCommandResponse>>;