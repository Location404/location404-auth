using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserIdentity.Application.Features.UserManagement.Commands.RegisterUser;

namespace UserIdentity.API.Endpoints;

public static class UserEndpoint
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder endpoint)
    {
        var group = endpoint.MapGroup("/api/v1/user").WithTags("User");

        group.MapPost("/register", async ([FromServices]IMediator mediator, [FromBody]RegisterUserCommand command) =>
        {
            var result = await mediator.Send(command);
            return result.IsSuccess ? Results.Created($"/api/v1/user/{result.Value}", result.Value) : Results.BadRequest(result.Error);
        });
    }
}