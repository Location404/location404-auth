using System.ComponentModel.DataAnnotations;
using LiteBus.Commands.Abstractions;
using Location404.Auth.Application.Common.Result;

namespace Location404.Auth.Application.Features.Authentication.Commands.RefreshTokenCommand;

public record RefreshTokenCommand(
    [Required] Guid UserId,
    [Required] string RefreshToken) : ICommand<Result<RefreshTokenCommandResponse>>;