using LiteBus.Commands.Abstractions;

using Microsoft.AspNetCore.Mvc;

using UserIdentityService.API.Filters;
using UserIdentityService.Application.Common.Result;
using UserIdentityService.Application.Features.Authentication.Commands.AuthenticateUserWithPasswordCommand;
using UserIdentityService.Application.Features.Authentication.Commands.RefreshTokenCommand;

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
        // .ProducesValidationProblem(StatusCodes.Status400BadRequest)
        // .AddEndpointFilter<ValidationFilter<LoginWithExternalProviderCommand>>();

        // POST /api/auth/refresh - Refresh JWT using refresh token
        group.MapPost("/refresh", HandleRefreshToken)
            .WithName("RefreshToken")
            .WithDescription("Refresh JWT using a valid refresh token")
            .WithTags("Authentication")
            .Produces<RefreshTokenCommandResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .AddEndpointFilter<ValidationFilter<RefreshTokenCommand>>();
    }

    #region [Endpoints Handlers]

    /// <summary>
    /// Authenticates a user with email and password
    /// </summary>
    private static async Task<IResult> HandleLoginWithPassword(
        [FromBody] AuthenticateUserWithPasswordCommand command,
        [FromServices] ICommandMediator mediator,
        HttpContext httpContext,
        [FromServices] ILogger<AuthenticateUserWithPasswordCommand> logger)
    {
        var result = await mediator.SendAsync(command);

        if (result.IsFailure)
        {
            logger.LogWarning("Authentication failed: {Error}", result.Error);
            return Results.BadRequest(result.Error);
        }

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        };

        httpContext.Response.Cookies.Append("accessToken", result.Value.AccessToken, cookieOptions);
        httpContext.Response.Cookies.Append("refreshToken", result.Value.RefreshToken, cookieOptions);
        httpContext.Response.Cookies.Append("refreshTokenExpiresAt", result.Value.RefreshTokenExpiresAt.ToString(), cookieOptions);

        return Results.Ok(result.Value);
    }

    private static async Task<IResult> HandleRefreshToken(
        [FromBody] RefreshTokenCommand command,
        [FromServices] HttpContext httpContext,
        [FromServices] ICommandMediator mediator,
        [FromServices] ILogger<RefreshTokenCommand> logger)
    {
        var result = await mediator.SendAsync(command);

        if (result.IsFailure)
        {
            logger.LogWarning("Token refresh failed: {Error}", result.Error);
            return Results.BadRequest(result.Error);
        }

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        };

        httpContext.Response.Cookies.Append("refreshToken", result.Value.RefreshToken, cookieOptions);
        httpContext.Response.Cookies.Append("refreshTokenExpiresAt", result.Value.RefreshTokenExpiresAt.ToString(), cookieOptions);

        return Results.Ok(result.Value);

    }

    #endregion
}