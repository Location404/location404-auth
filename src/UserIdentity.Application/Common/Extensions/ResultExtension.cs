using UserIdentity.Domain.Common.Results;

namespace UserIdentity.Application.Common.Extensions;

public static class ResultExtensions
{
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> mapper)
    {
        return result.IsSuccess
            ? Result<TOut>.Success(mapper(result.Value!))
            : Result<TOut>.Failure(result.Error!);
    }

    public static async Task<Result<TOut>> Map<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, TOut> mapper)
    {
        Result<TIn> result = await resultTask;
        return result.Map(mapper);
    }

    public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> func)
    {
        return result.IsSuccess
            ? func(result.Value!)
            : Result<TOut>.Failure(result.Error!);
    }

    public static Result<T> Tap<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess) action(result.Value!);
        return result;
    }
}