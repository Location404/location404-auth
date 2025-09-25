using System.Security.Claims;

using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Mvc;
using UserIdentityService.API.Filters;
using UserIdentityService.Application.Common.Result;
using UserIdentityService.Application.Features.Authentication.Commands.AuthenticateUserWithPasswordCommand;
using UserIdentityService.Application.Features.Authentication.Commands.RefreshTokenCommand;

namespace UserIdentityService.API.Endpoints;

public static class AuthenticationEndpoints
{
    public record UserLoginResponse(string Id, string Username, string Email, string ProfileImageUrl);

    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/login", HandleLoginWithPassword)
            .WithName("LoginWithPassword")
            .WithDescription("Authenticate a user with email and password")
            .WithTags("Authentication")
            .Produces<AuthenticateUserWithPasswordCommandResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .AddEndpointFilter<ValidationFilter<AuthenticateUserWithPasswordCommand>>();

        group.MapPost("/refresh", HandleRefreshToken)
            .WithName("RefreshToken")
            .WithDescription("Refresh JWT using a valid refresh token cookie")
            .WithTags("Authentication")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
    }

    #region [Endpoints Handlers]

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
            return Results.Unauthorized();
        }

        var accessTokenOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddMinutes(15)
        };

        var refreshTokenOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            Path = "/api/auth/refresh"
        };

        httpContext.Response.Cookies.Append("accessToken", result.Value.AccessToken, accessTokenOptions);
        httpContext.Response.Cookies.Append("refreshToken", result.Value.RefreshToken, refreshTokenOptions);

        return Results.Ok(result.Value);
    }

    private static async Task<IResult> HandleRefreshToken(
        [FromServices] ICommandMediator mediator,
        HttpContext httpContext,
        [FromServices] ILogger<RefreshTokenCommand> logger,
        ClaimsPrincipal user)
    {
        var refreshTokenFromCookie = httpContext.Request.Cookies["refreshToken"];
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
        {
            return Results.Unauthorized();
        }

        if (string.IsNullOrEmpty(refreshTokenFromCookie))
        {
            return Results.Unauthorized();
        }

        var command = new RefreshTokenCommand(userGuid, refreshTokenFromCookie);    
        var result = await mediator.SendAsync(command);

        if (result.IsFailure && result.Error.Type == ErrorType.UnAuthenticated)
        {
            logger.LogWarning("Token refresh failed: {Error}", result.Error);
            return Results.Unauthorized();
        }

        if (result.IsFailure)
        {
            logger.LogError("Token refresh error: {Error}", result.Error);
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }

        var newAccessTokenOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddMinutes(15)
        };

        var newRefreshTokenOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            Path = "/api/auth/refresh"
        };
        
        httpContext.Response.Cookies.Append("accessToken", result.Value.AccessToken, newAccessTokenOptions);
        httpContext.Response.Cookies.Append("refreshToken", result.Value.RefreshToken, newRefreshTokenOptions);

        return Results.Ok();
    }

    #endregion
}