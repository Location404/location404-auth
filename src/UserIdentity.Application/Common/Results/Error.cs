namespace UserIdentity.Application.Common.Results;

public record Error(int StatusCode, string Message = "")
{
    public static Error NotFound(string message = "Not Found") => new(404, message);
    public static Error BadRequest(string message = "Bad Request") => new(400, message);
    public static Error Unauthorized(string message = "Unauthorized") => new(401, message);
    public static Error Forbidden(string message = "Forbidden") => new(403, message);
    public static Error InternalServerError(string message = "Internal Server Error") => new(500, message);
    public static Error Conflict(string message = "Conflict") => new(409, message);
}