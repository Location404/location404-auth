namespace UserIdentity.API.Endpoints;

public static class AuthenticationEndpoint
{
    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoint)
    {
        var group = endpoint.MapGroup("/api/v1/auth").WithTags("Authentication");
    }
}