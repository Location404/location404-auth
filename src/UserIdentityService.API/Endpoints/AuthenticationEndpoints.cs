using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Mvc;

using UserIdentityService.API.Filters;
using UserIdentityService.Application.Features.Authentication.Commands.AuthenticateUserWithPasswordCommand;

namespace UserIdentityService.API.Endpoints;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        // POST /api/auth/login - Login with email and password
        group.MapPost("/login", HandleLoginWithPassword)
            .WithName("LoginWithPassword")
            .WithDescription("Authenticate a user with email and password")
            .WithTags("Authentication")
            .Produces<AuthenticateUserWithPasswordCommandResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .AddEndpointFilter<ValidationFilter<AuthenticateUserWithPasswordCommand>>();

        // // POST /api/auth/external - Login with external provider
        // group.MapPost("/external", HandleLoginWithExternalProvider)
        //     .WithName("LoginWithExternalProvider")
        //     .WithDescription("Authenticate a user with an external authentication provider")
        //     .WithTags("Authentication")
        //     .Produces<LoginWithExternalProviderCommandResponse>(StatusCodes.Status200OK)
        //     .ProducesValidationProblem(StatusCodes.Status400BadRequest)
        //     .AddEndpointFilter<ValidationFilter<LoginWithExternalProviderCommand>>();

        // // POST /api/auth/refresh - Refresh JWT using refresh token
        // group.MapPost("/refresh", HandleRefreshToken)
        //     .WithName("RefreshToken")
        //     .WithDescription("Refresh JWT using a valid refresh token")
        //     .WithTags("Authentication")
        //     .Produces<RefreshTokenCommandResponse>(StatusCodes.Status200OK)
        //     .ProducesValidationProblem(StatusCodes.Status400BadRequest)
        //     .AddEndpointFilter<ValidationFilter<RefreshTokenCommand>>();
    }

    #region [Endpoints Handlers]

    /// <summary>
    /// Authenticates a user with email and password
    /// </summary>
    private static async Task<IResult> HandleLoginWithPassword(
        [FromBody] AuthenticateUserWithPasswordCommand command,
        [FromServices] ICommandMediator mediator,
        [FromServices] ILogger<AuthenticateUserWithPasswordCommand> logger)
    {
        logger.LogInformation("Logging in user with email: {Email}", command.Email);

        var result = await mediator.SendAsync(command);

        if (result.IsSuccess)
        {
            return Results.Ok(result.Value);
        }

        return Results.BadRequest(result.Error);
    }

    #endregion
}