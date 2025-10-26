namespace Location404.Auth.API.Filters;

using System.ComponentModel.DataAnnotations;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var model = context.Arguments.FirstOrDefault(a => a is T) as T;

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model!);

        if (!Validator.TryValidateObject(model!, validationContext, validationResults, true))
        {
            var errors = validationResults
                .GroupBy(v => v.MemberNames.FirstOrDefault()!)
                .ToDictionary(
                    g => g.Key.ToLower(),
                    g => g.Select(v => v.ErrorMessage!).ToArray()
                );

            return Results.ValidationProblem(errors);
        }

        return await next(context);
    }
}
