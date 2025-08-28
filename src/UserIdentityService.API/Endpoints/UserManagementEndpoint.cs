using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Mvc;

using UserIdentityService.API.Filters;
using UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithExternalProviderCommand;
using UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithPasswordCommand;

namespace UserIdentityService.API.Endpoints;

public static class UserManagement
{
    public static void MapUserManagementEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/user");

        group.MapPost("/create/password", HandleCreateUserWithPassword)
            .WithDescription("Create a user with email and password")
            .Produces<CreateUserWithPasswordCommandResponse>(StatusCodes.Status200OK)
            .AddEndpointFilter<ValidationFilter<CreateUserWithPasswordCommand>>();

        group.MapPost("/create/external-provider", HandleCreateUserWithExternalProvider)
            .WithDescription("Create a user with an external provider")
            .Produces<CreateUserWithPasswordCommandResponse>(StatusCodes.Status200OK)
            .AddEndpointFilter<ValidationFilter<CreateUserWithExternalProviderCommand>>();
    }

    #region [Endpoints Handlers]

    // Create User with Password
    private static async ValueTask<IResult> HandleCreateUserWithPassword(
        [FromBody] CreateUserWithPasswordCommand command,
        [FromServices] ICommandMediator mediator,
        [FromServices] ILogger<CreateUserWithPasswordCommand> logger)
    {
        logger.LogInformation("Creating user with email: {Email} and username: {Username}", command.Email, command.Username);

        var result = await mediator.SendAsync(command);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Error);
    }

    // Create User with External Provider
    private static async ValueTask<IResult> HandleCreateUserWithExternalProvider(
        [FromBody] CreateUserWithExternalProviderCommand command,
        [FromServices] ICommandMediator mediator,
        [FromServices] ILogger<CreateUserWithExternalProviderCommand> logger)
    {
        logger.LogInformation("Creating user with external provider: {Provider}", command.Provider);

        var result = await mediator.SendAsync(command);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Error);
    }

    #endregion
}