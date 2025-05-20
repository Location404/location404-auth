using MediatR;
using UserIdentity.Application.Features.Authentication.Commands.LoginUser;
using UserIdentity.Application.Features.Authentication.Commands.RegisterUser;

namespace UserIdentity.API.Endpoints;

public static class AuthenticationEndpoint
{
    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoint)
    {
        var group = endpoint.MapGroup("/api/v1/auth").WithTags("Authentication");

        group.MapPost("/register", RegisterUserHandler).AllowAnonymous();

        group.MapPost("/login", LoginUserHandler).RequireAuthorization();


        #region [Endpoints Handlers]

        static async Task<IResult> RegisterUserHandler(ISender mediator, RegisterUserCommand command)
        {
            const string uri = "/api/v1/auth/register/";
            var result = await mediator.Send(command);

            return result.IsSuccess
                ? Results.Created(uri + result.Value?.UserId, result.Value)
                : Results.Problem(title: result.Error?.Message, statusCode: result.Error?.StatusCode);
        }
        
        static async Task<IResult> LoginUserHandler(ISender mediator, LoginUserCommand command)
        {
            var result = await mediator.Send(command);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(title: result.Error?.Message, statusCode: result.Error?.StatusCode);
        }

        #endregion
    }
}