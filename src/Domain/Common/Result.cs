namespace Raga.Domain.Common;

/// <summary>
/// Represents the outcome of a domain operation that returns a value.
/// </summary>
public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string ErrorCode { get; }
    public string ErrorMessage { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
        ErrorCode = string.Empty;
        ErrorMessage = string.Empty;
    }

    private Result(string errorCode, string errorMessage)
    {
        IsSuccess = false;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(string errorCode, string errorMessage) => new(errorCode, errorMessage);
}

/// <summary>
/// Represents the outcome of a domain operation that returns no value.
/// </summary>
public sealed class Result
{
    public bool IsSuccess { get; }
    public string ErrorCode { get; }
    public string ErrorMessage { get; }

    private Result()
    {
        IsSuccess = true;
        ErrorCode = string.Empty;
        ErrorMessage = string.Empty;
    }

    private Result(string errorCode, string errorMessage)
    {
        IsSuccess = false;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public static Result Success() => new();
    public static Result Failure(string errorCode, string errorMessage) => new(errorCode, errorMessage);
}
