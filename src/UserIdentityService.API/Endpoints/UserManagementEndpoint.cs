using LiteBus.Commands.Abstractions;
using UserIdentityService.API.Filters;
using UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithExternalProviderCommand;
using UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithPasswordCommand;

namespace UserIdentityService.API.Endpoints;

public static class UserManagement
{
    /// <summary>
    /// Registers user management API endpoints on the provided <see cref="IEndpointRouteBuilder"/>.
    /// </summary>
    /// <remarks>
    /// Adds a route group at "/api/user" and registers:
    /// - POST /create/password: creates a user with email and password; validates <see cref="CreateUserWithPasswordCommand"/> and produces <see cref="CreateUserWithPasswordCommandResponse"/> on success.
    /// - POST /create/external-provider: creates a user via an external provider; validates <see cref="CreateUserWithExternalProviderCommand"/> and produces <see cref="CreateUserWithPasswordCommandResponse"/> on success.
    /// </remarks>
    public static void MapUserManagementEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/user");

        group.MapPost("/create/password", HandleCreateUserWithPassword)
            .WithDescription("Create a user with email and password")
            .Produces<CreateUserWithPasswordCommandResponse>(StatusCodes.Status200OK)
            .AddEndpointFilter<ValidationFilter<CreateUserWithPasswordCommand>>();

        group.MapPost("/create/external-provider", HandleCreateUserWithExternalProvider)
            .WithDescription("Create a user with an external provider")
            .Produces<CreateUserWithPasswordCommandResponse>(StatusCodes.Status200OK)
            .AddEndpointFilter<ValidationFilter<CreateUserWithExternalProviderCommand>>();
    }

    #region [Endpoints Handlers]

    /// <summary>
    /// Handles a request to create a user with an email and password by dispatching the command and translating the result to an HTTP response.
    /// </summary>
    /// <param name="command">The command containing the user's email and password used to create the account.</param>
    /// <returns>An <see cref="IResult"/> that is 200 OK with the command response on success, or 400 Bad Request with the error on failure.</returns>
    private static async ValueTask<IResult> HandleCreateUserWithPassword(CreateUserWithPasswordCommand command, ICommandMediator mediator)
    {
        var result = await mediator.SendAsync(command);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Error);
    }

    /// <summary>
    /// Handles creation of a user via an external authentication provider by dispatching the provided command to the application mediator and returning an HTTP result.
    /// </summary>
    /// <param name="command">Command containing the external-provider user creation data (e.g., provider info and user details).</param>
    /// <returns>
    /// An <see cref="IResult"/> representing the HTTP response:
    ///  - 200 OK with the command response value on success.
    ///  - 400 Bad Request with the error on failure.
    /// </returns>
    private static async ValueTask<IResult> HandleCreateUserWithExternalProvider(CreateUserWithExternalProviderCommand command, ICommandMediator mediator)
    {
        var result = await mediator.SendAsync(command);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Error);
    }

    #endregion
}