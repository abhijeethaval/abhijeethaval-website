namespace AbhijeetSite.Api.SharedKernel.Result;

/// <summary>
/// Represents the outcome of an operation that does not return a value.
/// </summary>
public readonly struct Result
{
    private Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
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
    /// Creates a successful result.
    /// </summary>
    public static Result Success()
    {
        return new Result(true, null);
    }

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    public static Result Failure(Error error)
    {
        return new Result(false, error);
    }
}
