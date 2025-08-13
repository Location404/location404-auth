using System.ComponentModel.DataAnnotations;
using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Mvc;

using UserIdentityService.API.Common;
using UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithPasswordCommand;

namespace UserIdentityService.API.Endpoints;

public static class UserManagement
{
    public static void MapUserManagementEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/user");

        group.MapPost("/create-with-password", async (CreateUserWithPasswordCommand command, [FromServices] ICommandMediator mediator) =>
        {
            if (!ValidateModel.Validate(command, out var errorResult))
                return errorResult!;

            var result = await mediator.SendAsync(command);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        });
    }
}