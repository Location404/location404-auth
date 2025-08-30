using System.ComponentModel.DataAnnotations;
using LiteBus.Commands.Abstractions;
using UserIdentityService.Application.Common.Result;

namespace UserIdentityService.Application.Features.Authentication.Commands.RefreshTokenCommand;

public record RefreshTokenCommand(
    [Required] Guid UserId,
    [Required] string RefreshToken) : ICommand<Result<RefreshTokenCommandResponse>>;