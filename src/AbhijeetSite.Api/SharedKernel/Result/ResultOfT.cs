namespace AbhijeetSite.Api.SharedKernel.Result;

/// <summary>
/// Represents the outcome of an operation that returns a value.
/// </summary>
/// <typeparam name="TValue">Returned value type.</typeparam>
public readonly struct Result<TValue>
{
    private readonly TValue? _value;

    private Result(TValue? value, Error? error, bool isSuccess)
    {
        _value = value;
        Error = error;
        IsSuccess = isSuccess;
    }

    /// <summary>
    /// Gets whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the operation error when the result failed.
    /// </summary>
    public Error? Error { get; }

    /// <summary>
    /// Gets the successful value or raises a programmer error if the result failed.
    /// </summary>
    public TValue Value
    {
        get
        {
            if (!IsSuccess || _value is null)
            {
                throw new InvalidOperationException("Cannot read Value from a failed result.");
            }

            return _value;
        }
    }

    /// <summary>
    /// Creates a successful result with a value.
    /// </summary>
    public static Result<TValue> Success(TValue value)
    {
        return new Result<TValue>(value, null, true);
    }

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    public static Result<TValue> Failure(Error error)
    {
        return new Result<TValue>(default, error, false);
    }
}
