namespace Location404.Auth.Application.Common.Result;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
        {
            throw new InvalidOperationException();
        }

        if (!isSuccess && error == Error.None)
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result<TValue> Success<TValue>(TValue value) => Result<TValue>.Success(value);
    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException();

    protected Result(TValue value) : base(true, Error.None) => _value = value;
    protected Result(Error error) : base(false, error) => _value = default;
    
    public static Result<TValue> Success(TValue value) => new(value);
    public static new Result<TValue> Failure(Error error) => new(error);

    /// <summary>
    /// Executa uma função com base no estado do resultado (Sucesso ou Falha).
    /// </summary>
    public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<Error, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(Value) : onFailure(Error);
    }
}