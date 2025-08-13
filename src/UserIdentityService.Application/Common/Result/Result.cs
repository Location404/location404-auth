namespace UserIdentityService.Application.Common.Result;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    /// <summary>
    /// Initializes a Result with the given success flag and associated error, enforcing invariants.
    /// </summary>
    /// <param name="isSuccess">True for a successful result; false for a failure.</param>
    /// <param name="error">The error associated with a failure, or <c>Error.None</c> for success.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the provided arguments violate invariants:
    /// - <paramref name="isSuccess"/> is true but <paramref name="error"/> is not <c>Error.None</c>, or
    /// - <paramref name="isSuccess"/> is false but <paramref name="error"/> is <c>Error.None</c>.
    /// </exception>
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

    /// <summary>
/// Creates a successful <see cref="Result"/> instance with no error.
/// </summary>
/// <returns>A <see cref="Result"/> representing success (its <see cref="Error"/> is <c>Error.None</c>).</returns>
public static Result Success() => new(true, Error.None);
    /// <summary>
/// Creates a failed <see cref="Result"/> containing the specified <paramref name="error"/>.
/// </summary>
/// <param name="error">The error describing the failure; must not be <see cref="Error.None"/>.</param>
/// <returns>A <see cref="Result"/> representing failure with the provided error.</returns>
/// <exception cref="InvalidOperationException">Thrown if <paramref name="error"/> is <see cref="Error.None"/>.</exception>
public static Result Failure(Error error) => new(false, error);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException();

    /// <summary>
/// Initializes a successful Result&lt;TValue&gt; that carries the specified value.
/// The created instance represents success and has Error.None as its error.
/// </summary>
/// <param name="value">The success value to store in the result.</param>
protected Result(TValue value) : base(true, Error.None) => _value = value;
    /// <summary>
/// Initializes a failed Result&lt;TValue&gt; with the specified error.
/// </summary>
/// <param name="error">The error describing the failure (used as the result's Error).</param>
/// <remarks>The result carries no value; the Value property is not valid for a failed result.</remarks>
protected Result(Error error) : base(false, error) => _value = default;
    
    /// <summary>
/// Creates a successful <see cref="Result{TValue}"/> that carries the provided value.
/// </summary>
/// <param name="value">The value to store in the successful result.</param>
/// <returns>A <see cref="Result{TValue}"/> representing success containing <paramref name="value"/>.</returns>
public static Result<TValue> Success(TValue value) => new(value);
    /// <summary>
/// Creates a failed <see cref="Result{TValue}"/> carrying the specified <paramref name="error"/>.
/// </summary>
/// <param name="error">The error describing the failure. Must not be <c>Error.None</c>.</param>
/// <returns>A <see cref="Result{TValue}"/> representing failure with the provided error.</returns>
/// <exception cref="InvalidOperationException">Thrown if <paramref name="error"/> is <c>Error.None</c>.</exception>
public static new Result<TValue> Failure(Error error) => new(error);

    /// <summary>
    /// Executa uma função com base no estado do resultado (Sucesso ou Falha).
    /// <summary>
    /// Executes one of two callbacks depending on whether the result is a success or a failure.
    /// </summary>
    /// <param name="onSuccess">Callback invoked with the successful value when the result represents success.</param>
    /// <param name="onFailure">Callback invoked with the associated <see cref="Error"/> when the result represents failure.</param>
    /// <returns>The value returned by the executed callback.</returns>
    public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<Error, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(Value) : onFailure(Error);
    }
}