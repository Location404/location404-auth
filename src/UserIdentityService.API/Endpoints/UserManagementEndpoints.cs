using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Mvc;
using UserIdentityService.API.Filters;
using UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithExternalProviderCommand;
using UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithPasswordCommand;

namespace UserIdentityService.API.Endpoints;

public static class UserManagementEndpoints
{
    public static void MapUserManagementEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users");

        // POST /api/users - Create user with password
        group.MapPost("/", HandleCreateUserWithPassword)
            .WithName("CreateUser")
            .WithDescription("Create a new user with email and password")
            .WithTags("Users")
            .Produces<CreateUserWithPasswordCommandResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .AddEndpointFilter<ValidationFilter<CreateUserWithPasswordCommand>>();

        // POST /api/users/external - Create user with external provider
        group.MapPost("/external", HandleCreateUserWithExternalProvider)
            .WithName("CreateUserWithExternalProvider")
            .WithDescription("Create a new user with an external authentication provider")
            .WithTags("Users")
            .Produces<CreateUserWithExternalProviderCommandResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .AddEndpointFilter<ValidationFilter<CreateUserWithExternalProviderCommand>>();
    }

    #region [Endpoints Handlers]

    /// <summary>
    /// Creates a new user with email and password authentication
    /// </summary>
    private static async Task<IResult> HandleCreateUserWithPassword(
        [FromBody] CreateUserWithPasswordCommand command,
        [FromServices] ICommandMediator mediator,
        [FromServices] ILogger<CreateUserWithPasswordCommand> logger,
        HttpContext context)
    {
        logger.LogInformation("Creating user with email: {Email} and username: {Username}", 
            command.Email, command.Username);

        var result = await mediator.SendAsync(command);

        if (result.IsSuccess)
        {
            var locationUri = $"{context.Request.Scheme}://{context.Request.Host}/api/users/{result.Value.Id}";
            return Results.Created(locationUri, result.Value);
        }

        return Results.BadRequest(result.Error);
    }

    /// <summary>
    /// Creates a new user with external authentication provider
    /// </summary>
    private static async Task<IResult> HandleCreateUserWithExternalProvider(
        [FromBody] CreateUserWithExternalProviderCommand command,
        [FromServices] ICommandMediator mediator,
        [FromServices] ILogger<CreateUserWithExternalProviderCommand> logger,
        HttpContext context)
    {
        logger.LogInformation($"Creating user with external provider: {command.Provider} for user: {command.ProviderUserId}");

        var result = await mediator.SendAsync(command);

        if (result.IsSuccess)
        {
            var locationUri = $"{context.Request.Scheme}://{context.Request.Host}/api/users/{result.Value.Id}";
            return Results.Created(locationUri, result.Value);
        }

        return Results.BadRequest(result.Error);
    }

    #endregion
}