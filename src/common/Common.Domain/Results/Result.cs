using Common.Domain.Errors;
using System.Diagnostics.CodeAnalysis;

namespace Common.Domain.Results;

public class Result
{
    public Result(bool isSuccess, CustomError error)
    {
        if ((isSuccess && error != CustomError.None) ||
                (!isSuccess && error == CustomError.None))
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public CustomError Error { get; }

    public static Result Success()
    {
        return new(true, CustomError.None);
    }

    public static Result<TValue> Success<TValue>(TValue value)
    {
        return new(value, true, CustomError.None);
    }

    public static Result Failure(CustomError error)
    {
        return new(false, error);
    }

    public static Result<TValue> Failure<TValue>(CustomError error)
    {
        return new(default, false, error);
    }
}

public class Result<TValue>(TValue? value, bool isSuccess, CustomError error) : Result(isSuccess, error)
{
    private readonly TValue? _value = value;

    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can't be accessed.");

    public static implicit operator Result<TValue>(TValue? value)
    {
        return value is not null ? Success(value) : Failure<TValue>(CustomError.NullValue);
    }

#pragma warning disable CA1000 // Do not declare static members on generic types
    public static Result<TValue> ValidationFailure(CustomError error)
#pragma warning restore CA1000 // Do not declare static members on generic types
    {
        return new(default, false, error);
    }

}
