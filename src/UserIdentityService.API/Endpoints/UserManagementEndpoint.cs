using System.ComponentModel.DataAnnotations;
using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Mvc;
using UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithPasswordCommand;

namespace UserIdentityService.API.Endpoints;

public static class UserManagement
{
    public static void MapUserManagementEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/user");

        group.MapPost("/create-with-password", async (CreateUserWithPasswordCommand command, [FromServices] ICommandMediator mediator) =>
        {
            if (!ValidateModel(command, out var errorResult))
                return errorResult!;

            var result = await mediator.SendAsync(command);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        });
    }

    #region [private methods]
    private static bool ValidateModel(CreateUserWithPasswordCommand command, out IResult? errorResult)
    {
        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(command, new ValidationContext(command), validationResults, true))
        {
            var errors = validationResults
                .GroupBy(v => v.MemberNames.FirstOrDefault()!)
                .ToDictionary(
                    g => g.Key.ToLower(),
                    g => g.Select(v => v.ErrorMessage!).ToArray()
                );

            errorResult = Results.ValidationProblem(errors);
            return false;
        }

        errorResult = null;
        return true;
    }
    #endregion
}