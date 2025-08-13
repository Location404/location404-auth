using System.ComponentModel.DataAnnotations;

namespace UserIdentityService.API.Common;

public static class ValidateModel
{
    public static bool Validate<T>(T model, out IResult? errorResult) where T : class
    {
        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true))
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
}