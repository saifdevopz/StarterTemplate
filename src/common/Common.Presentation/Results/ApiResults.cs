using Common.Domain.Errors;
using Common.Domain.Results;
using Microsoft.AspNetCore.Http;

namespace Common.Presentation.Results;
public static class ApiResults
{
    public static IResult Problem(Result result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return result.IsSuccess
            ? throw new InvalidOperationException()
            : Microsoft.AspNetCore.Http.Results.Problem(
                title: GetTitle(result.Error),
                detail: GetDetail(result.Error),
                type: GetType(result.Error.Type),
                statusCode: GetStatusCode(result.Error.Type),
                extensions: GetErrors(result));

        static string GetTitle(CustomError error)
        {
            return error.Type switch
            {
                ErrorType.Validation => error.Code,
                ErrorType.Problem => error.Code,
                ErrorType.NotFound => error.Code,
                ErrorType.Conflict => error.Code,
                ErrorType.Failure => throw new NotImplementedException(),
                _ => "Server failure"
            };
        }

        static string GetDetail(CustomError error)
        {
            return error.Type switch
            {
                ErrorType.Validation => error.Description,
                ErrorType.Problem => error.Description,
                ErrorType.NotFound => error.Description,
                ErrorType.Conflict => error.Description,
                ErrorType.Failure => throw new NotImplementedException(),
                _ => "An unexpected error occurred"
            };
        }

        static string GetType(ErrorType errorType)
        {
            return errorType switch
            {
                ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                ErrorType.Problem => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                ErrorType.Failure => throw new NotImplementedException(),
                _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };
        }

        static int GetStatusCode(ErrorType errorType)
        {
            return errorType switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.Problem => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Failure => throw new NotImplementedException(),
                _ => StatusCodes.Status500InternalServerError
            };
        }

        static Dictionary<string, object?>? GetErrors(Result result)
        {
            return result.Error is not ValidationError validationError
                ? null
                : new Dictionary<string, object?>
                {
                        { "errors", validationError.Errors }
                };
        }
    }
}
