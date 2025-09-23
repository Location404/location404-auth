namespace UserIdentityService.Application.Features.UserManagement.Queries.GetCurrentUserInformation;

public record GetCurrentUserInformationQueryResponse(
    Guid Id,
    string Username,
    string Email,
    byte[]? ProfileImage
);