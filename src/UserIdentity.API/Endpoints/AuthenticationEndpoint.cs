using MediatR;
using Microsoft.AspNetCore.Mvc;

using UserIdentity.Application.Features.Authentication.Commands.RegisterUser;

namespace UserIdentity.API.Endpoints;

public static class AuthenticationEndpoint
{
    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoint)
    {
        var group = endpoint.MapGroup("/api/v1/auth").WithTags("Authentication");

        group.MapPost("/register", async ([FromServices] ISender mediator, [FromBody] RegisterUserCommand command) =>
        {
            var result = await mediator.Send(command);

            return result.IsSuccess
                ? Results.Created($"/api/v1/user/{result.Value!.UserId}", result.Value)
                : Results.BadRequest(result.Error);
        });
    }
}