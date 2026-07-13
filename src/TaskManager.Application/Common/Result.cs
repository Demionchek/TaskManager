namespace TaskManager.Application.Common;

public enum ErrorType
{
    NotFound,
    Forbidden,
    BadRequest,
    Unauthorized
}

public class Result<T>
{
    public T? Value { get; }
    public ErrorType? Error { get; }
    public string? Message { get; }
    public bool IsSuccess => Error == null;

    private Result(T? value, ErrorType? error, string? message)
    {
        Value = value;
        Error = error;
        Message = message;
    }

    public static Result<T> Success(T value) => new(value, null, null);
    public static Result<T> Fail(ErrorType error, string? message = null) => new(default, error, message);
}
