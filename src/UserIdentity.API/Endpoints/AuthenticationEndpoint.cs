using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserIdentity.Application.Features.Authentication.Commands.LoginUser;
using UserIdentity.Application.Features.Authentication.Commands.RegisterUser;

namespace UserIdentity.API.Endpoints;

public static class AuthenticationEndpoint
{
    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoint)
    {
        var group = endpoint.MapGroup("/api/v1/auth").WithTags("Authentication");

        group.MapPost("/register", async ([FromServices] ISender mediator, [FromBody] RegisterUserCommand command) =>
        {
            const string uri = "/api/v1/auth/register/";
            var result = await mediator.Send(command);

            return result.IsSuccess
                ? Results.Created(uri + result.Value?.UserId, result.Value)
                : Results.Problem(title: result.Error?.Message, statusCode: result.Error?.StatusCode);
        });

        group.MapPost("/login", async ([FromServices] ISender mediator, [FromBody] LoginUserCommand command) =>
        {
            var result = await mediator.Send(command);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(title: result.Error?.Message, statusCode: result.Error?.StatusCode);
        });
    }
}