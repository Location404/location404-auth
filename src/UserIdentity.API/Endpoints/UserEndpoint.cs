namespace UserIdentity.API.Endpoints;

public static class UserEndpoint
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder endpoint)
    {
        var group = endpoint.MapGroup("/api/v1/user").WithTags("User");
    }
}