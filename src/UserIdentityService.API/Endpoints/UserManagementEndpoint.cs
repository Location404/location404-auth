using LiteBus.Commands.Abstractions;
using UserIdentityService.API.Filters;
using UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithPasswordCommand;

namespace UserIdentityService.API.Endpoints;

public static class UserManagement
{
    public static void MapUserManagementEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/user");

        group.MapPost("/create-with-password", HandleCreateUserWithPassword)
            .WithDescription("Create a user with email and password")
            .Produces<CreateUserWithPasswordCommandResponse>(StatusCodes.Status200OK)
            .AddEndpointFilter<ValidationFilter<CreateUserWithPasswordCommand>>();
    }

    #region [Endpoints Handlers]

    // Create User with Password
    private static async ValueTask<IResult> HandleCreateUserWithPassword(CreateUserWithPasswordCommand command, ICommandMediator mediator)
    {
        var result = await mediator.SendAsync(command);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Error);
    }

    #endregion
}