namespace Location404.Auth.Application.Features.UserManagement.Queries.GetCurrentUserInformation;

public record GetCurrentUserInformationQueryResponse(
    Guid Id,
    string Username,
    string Email,
    string ProfileImage
);