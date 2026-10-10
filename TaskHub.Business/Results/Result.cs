namespace TaskHub.Business.Results;

public class Result<TValue>
{
    public TValue? Value { get; }
    public string? Error { get; }
    public bool IsSuccess => Error == null;

    private Result(TValue? value, string? error)
    {
        Value = value;
        Error = error;
    }

    public static Result<TValue> Success(TValue value) => 
        new(value, null);
    public static Result<TValue> Failure(string error) => 
        new(default, error);
}

public static class Result
{
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    public static Result<T> Failure<T>(string error) => Result<T>.Failure(error);
}