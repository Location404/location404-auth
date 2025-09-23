using System.IdentityModel.Tokens.Jwt;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;

using Microsoft.AspNetCore.Mvc;

using UserIdentityService.API.Filters;
using UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithExternalProviderCommand;
using UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithPasswordCommand;
using UserIdentityService.Application.Features.UserManagement.Commands.UpdateUserInformationsCommand;
using UserIdentityService.Application.Features.UserManagement.Queries.GetCurrentUserInformation;

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

        // GET /api/me - Get current user information
        group.MapGet("/me", HandleGetCurrentUser)
            .WithName("GetCurrentUser")
            .WithDescription("Get information about the currently authenticated user")
            .WithTags("Users")
            .Produces<GetCurrentUserInformationQueryResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        // PATCH /api/users/{id} - Update user
        group.MapPatch("/{id}", HandleUpdateUser)
            .WithName("UpdateUser")
            .WithDescription("Update an existing user's information")
            .WithTags("Users")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
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

    private static async Task<IResult> HandleGetCurrentUser(
        [FromServices] IQueryMediator query,
        [FromServices] ILogger logger,
        HttpContext context)
    {
        if (!Guid.TryParse(context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, out var subClaim))
        {
            logger.LogWarning("Invalid or missing 'sub' claim in JWT token.");
            return Results.Unauthorized();
        }

        var result = await query.QueryAsync(new GetCurrentUserInformationQuery(subClaim));

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(result.Error);
    }

    private static async Task<IResult> HandleUpdateUser(
        [FromBody] UpdateUserInformationsCommand command,
        [FromServices] ICommandMediator mediator,
        [FromServices] ILogger<UpdateUserInformationsCommand> logger)
    {
        logger.LogInformation("Updating user information for user: {UserId}", command.Id);

        var result = await mediator.SendAsync(command);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(result.Error);
    }

    #endregion
}