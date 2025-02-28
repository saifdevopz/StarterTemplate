using Common.Domain.Errors;

namespace Common.Application.Exceptions;

public sealed class StarterException(string requestName, CustomError? error = default, Exception? innerException = default)
    : Exception("Application exception", innerException)
{
    public string RequestName { get; } = requestName;

    public CustomError? Error { get; } = error;

}
