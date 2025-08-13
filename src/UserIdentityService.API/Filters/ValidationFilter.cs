namespace UserIdentityService.API.Filters;

using System.ComponentModel.DataAnnotations;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
    /// <summary>
    /// Validates the first endpoint argument of type <typeparamref name="T"/> using DataAnnotations and either short-circuits with a validation problem response or invokes the next filter.
    /// </summary>
    /// <param name="context">The current endpoint filter invocation context; the filter looks for the first argument assignable to <typeparamref name="T"/>.</param>
    /// <param name="next">The next endpoint filter delegate to invoke when validation succeeds.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> that yields either an <see cref="IResult"/> produced by <c>Results.ValidationProblem</c> when validation fails, or the result of invoking <paramref name="next"/> when validation succeeds.
    /// </returns>
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
