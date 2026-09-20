namespace ShopFlow.Common;

public class Result
{
    protected Result(bool isSuccess, string message, ErrorType errorType)
    {
        IsSuccess = isSuccess;
        Message = message;
        ErrorType = errorType;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public string Message { get; }

    public ErrorType ErrorType { get; }

    public static Result Success(string message = "") => new(true, message, ErrorType.None);

    public static Result Failure(string message, ErrorType errorType = ErrorType.Validation)
        => new(false, message, errorType);

    public static Result NotFound(string message) => new(false, message, ErrorType.NotFound);

    public static Result Conflict(string message) => new(false, message, ErrorType.Conflict);

    public static Result Forbidden(string message) => new(false, message, ErrorType.Forbidden);
}

public sealed class Result<TValue> : Result
{
    private Result(TValue? value, bool isSuccess, string message, ErrorType errorType)
        : base(isSuccess, message, errorType)
    {
        Value = value;
    }

    public TValue? Value { get; }

    public static Result<TValue> Success(TValue value, string message = "")
        => new(value, true, message, ErrorType.None);

    public static new Result<TValue> Failure(string message, ErrorType errorType = ErrorType.Validation)
        => new(default, false, message, errorType);

    public static new Result<TValue> NotFound(string message)
        => new(default, false, message, ErrorType.NotFound);

    public static new Result<TValue> Conflict(string message)
        => new(default, false, message, ErrorType.Conflict);

    public static new Result<TValue> Forbidden(string message)
        => new(default, false, message, ErrorType.Forbidden);
}
